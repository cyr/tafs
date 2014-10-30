using System;
using System.Collections.Generic;
using System.IO;
using Dokan;
using SevenZip;

namespace tafs.FileSystem
{
    public class ArchiveContentFolder : IVirtualDirectory
    {
        private readonly ArchiveDirectory _archive;

        public ArchiveContentFolder(ArchiveDirectory archive, ArchiveFileInfo archiveFileInfo, string path)
        {
            _archive = archive;

            Name = Path.GetFileName(path);
            FullName = path;
            Attributes = FileAttributes.Directory;
            CreationTime = archiveFileInfo.CreationTime;
            LastAccessTime = archiveFileInfo.LastAccessTime;
            LastWriteTime = archiveFileInfo.LastWriteTime;
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
            return new List<string>();
        }

        public bool Exists { get { return true; } }
        public bool IsDirectory { get { return true; } }
        public bool IsFile { get { return false; } }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public FileAttributes Attributes { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
    }
}