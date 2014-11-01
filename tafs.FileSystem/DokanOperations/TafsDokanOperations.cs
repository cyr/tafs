using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Dokan;
using SevenZip;
using tafs.FileSystem.Handles;

namespace tafs.FileSystem
{
    public class TafsDokanOperations : DokanOperations, IDisposable
    {
        private readonly VirtualFilesystem _virtualFilesystem;

        public TafsDokanOperations(string pathToMount)
        {
            SevenZipBase.SetLibraryPath(GetSevenZipDllPath());

            _virtualFilesystem = new VirtualFilesystem(pathToMount);
        }

        public int CreateFile(string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options, DokanFileInfo info)
        {
            var virtualPath = _virtualFilesystem.GetVirtualPath(filename);

            info.Context = _virtualFilesystem.CreateHandle(virtualPath, access, share, mode);
            info.IsDirectory = virtualPath.IsDirectory;

            return info.Context != null ? DokanNet.DOKAN_SUCCESS : -DokanNet.ERROR_FILE_NOT_FOUND;
        }

        public int OpenDirectory(string filename, DokanFileInfo info)
        {
            var virtualPath = _virtualFilesystem.GetVirtualPath(filename);
            
            info.Context = _virtualFilesystem.CreateHandle(virtualPath);

            if (virtualPath.Exists && virtualPath.IsDirectory)
                return DokanNet.DOKAN_SUCCESS;

            return -DokanNet.ERROR_PATH_NOT_FOUND;
        }

        public int CreateDirectory(string filename, DokanFileInfo info)
        {
            var virtualPath = _virtualFilesystem.GetVirtualPath(filename);

            if (virtualPath.Exists)
                return -DokanNet.ERROR_ACCESS_DENIED;

            return _virtualFilesystem.CreateDirectory(virtualPath as NonExistingPath);
        }

        public int Cleanup(string filename, DokanFileInfo info)
        {
            // TODO: uuh, probably check what this method is actually supposed to do?
            return DokanNet.DOKAN_SUCCESS;
        }

        public int CloseFile(string filename, DokanFileInfo info)
        {
            var handleId = GetHandleId(info);

            return _virtualFilesystem.CloseFile(handleId);
        }

        public int ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
        {
            var handleId = GetHandleId(info);

            return _virtualFilesystem.ReadFile(handleId, buffer, ref readBytes, offset);
        }

        public int WriteFile(string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info)
        {
            var handleId = GetHandleId(info);

            return _virtualFilesystem.WriteFile(handleId, buffer, ref writtenBytes, offset);
        }

        public int FlushFileBuffers(string filename, DokanFileInfo info)
        {
            return DokanNet.DOKAN_SUCCESS;
        }

        public int GetFileInformation(string filename, FileInformation fileinfo, DokanFileInfo info)
        {
            var virtualPath = _virtualFilesystem.GetVirtualPath(filename);

            return _virtualFilesystem.PopulateFileInformation(virtualPath, fileinfo);
        }

        public int FindFiles(string filename, ArrayList files, DokanFileInfo info)
        {
            return ExecuteIfType<IVirtualDirectory>(filename,
                virtualDirectory => _virtualFilesystem.PopulateListFromDirectory(virtualDirectory, files));
        }

        public int SetFileAttributes(string filename, FileAttributes attr, DokanFileInfo info)
        {
            return ExecuteIfType<IWriteableFile>(filename, 
                writeableFile => _virtualFilesystem.SetAttributes(writeableFile, attr));
        }

        public int SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
        {
            return ExecuteIfType<IWriteableFile>(filename, 
                writeableFile => _virtualFilesystem.SetFileTime(writeableFile, ctime, atime, mtime));
        }

        public int DeleteFile(string filename, DokanFileInfo info)
        {
            return ExecuteIfType<IWriteableFile>(filename, 
                writeableFile =>_virtualFilesystem.Delete(writeableFile));
        }

        public int DeleteDirectory(string filename, DokanFileInfo info)
        {
            return ExecuteIfType<IWriteableDirectory>(filename, 
                writeableDirectory => _virtualFilesystem.Delete(writeableDirectory));
        }

        public int MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
        {
            var targetPath = _virtualFilesystem.GetVirtualPath(newname);

            return ExecuteIfType<IWriteableFile>(filename,
                writeableFile => _virtualFilesystem.Move(writeableFile, targetPath));
        }

        public int SetEndOfFile(string filename, long length, DokanFileInfo info)
        {
            // TODO: Support keeping streams open
            return DokanNet.DOKAN_SUCCESS;
        }

        public int SetAllocationSize(string filename, long length, DokanFileInfo info)
        {
            return ExecuteIfType<IWriteableFile>(filename,
                writeableFile => _virtualFilesystem.AllocateSize(writeableFile, length));
        }

        public int LockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            // TODO: Locking?
            return DokanNet.DOKAN_SUCCESS;
        }

        public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            // TODO: Locking?
            return DokanNet.DOKAN_SUCCESS;
        }

        public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
        {
            return _virtualFilesystem.GetDiskFreeSpace(out freeBytesAvailable, out totalBytes, out totalFreeBytes);
        }

        public int Unmount(DokanFileInfo info)
        {
            // TODO: Unmount routines?
            return DokanNet.DOKAN_SUCCESS;
        }

        public void Dispose()
        {
        }

        private static VirtualFileSystemHandleId GetHandleId(DokanFileInfo info)
        {
            var handleId = info.Context as VirtualFileSystemHandleId;

            if (handleId == null)
                throw new Exception("Trying to close file without providing a handle.");

            return handleId;
        }

        private int ExecuteIfType<T>(string filename, Func<T, int> func) where T : class, IVirtualPath
        {
            var virtualPath = _virtualFilesystem.GetVirtualPath(filename);
            var instanceOfType = virtualPath as T;

            if (instanceOfType == null)
                return DokanNet.DOKAN_ERROR;

            return func(instanceOfType);
        }

        private static string GetSevenZipDllPath()
        {
            return Path.Combine(GetExecutingPath(), "7z.dll");
        }

        private static string GetExecutingPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
