using System;
using System.Collections.Generic;
using System.IO;
using Dokan;

namespace tafs.FileSystem
{
    public class ArchiveDirectory : IVirtualDirectory
    {
        private readonly PhysicalFile _archiveFile;

        public ArchiveDirectory(PhysicalFile archiveFile)
        {
            _archiveFile = archiveFile;

            Attributes = FileAttributes.Directory | FileAttributes.Compressed;
        }

        public bool Exists { get { return true; } }
        public bool IsDirectory { get { return true; } }
        public bool IsFile { get { return false; } }
        public string Name { get { return _archiveFile.Name; } }
        public string FullName { get { return _archiveFile.FullName; } }

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

        public FileAttributes Attributes { get; private set; }
        public DateTime CreationTime { get { return _archiveFile.CreationTime; } }
        public DateTime LastAccessTime { get { return _archiveFile.LastAccessTime; } }
        public DateTime LastWriteTime { get { return _archiveFile.LastWriteTime; } }

        public List<string> GetChildren()
        {
            return new List<string>();
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}