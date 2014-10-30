using System.IO;
using Dokan;

namespace tafs.FileSystem
{
    public class NonExistingPath : IVirtualPath
    {
        private readonly string _path;

        public NonExistingPath(string path)
        {
            _path = path;

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
            return new PhysicalDirectory(_path);
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
    }
}