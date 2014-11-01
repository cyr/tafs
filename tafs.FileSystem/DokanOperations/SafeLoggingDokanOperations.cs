using System;
using System.Collections;
using System.IO;
using Dokan;

namespace tafs.FileSystem
{
    public class SafeLoggingDokanOperations : DokanOperations
    {
        private readonly DokanOperations _dokanOperations;

        public SafeLoggingDokanOperations(DokanOperations dokanOperations)
        {
            _dokanOperations = dokanOperations;
        }

        private int TryCatchAndLog(Func<int> action, string log, DokanFileInfo info)
        {
            try
            {
                int result = action();

                Console.WriteLine(log);
                LogHandle(info);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        private static void LogHandle(DokanFileInfo info)
        {
            if (info.Context != null)
                Console.WriteLine("\tHandle: {0}", info.Context);
        }

        public int CreateFile(string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.CreateFile(filename, access, share, mode, options, info), string.Format("CreateFile: {0}", filename), info);
        }

        public int OpenDirectory(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.OpenDirectory(filename, info), string.Format("OpenDirectory: {0}", filename), info);
        }

        public int CreateDirectory(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.CreateDirectory(filename, info), string.Format("CreateDirectory: {0}", filename), info);
        }

        public int Cleanup(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.Cleanup(filename, info), string.Format("Cleanup: {0}", filename), info);
        }

        public int CloseFile(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.CloseFile(filename, info), string.Format("CloseFile: {0}", filename), info);
        }

        public int ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
        {
            try
            {
                int result = _dokanOperations.ReadFile(filename, buffer, ref readBytes, offset, info);

                Console.WriteLine("ReadFile: {0}", filename);
                LogHandle(info);

                return result;
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
                int result = _dokanOperations.WriteFile(filename, buffer, ref writtenBytes, offset, info);

                Console.WriteLine("WriteFile: {0}", filename);
                LogHandle(info);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public int FlushFileBuffers(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.FlushFileBuffers(filename, info), string.Format("FlushFileBuffers: {0}", filename), info);
        }

        public int GetFileInformation(string filename, FileInformation fileinfo, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.GetFileInformation(filename, fileinfo, info), string.Format("GetFileInformation: {0}", filename), info);
        }

        public int FindFiles(string filename, ArrayList files, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.FindFiles(filename, files, info), string.Format("FindFiles: {0}", filename), info);
        }

        public int SetFileAttributes(string filename, FileAttributes attr, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.SetFileAttributes(filename, attr, info), string.Format("CreateFile: {0}", filename), info);
        }

        public int SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.SetFileTime(filename, ctime, atime, mtime, info), string.Format("SetFileTime: {0}", filename), info);
        }

        public int DeleteFile(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.DeleteFile(filename, info), string.Format("DeleteFile: {0}", filename), info);
        }

        public int DeleteDirectory(string filename, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.DeleteDirectory(filename, info), string.Format("DeleteDirectory: {0}", filename), info);
        }

        public int MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.MoveFile(filename, newname, replace, info), string.Format("MoveFile: {0}", filename), info);
        }

        public int SetEndOfFile(string filename, long length, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.SetEndOfFile(filename, length, info), string.Format("SetEndOfFile: {0}", filename), info);
        }

        public int SetAllocationSize(string filename, long length, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.SetAllocationSize(filename, length, info), string.Format("SetAllocationSize: {0}", filename), info);
        }

        public int LockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.LockFile(filename, offset, length, info), string.Format("LockFile: {0}", filename), info);
        }

        public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.UnlockFile(filename, offset, length, info), string.Format("UnlockFile: {0}", filename), info);
        }

        public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
        {
            try
            {
                int result = _dokanOperations.GetDiskFreeSpace(ref freeBytesAvailable, ref totalBytes, ref totalFreeBytes, info);

                Console.WriteLine("GetDiskFreeSpace: available({0}), total({1}), totalfree({2})", freeBytesAvailable, totalBytes, totalFreeBytes);
                LogHandle(info);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public int Unmount(DokanFileInfo info)
        {
            return TryCatchAndLog(() => _dokanOperations.Unmount(info), "Unmount", info);
        }
    }
}
