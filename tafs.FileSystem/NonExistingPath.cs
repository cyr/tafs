using System.IO;
using Dokan;

namespace tafs.FileSystem
{
    public class NonExistingPath : IVirtualPath
    {
        private readonly string _path;
        private readonly VirtualPathProvider _virtualPathProvider;

        public NonExistingPath(string path, VirtualPathProvider virtualPathProvider)
        {
            _path = path;
            _virtualPathProvider = virtualPathProvider;

            Name = GetNamePart(path);
        }

        private static string GetNamePart(string path)
        {
            return path.TrimEnd(Path.DirectorySeparatorChar).Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }

        public IWriteableFile ToFile()
        {
            return new PhysicalFile(_path);
        }

        public IWriteableDirectory ToDirectory()
        {
            return new PhysicalDirectory(_path, _virtualPathProvider);
        }

        public FileInformation GetFileInformation()
        {
            return new FileInformation { Length = -1, FileName = Name, Attributes = new FileAttributes() };
        }

        public bool Exists { get { return false; } }
        public bool IsDirectory { get { return false; } }
        public bool IsFile { get { return false; } }
        public string Name { get; private set; }
        public string FullName { get { return _path; } }

        public override string ToString()
        {
            return FullName;
        }
    }
}