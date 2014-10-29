using System.IO;

namespace tafs.FileSystem
{
    public interface IWriteableFile : IVirtualFile
    {
        Stream Create(FileMode mode, FileAccess access, FileShare share);
    }
}