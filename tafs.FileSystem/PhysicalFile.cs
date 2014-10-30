using System;
using System.IO;
using Dokan;

namespace tafs.FileSystem
{
    public class PhysicalFile : IWriteableFile
    {
        private readonly FileInfo _file;

        public PhysicalFile(string path) : this(new FileInfo(path)) { }

        public PhysicalFile(FileInfo file)
        {
            _file = file;
        }

        public Stream Create(FileMode mode, FileAccess access, FileShare share)
        {
            return _file.Open(mode, access, share);
        }

        public void Delete()
        {
            _file.Delete();
        }

        public void SetAttributes(FileAttributes attr)
        {
            File.SetAttributes(_file.FullName, attr);
        }

        public void SetFileTime(DateTime ctime, DateTime atime, DateTime mtime)
        {
            File.SetCreationTime(_file.FullName, ctime);
            File.SetLastAccessTime(_file.FullName, atime);
            File.SetLastWriteTime(_file.FullName, mtime);
        }

        public void MoveTo(IVirtualPath targetPath)
        {
            _file.MoveTo(targetPath.FullName);
        }

        public void AllocateSize(long length)
        {
            using (var stream = _file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                stream.SetLength(length);
            }
        }

        public Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return _file.Open(mode, access, share);
        }

        public FileInformation GetFileInformation()
        {
            return new FileInformation
            {
                Attributes = Attributes, 
                CreationTime = CreationTime, 
                LastAccessTime = LastAccessTime, 
                LastWriteTime = LastWriteTime, 
                Length = Size, 
                FileName = Name
            };
        }

        public DateTime LastAccessTime { get { return _file.LastAccessTime; } }
        public DateTime LastWriteTime { get { return _file.LastWriteTime; } }
        public DateTime CreationTime { get { return _file.CreationTime; } }
        public FileAttributes Attributes { get { return _file.Attributes; } }
        public long Size { get { return _file.Length; } }
        public bool IsDirectory { get { return false; }}
        public bool IsFile { get { return true; } }
        public string Name { get { return _file.Name; } }
        public string FullName { get { return _file.FullName; } }
        public bool Exists { get { return _file.Exists; } }
    }
}