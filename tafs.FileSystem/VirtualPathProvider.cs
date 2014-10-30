using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dokan;

namespace tafs.FileSystem
{
    public class VirtualPathProvider
    {
        private readonly string[] _archiveExtensions = { ".rar" };
        private readonly DirectoryInfo _rootDirectory;

        public VirtualPathProvider(string rootDirectory)
        {
            _rootDirectory = new DirectoryInfo(rootDirectory);
        }

        public IVirtualPath GetVirtualPath(string filename)
        {
            var path = GetPhysicalPath(filename);

            if (Directory.Exists(path))
                return new PhysicalDirectory(path, this);

            if (File.Exists(path))
            {
                var physicalFile = new PhysicalFile(path);

                if (FileIsArchive(physicalFile))
                    return new ArchiveDirectory(physicalFile);

                return physicalFile;
            }

            return new NonExistingPath(path, this);
        }

        public DriveInfo GetDriveInfoForRoot()
        {
            return DriveInfo.GetDrives().FirstOrDefault(info => info.RootDirectory.FullName == _rootDirectory.Root.FullName);
        }

        public List<FileInformation> GetChildren(IVirtualDirectory virtualDirectory)
        {
            return virtualDirectory.GetChildren().Select(path => GetVirtualPath(path).GetFileInformation()).ToList();
        }

        private string GetPhysicalPath(string dokanFilename)
        {
            if (PathIsRooted(dokanFilename))
                return dokanFilename;

            return GetRootedPath(dokanFilename);
        }

        private bool FileIsArchive(PhysicalFile physicalFile)
        {
            return _archiveExtensions.Contains(physicalFile.Extension);
        }

        private string GetRootedPath(string dokanFilename)
        {
            return Path.Combine(_rootDirectory.FullName, dokanFilename.TrimStart(Path.DirectorySeparatorChar));
        }

        private bool PathIsRooted(string dokanFilename)
        {
            return dokanFilename.StartsWith(_rootDirectory.Root.FullName);
        }
    }
}