using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonSiegeRawPdnFileType {
    public static class DSRawFileUtil {
        public const uint RapiMagic = 0x52617069; // 'Rapi'

        public interface ILoadable {
            int MaxSurfaceCount { get; }
            void Initialize(int width, int height);
            void AddLayer();
            void SetPixelBgra(int x, int y, uint bgra);
        }

        public static bool Load(Stream input, ILoadable output) {
            BinaryReader stream = new BinaryReader(input);

            uint magic = stream.ReadUInt32();
            if (magic != RapiMagic) {
                return false;
            }

            uint format = stream.ReadUInt32();
            ushort flags = stream.ReadUInt16();
            ushort surfaceCount = stream.ReadUInt16();
            ushort width = stream.ReadUInt16();
            ushort height = stream.ReadUInt16();

            output.Initialize(width, height);

            for (int i = 0; i != output.MaxSurfaceCount && i < surfaceCount; ++i) {
                output.AddLayer();

                for (int y = height - 1; y >= 0; --y) {
                    for (int x = 0; x < width; ++x) {
                        output.SetPixelBgra(x, y, stream.ReadUInt32());
                    }
                }

                width >>= 1;
                height >>= 1;
            }

            return true;
        }
    }
}
