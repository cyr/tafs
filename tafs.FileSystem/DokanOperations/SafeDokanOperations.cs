using System;
using System.Collections;
using System.IO;
using Dokan;

namespace tafs.FileSystem
{
    public class SafeDokanOperations : DokanOperations
    {
        private readonly DokanOperations _dokanOperations;

        public SafeDokanOperations(DokanOperations dokanOperations)
        {
            _dokanOperations = dokanOperations;
        }

        private int TryCatch(Func<int> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public int CreateFile(string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options, DokanFileInfo info)
        {
            return TryCatch(() =>_dokanOperations.CreateFile(filename, access, share, mode, options, info));
        }

        public int OpenDirectory(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.OpenDirectory(filename, info));
        }

        public int CreateDirectory(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.CreateDirectory(filename, info));
        }

        public int Cleanup(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.Cleanup(filename, info));
        }

        public int CloseFile(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.CloseFile(filename, info));
        }

        public int ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
        {
            try
            {
                return _dokanOperations.ReadFile(filename, buffer, ref readBytes, offset, info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public int WriteFile(string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info)
        {
            try
            {
                return _dokanOperations.WriteFile(filename, buffer, ref writtenBytes, offset, info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public int FlushFileBuffers(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.FlushFileBuffers(filename, info));
        }

        public int GetFileInformation(string filename, FileInformation fileinfo, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.GetFileInformation(filename, fileinfo, info));
        }

        public int FindFiles(string filename, ArrayList files, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.FindFiles(filename, files, info));
        }

        public int SetFileAttributes(string filename, FileAttributes attr, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.SetFileAttributes(filename, attr, info));
        }

        public int SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.SetFileTime(filename, ctime, atime, mtime, info));
        }

        public int DeleteFile(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.DeleteFile(filename, info));
        }

        public int DeleteDirectory(string filename, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.DeleteDirectory(filename, info));
        }

        public int MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.MoveFile(filename, newname, replace, info));
        }

        public int SetEndOfFile(string filename, long length, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.SetEndOfFile(filename, length, info));
        }

        public int SetAllocationSize(string filename, long length, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.SetAllocationSize(filename, length, info));
        }

        public int LockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.LockFile(filename, offset, length, info));
        }

        public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.UnlockFile(filename, offset, length, info));
        }

        public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
        {
            try
            {
                return _dokanOperations.GetDiskFreeSpace(ref freeBytesAvailable, ref totalBytes, ref totalFreeBytes, info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public int Unmount(DokanFileInfo info)
        {
            return TryCatch(() => _dokanOperations.Unmount(info));
        }
    }
}
