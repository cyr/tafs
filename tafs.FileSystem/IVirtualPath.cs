using Dokan;

namespace tafs.FileSystem
{
    public interface IVirtualPath
    {
        bool Exists { get; }
        bool IsDirectory { get; }
        bool IsFile { get; }
        string Name { get; }
        string FullName { get; }
        FileInformation GetFileInformation();
    }
}