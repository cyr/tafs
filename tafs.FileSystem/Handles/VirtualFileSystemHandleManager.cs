using System;
using System.Collections.Generic;
using System.IO;

namespace tafs.FileSystem.Handles
{
    public class VirtualFileSystemHandleManager
    {
        private readonly Dictionary<VirtualFileSystemHandleId, VirtualFileSystemHandle> _handles = new Dictionary<VirtualFileSystemHandleId, VirtualFileSystemHandle>();

        public VirtualFileSystemHandleId CreateHandle(IVirtualPath virtualPath, FileAccess access, FileShare share, FileMode mode)
        {
            var handleId = VirtualFileSystemHandleId.Create();
            Console.WriteLine("\tHandle created: {0}", handleId);

            if (virtualPath.IsDirectory)
                return CreateDirectoryHandle(virtualPath, handleId);

            return CreateFileHandle(virtualPath, access, share, mode, handleId);
        }

        public void CloseHandle(VirtualFileSystemHandleId handleId)
        {
            VirtualFileSystemHandle handle;
            if (_handles.TryGetValue(handleId, out handle))
            {
                handle.CloseHandle();
                _handles.Remove(handleId);
            }
        }

        public VirtualFileSystemHandle GetHandle(VirtualFileSystemHandleId handleId)
        {
            VirtualFileSystemHandle handle;

            _handles.TryGetValue(handleId, out handle);

            return handle;
        }

        private VirtualFileSystemHandleId CreateFileHandle(IVirtualPath virtualPath, FileAccess access, FileShare share, FileMode mode, VirtualFileSystemHandleId handleId)
        {
            if (!virtualPath.Exists)
                virtualPath = ConvertNonExistingPathToVirtualFilePath(virtualPath);

            if (LacksRequiredAcessToPath(virtualPath, access))
                return null;

            try
            {
                var virtualFile = virtualPath as IVirtualFile;

                if (virtualFile == null)
                    return null;

                _handles.Add(handleId, new VirtualFileSystemHandle(virtualFile, virtualFile.Open(mode, access, share)));

                return handleId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private VirtualFileSystemHandleId CreateDirectoryHandle(IVirtualPath virtualPath, VirtualFileSystemHandleId handleId)
        {
            _handles.Add(handleId, new VirtualFileSystemHandle(virtualPath));
            return handleId;
        }

        private static bool LacksRequiredAcessToPath(IVirtualPath virtualPath, FileAccess access)
        {
            return !(virtualPath is IWriteableFile) && access.HasFlag(FileAccess.Write);
        }

        private static IVirtualPath ConvertNonExistingPathToVirtualFilePath(IVirtualPath virtualPath)
        {
            return ((NonExistingPath)virtualPath).ToFile();
        }
    }
}