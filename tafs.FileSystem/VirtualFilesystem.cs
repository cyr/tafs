using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dokan;

namespace tafs.FileSystem
{
    public class VirtualFilesystem
    {
        private readonly DirectoryInfo _rootFolder;

        public VirtualFilesystem(string pathToMount)
        {
            _rootFolder = new DirectoryInfo(pathToMount);
        }

        public IVirtualPath GetVirtualPath(string filename)
        {
            var path = GetPhysicalPath(filename);

            if (Directory.Exists(path))
                return new PhysicalDirectory(path);

            if (File.Exists(path))
                return new PhysicalFile(path);

            return new NonExistingPath(path);
        }

        private string GetPhysicalPath(string dokanFilename)
        {
            return Path.Combine(_rootFolder.FullName, dokanFilename.TrimStart(Path.DirectorySeparatorChar));
        }

        public int CreateFile(IVirtualPath virtualPath, FileAccess access, FileShare share, FileMode mode)
        {
            if (!virtualPath.Exists)
                virtualPath = ConvertNonExistingPathToVirtualFilePath(virtualPath);

            if (virtualPath is IWriteableFile)
                return Create(virtualPath as IWriteableFile, access, share, mode);

            return OpenForReading(virtualPath as IVirtualFile, access, share, mode);
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

        public int ReadFile(IVirtualFile virtualFile, byte[] buffer, ref uint readBytes, long offset)
        {
            try
            {
                using (var stream = virtualFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    readBytes = (uint)stream.Read(buffer, 0, buffer.Length);

                    return DokanNet.DOKAN_SUCCESS;
                }
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        public int WriteFile(IWriteableFile writeableFile, byte[] buffer, ref uint writtenBytes, long offset)
        {
            try
            {
                using (var stream = writeableFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Write(buffer, (int)offset, buffer.Length);
                    writtenBytes = (uint)buffer.Length;

                    return DokanNet.DOKAN_SUCCESS;
                }
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

        public int PopulateListFromDirectoryChildren(IVirtualDirectory virtualDirectory, ArrayList files)
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

        private static List<FileInformation> GetFileInformationsFromVirtualPath(IVirtualDirectory virtualDirectory)
        {
            return virtualDirectory.GetChildren().Select(virtualPath => virtualPath.GetFileInformation()).ToList();
        }

        private static IVirtualPath ConvertNonExistingPathToVirtualFilePath(IVirtualPath virtualPath)
        {
            return ((NonExistingPath)virtualPath).ToFile();
        }

        private int OpenForReading(IVirtualFile virtualFile, FileAccess access, FileShare share, FileMode mode)
        {
            if (access.HasFlag(FileAccess.Write))
                return -DokanNet.ERROR_ACCESS_DENIED;

            return Open(virtualFile, access, share, mode);
        }

        private static int Open(IVirtualFile virtualFile, FileAccess access, FileShare share, FileMode mode)
        {
            try
            {
                virtualFile.Open(mode, access, share).Dispose(); // TODO: Keep stream open?

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
        }

        private static int Create(IWriteableFile writeableFile, FileAccess access, FileShare share, FileMode mode)
        {
            try
            {
                writeableFile.Create(mode, access, share).Dispose(); // TODO: Keep stream open?

                return DokanNet.DOKAN_SUCCESS;
            }
            catch (Exception)
            {
                return DokanNet.DOKAN_ERROR;
            }
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
            return DriveInfo.GetDrives().FirstOrDefault(info => info.RootDirectory.FullName == _rootFolder.Root.FullName);
        }
    }
}