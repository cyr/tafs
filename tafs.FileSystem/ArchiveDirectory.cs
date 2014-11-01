using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dokan;
using SevenZip;
using tafs.FileSystem.IO;

namespace tafs.FileSystem
{
    public class ArchiveDirectory : IVirtualDirectory
    {
        private readonly PhysicalFile _archiveFile;
        private readonly VirtualPathProvider _virtualPathProvider;

        public ArchiveDirectory(PhysicalFile archiveFile, VirtualPathProvider virtualPathProvider)
        {
            _archiveFile = archiveFile;
            _virtualPathProvider = virtualPathProvider;

            Attributes = FileAttributes.Directory | FileAttributes.Compressed;
        }

        public FileInformation GetFileInformation()
        {
            return new FileInformation
            {
                Attributes = Attributes,
                CreationTime = CreationTime,
                LastAccessTime = LastAccessTime,
                LastWriteTime = LastWriteTime,
                FileName = Name
            };
        }

        public List<string> GetChildren()
        {
            using (var extractor = new SevenZipExtractor(FullName))
            {
                return GetArchiveContent(extractor);
            }
        }

        public IVirtualPath GetArchivePath(string originalPath)
        {
            using (var extractor = new SevenZipExtractor(FullName))
            {
                var archiveFileInfo = GetArchiveFileInfo(extractor, originalPath);
                
                if (!archiveFileInfo.HasValue)
                    return new NonExistingPath(originalPath, _virtualPathProvider);

                return ReturnArchiveContentPath(originalPath, archiveFileInfo.Value);
            }
        }

        public Stream OpenFileStream(ArchiveContentFile contentFile)
        {
            var extractor = new SevenZipExtractor(FullName);
            var localPath = GetArchiveLocalPath(contentFile.FullName);

            var archiveContentStream = new ArchiveContentStream(extractor);

            Task.Run(() =>
            {
                try
                {
                    extractor.ExtractFile(localPath, archiveContentStream);
                }
                catch {
                    
                }
                
            });

            return archiveContentStream;
        }

        private IVirtualPath ReturnArchiveContentPath(string originalPath, ArchiveFileInfo archiveFileInfo)
        {
            if (archiveFileInfo.IsDirectory)
                return new ArchiveContentFolder(this, archiveFileInfo, originalPath);

            return new ArchiveContentFile(this, archiveFileInfo, originalPath);
        }

        private List<string> GetArchiveContent(SevenZipExtractor extractor)
        {
            return extractor.ArchiveFileData.Select(s => GetFullNameFromArchiveFile(s.FileName)).ToList();
        }

        private ArchiveFileInfo? GetArchiveFileInfo(SevenZipExtractor extractor, string originalPath)
        {
            var localPath = GetArchiveLocalPath(originalPath);

            foreach (var archiveFileInfo in GetArchiveContentByPath(extractor, localPath))
            {
                return archiveFileInfo;
            }

            return null;
        }

        private static IEnumerable<ArchiveFileInfo> GetArchiveContentByPath(SevenZipExtractor extractor, string localPath)
        {
            return extractor.ArchiveFileData.Where(path => path.FileName == localPath);
        }

        private string GetArchiveLocalPath(string originalPath)
        {
            return originalPath.StartsWith(FullName) ? originalPath.Substring(FullName.Length + 1) : originalPath;
        }

        private string GetFullNameFromArchiveFile(string fileName)
        {
            return Path.Combine(FullName, fileName);
        }

        public bool Exists { get { return true; } }
        public bool IsDirectory { get { return true; } }
        public bool IsFile { get { return false; } }
        public string Name { get { return _archiveFile.Name; } }
        public string FullName { get { return _archiveFile.FullName; } }
        public FileAttributes Attributes { get; private set; }
        public DateTime CreationTime { get { return _archiveFile.CreationTime; } }
        public DateTime LastAccessTime { get { return _archiveFile.LastAccessTime; } }
        public DateTime LastWriteTime { get { return _archiveFile.LastWriteTime; } }

        public override string ToString()
        {
            return FullName;
        }
    }
}