using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dokan;

namespace tafs.FileSystem
{
    public class PhysicalDirectory : IWriteableDirectory
    {
        private readonly DirectoryInfo _directory;
        private readonly VirtualPathProvider _virtualPathProvider;

        public PhysicalDirectory(string path, VirtualPathProvider virtualPathProvider) : this(new DirectoryInfo(path), virtualPathProvider) { }

        public PhysicalDirectory(DirectoryInfo directory, VirtualPathProvider virtualPathProvider)
        {
            _directory = directory;
            _virtualPathProvider = virtualPathProvider;
        }

        public void Create()
        {
            _directory.Create();
        }

        public void Delete()
        {
            _directory.Delete(true);
        }

        public FileInformation GetFileInformation()
        {
            return new FileInformation
            {
                Attributes = Attributes,
                CreationTime = CreationTime,
                LastAccessTime = LastAccessTime,
                LastWriteTime = LastWriteTime,
                FileName = Name
            };
        }

        public List<string> GetChildren()
        {
            return _directory.GetFileSystemInfos().Select(dir => dir.FullName).ToList();
        }

        public FileAttributes Attributes { get { return _directory.Attributes; } }
        public DateTime CreationTime { get { return _directory.CreationTime; } }
        public DateTime LastAccessTime { get { return _directory.LastAccessTime; } }
        public DateTime LastWriteTime { get { return _directory.LastWriteTime; } }

        public bool Exists { get { return _directory.Exists; } }
        public bool IsDirectory { get { return true; } }
        public bool IsFile { get { return false; } }
        public string Name { get { return _directory.Name; } }
        public string FullName { get { return _directory.FullName; } }

        public override string ToString()
        {
            return FullName;
        }
    }
}