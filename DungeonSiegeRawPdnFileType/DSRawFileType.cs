using PaintDotNet;
using PaintDotNet.PropertySystem;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static DungeonSiegeRawPdnFileType.DSRawFileUtil;

namespace DungeonSiegeRawPdnFileType {
    public sealed class DSRawFileType : PropertyBasedFileType {
        private enum Format : uint {
            _8888 = 0x38383838,
        }

        private class DocumentWrapper : ILoadable {
            private Document document;
            private Surface surface;

            int ILoadable.MaxSurfaceCount => -1;

            void ILoadable.Initialize(int width, int height) {
                document = new Document(width, height);
            }

            void ILoadable.AddLayer() {
                BitmapLayer layer = Layer.CreateBackgroundLayer(document.Width, document.Height);
                surface = layer.Surface;
                document.Layers.Add(layer);
            }

            void ILoadable.SetPixelBgra(int x, int y, uint bgra) {
                surface.GetPointReference(x, y).Bgra = bgra;
            }

            public Document GetDocument() {
                return document;
            }
        }

        
        public readonly static string[] FileExtensions = { ".raw" };

        public DSRawFileType() : base(
            "Dungeon Siege Raw Texture (RAW)",
            FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving,
            FileExtensions) {
        }

#if false
        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        static extern int GetPrivateProfileString(string appName, string keyName, string defaultValue, StringBuilder valueOut, int size, string fileName);
        static DSRawFileType() {
            string fileName = Assembly.GetExecutingAssembly().GetName().Name + ".ini";
            StringBuilder value = new StringBuilder(256);
            int error = GetPrivateProfileString("FileExtensions", "sAlternativeFileExtensions", ".raw", value, 256, fileName);
            if (error != 0) {
                Console.WriteLine("Error: {0}", error);
            }
            FileExtensions = value.ToString().Split(',');
        }
#endif

        protected override Document OnLoad(Stream input) {
            DocumentWrapper output = new DocumentWrapper();
            return DSRawFileUtil.Load(input, output) ? output.GetDocument() : null;
        }

        public override PropertyCollection OnCreateSavePropertyCollection() {
            return PropertyCollection.CreateEmpty();
        }

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback) {
            BinaryWriter stream = new BinaryWriter(output);

            // Magic
            stream.Write(RapiMagic);
            // Format
            stream.Write((uint)Format._8888);
            // Flags
            stream.Write((ushort)0);
            // SurfaceCount
            stream.Write((ushort)1);
            // Width
            stream.Write((ushort)input.Width);
            // Height
            stream.Write((ushort)input.Height);

            Surface flatSurface = new Surface(input.Width, input.Height);
            input.Flatten(flatSurface);

            for (int y = input.Height - 1; y >= 0; --y) {
                for (int x = 0; x < input.Width; ++x) {
                    stream.Write(flatSurface.GetPointReference(x, y).Bgra);
                }
            }
        }
    }
}
