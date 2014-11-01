using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dokan;
using tafs.FileSystem.Handles;

namespace tafs.FileSystem
{
    public class VirtualFilesystem
    {
        private readonly VirtualPathProvider _pathProvider;
        private readonly VirtualFileSystemHandleManager _handleManager;

        public VirtualFilesystem(string pathToMount)
        {
            _pathProvider = new VirtualPathProvider(pathToMount);
            _handleManager = new VirtualFileSystemHandleManager();
        }

        public IVirtualPath GetVirtualPath(string filename)
        {
            return _pathProvider.GetVirtualPath(filename);
        }

        public int CreateDirectory(NonExistingPath path)
        {
            var virtualDirectory = path.ToDirectory();

            try
            {
                virtualDirectory.Create();

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int ReadFile(VirtualFileSystemHandleId handleId, byte[] buffer, ref uint readBytes, long offset)
        {
            try
            {
                var handle = _handleManager.GetHandle(handleId);

                if (handle.Stream.CanSeek && offset != 0)
                    handle.Stream.Seek(offset, SeekOrigin.Begin);

                readBytes = (uint)handle.Stream.Read(buffer, 0, buffer.Length);

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int CloseFile(VirtualFileSystemHandleId handleId)
        {
            try
            {
                _handleManager.CloseHandle(handleId);

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public VirtualFileSystemHandleId CreateHandle(IVirtualPath virtualPath, FileAccess access = (FileAccess)0, FileShare share = (FileShare)0, FileMode mode = (FileMode)0)
        {
            return _handleManager.CreateHandle(virtualPath, access, share, mode);
        }

        public int WriteFile(VirtualFileSystemHandleId handleId, byte[] buffer, ref uint writtenBytes, long offset)
        {
            try
            {
                var handle = _handleManager.GetHandle(handleId);

                handle.Stream.Write(buffer, (int)offset, buffer.Length);
                writtenBytes = (uint)buffer.Length;

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int PopulateFileInformation(IVirtualPath virtualPath, FileInformation fileinfo)
        {
            if (virtualPath.IsFile)
                return PopulateFileInformationFromFile(virtualPath as IVirtualFile, fileinfo);

            if (virtualPath.IsDirectory)
                return PopulateFileInformationFromDirectory(virtualPath as IVirtualDirectory, fileinfo);

            return DokanNet.DOKAN_ERROR;
        }

        public int PopulateListFromDirectory(IVirtualDirectory virtualDirectory, ArrayList files)
        {
            try
            {
                files.AddRange(GetFileInformationsFromVirtualPath(virtualDirectory));

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        public int SetAttributes(IWriteableFile writeableFile, FileAttributes attr)
        {
            try
            {
                writeableFile.SetAttributes(attr);
                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        public int SetFileTime(IWriteableFile writeableFile, DateTime ctime, DateTime atime, DateTime mtime)
        {
            try
            {
                writeableFile.SetFileTime(ctime, atime, mtime);
                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        public int Delete(IWriteableFile writeableFile)
        {
            try
            {
                writeableFile.Delete();
                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        public int Delete(IWriteableDirectory writeableDirectory)
        {
            try
            {
                writeableDirectory.Delete();
                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        public int Move(IWriteableFile writeableFile, IVirtualPath targetPath)
        {
            try
            {
                writeableFile.MoveTo(targetPath);
                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        public int AllocateSize(IWriteableFile writeableFile, long length)
        {
            try
            {
                writeableFile.AllocateSize(length);
                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return -DokanNet.ERROR_ACCESS_DENIED;
            }
        }

        private List<FileInformation> GetFileInformationsFromVirtualPath(IVirtualDirectory virtualDirectory)
        {
            return _pathProvider.GetChildren(virtualDirectory);
        }

        private int PopulateFileInformationFromDirectory(IVirtualDirectory virtualDirectory, FileInformation fileinfo)
        {
            fileinfo.FileName = virtualDirectory.Name;
            fileinfo.Attributes = virtualDirectory.Attributes;
            fileinfo.CreationTime = virtualDirectory.CreationTime;
            fileinfo.LastAccessTime = virtualDirectory.LastAccessTime;
            fileinfo.LastWriteTime = virtualDirectory.LastWriteTime;

            return DokanNet.DOKAN_SUCCESS;
        }

        private int PopulateFileInformationFromFile(IVirtualFile virtualFile, FileInformation fileinfo)
        {
            fileinfo.FileName = virtualFile.Name;
            fileinfo.Attributes = virtualFile.Attributes;
            fileinfo.CreationTime = virtualFile.CreationTime;
            fileinfo.LastAccessTime = virtualFile.LastAccessTime;
            fileinfo.LastWriteTime = virtualFile.LastWriteTime;
            fileinfo.Length = virtualFile.Size;

            return DokanNet.DOKAN_SUCCESS;
        }

        public int GetDiskFreeSpace(out ulong freeBytesAvailable, out ulong totalBytes, out ulong totalFreeBytes)
        {
            var driveInfo = GetDriveInfoForRootFolder();

            freeBytesAvailable = (ulong)driveInfo.AvailableFreeSpace;
            totalBytes = (ulong)driveInfo.TotalSize;
            totalFreeBytes = (ulong)driveInfo.TotalFreeSpace;

            return DokanNet.DOKAN_SUCCESS;
        }

        private DriveInfo GetDriveInfoForRootFolder()
        {
            return _pathProvider.GetDriveInfoForRoot();
        }
    }
}