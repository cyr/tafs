using System;
using System.Collections.Generic;
using System.IO;

namespace tafs.FileSystem
{
    public interface IVirtualDirectory : IVirtualPath
    {
        void Create();
        FileAttributes Attributes { get; }
        DateTime CreationTime { get; }
        DateTime LastAccessTime { get; }
        DateTime LastWriteTime { get; }
        List<IVirtualPath> GetChildren();
    }
}