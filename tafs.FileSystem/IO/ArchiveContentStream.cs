using System;
using System.IO;
using System.Threading;
using SevenZip;

namespace tafs.FileSystem.IO
{
    public class ArchiveContentStream : Stream
    {
        private readonly SevenZipExtractor _extractor;
        private readonly object _readLock = new object();
        private readonly object _writeLock = new object();
        private bool _closed = false;
        private bool _isWritingStalled = false;

        private readonly CircularBuffer<byte> _buffer = new CircularBuffer<byte>(4096);

        public ArchiveContentStream(SevenZipExtractor extractor)
        {
            _extractor = extractor;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin != SeekOrigin.Begin)
                throw new Exception("Only SeekOrigin.Begin is supported.");

            var tempBuffer = new byte[4096];

            long bytesToSkip = offset;

            while ((bytesToSkip -= Read(tempBuffer, 0, tempBuffer.Length)) > 0) ;

            return offset;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        private bool IsWriteable()
        {
            return GetWriteableCount() > 0;
        }

        private long GetWriteableCount()
        {
            return _buffer.Capacity - _buffer.Size;
        }

        public override void Close()
        {
            _closed = true;
            lock (_readLock)
            {
                Monitor.Pulse(_readLock);
            }
            lock (_writeLock)
            {
                Monitor.Pulse(_writeLock);
            }

            base.Close();

            //_extractor.Dispose();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            while (true)
            {
                lock (_readLock)
                {
                    int readCount = _buffer.Get(buffer, offset, count);
                    if (readCount == 0)
                    {
                        if (_closed)
                            return 0;
                        Monitor.Wait(_readLock);
                        continue;
                    }

                    if (_isWritingStalled)
                    {
                        lock (_writeLock)
                        {
                            Monitor.Pulse(_writeLock);
                        }
                    }
                    return readCount;
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_readLock)
            {
                int writeCount = Math.Min((int)GetWriteableCount(), count - offset);
                while (offset < count)
                {
                    if (!IsWriteable())
                    {
                        _isWritingStalled = true;
                        lock (_writeLock)
                        {
                            Monitor.Exit(_readLock);
                            Monitor.Wait(_writeLock);
                            Monitor.Enter(_readLock);
                        }
                        _isWritingStalled = false;
                        if (_closed)
                            break;
                    }
                    _buffer.Put(buffer, offset, writeCount);
                    offset += writeCount;
                    writeCount = Math.Min((int)GetWriteableCount(), count - offset);

                    Monitor.Pulse(_readLock);
                }
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
