using System;
using System.IO;

namespace tafs.FileSystem
{
    public interface IVirtualFile : IVirtualPath
    {
        DateTime LastAccessTime { get; }
        DateTime LastWriteTime { get; }
        DateTime CreationTime { get; }
        FileAttributes Attributes { get; }
        long Size { get; }
        Stream Open(FileMode mode, FileAccess access, FileShare share);
    }
}