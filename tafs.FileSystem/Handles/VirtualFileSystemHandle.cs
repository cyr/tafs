using System.IO;

namespace tafs.FileSystem.Handles
{
    public class VirtualFileSystemHandle
    {
        public VirtualFileSystemHandle(IVirtualPath virtualPath) : this(virtualPath, null) { }

        public VirtualFileSystemHandle(IVirtualPath virtualPath, Stream stream)
        {
            VirtualPath = virtualPath;
            IsDirectory = virtualPath.IsDirectory;
            IsFile = virtualPath.IsFile;

            Stream = stream;
        }

        public void CloseHandle()
        {
            if (Stream != null)
                Stream.Close();
        }

        public IVirtualPath VirtualPath { get; private set; }
        public bool IsDirectory { get; private set; }
        public bool IsFile { get; private set; }
        public Stream Stream { get; private set; }

    }
}