using System;
using System.IO;

namespace tafs.FileSystem
{
    public interface IWriteableFile : IVirtualFile
    {
        Stream Create(FileMode mode, FileAccess access, FileShare share);
        void Delete();
        void SetAttributes(FileAttributes attr);
        void SetFileTime(DateTime ctime, DateTime atime, DateTime mtime);
        void MoveTo(IVirtualPath targetPath);
        void AllocateSize(long length);
    }
}