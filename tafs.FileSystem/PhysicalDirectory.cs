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

        public PhysicalDirectory(string path) : this(new DirectoryInfo(path)) { }

        public PhysicalDirectory(DirectoryInfo directory)
        {
            _directory = directory;
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

        public List<IVirtualPath> GetChildren()
        {
            var children = new List<IVirtualPath>();
            
            children.AddRange(_directory.GetDirectories().Select(dir => new PhysicalDirectory(dir.FullName)));
            children.AddRange(_directory.GetFiles().Select(file => new PhysicalFile(file.FullName)));

            return children;
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
    }
}