using PaintDotNet;

namespace DungeonSiegeRawPdnFileType {
    public sealed class DSRawFileTypeFactory : IFileTypeFactory {
        public FileType[] GetFileTypeInstances() {
            return new FileType[] { new DSRawFileType() };
        }
    }
}
