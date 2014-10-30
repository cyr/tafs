using System;
using System.IO;
using Dokan;
using SevenZip;

namespace tafs.FileSystem
{
    public class ArchiveContentFile : IVirtualFile
    {
        private readonly ArchiveDirectory _archive;

        public ArchiveContentFile(ArchiveDirectory archive, ArchiveFileInfo archiveFileInfo, string path)
        {
            _archive = archive;

            FullName = path;
            Name = Path.GetFileName(path);
            LastAccessTime = archiveFileInfo.LastAccessTime;
            LastWriteTime = archiveFileInfo.LastWriteTime;
            CreationTime = archiveFileInfo.CreationTime;
            Attributes = new FileAttributes();
            Size = (long)archiveFileInfo.Size;
        }

        public Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            throw new NotImplementedException();
        }

        public FileInformation GetFileInformation()
        {
            return new FileInformation
            {
                Attributes = Attributes,
                CreationTime = CreationTime,
                LastAccessTime = LastAccessTime,
                LastWriteTime = LastWriteTime,
                Length = Size,
                FileName = Name
            };
        }

        public bool Exists { get { return true; } }
        public bool IsDirectory { get { return false; } }
        public bool IsFile { get { return false; } }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public DateTime CreationTime { get; private set; }
        public FileAttributes Attributes { get; private set; }
        public long Size { get; private set; }
    }
}