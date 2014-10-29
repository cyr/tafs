using System.IO;

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

        public IVirtualFile ToFile()
        {
            return new PhysicalFile(_path);
        }

        public IVirtualDirectory ToDirectory()
        {
            return new PhysicalDirectory(_path);
        }

        public bool Exists { get { return false; } }
        public bool IsDirectory { get { return false; } }
        public bool IsFile { get { return false; } }
        public string Name { get; private set; }
        public string FullName { get { return _path; } }

    }
}