using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tafs.FileSystem.Handles
{
    public class VirtualFileSystemHandleId
    {
        private readonly Guid _guid;

        private VirtualFileSystemHandleId(Guid guid)
        {
            _guid = guid;
        }

        public override bool Equals(object obj)
        {
            var otherHandleId = obj as VirtualFileSystemHandleId;

            return otherHandleId != null && otherHandleId.Guid == _guid;
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        public override string ToString()
        {
            return _guid.ToString();
        }

        internal Guid Guid
        {
            get { return _guid; }
        }

        public static VirtualFileSystemHandleId Create()
        {
            return new VirtualFileSystemHandleId(Guid.NewGuid());
        }
    }
}
