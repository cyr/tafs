namespace tafs.FileSystem
{
    public interface IWriteableDirectory : IVirtualDirectory
    {
        void Create();
        void Delete();
    }
}