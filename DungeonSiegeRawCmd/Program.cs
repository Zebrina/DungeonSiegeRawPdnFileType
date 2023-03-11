using DungeonSiegeRawPdnFileType;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DungeonSiegeRawPdnFileType.DSRawFileUtil;

namespace DungeonSiegeRawCmd {
    class BitmapWrapper : ILoadable {
        private Bitmap bitmap;

        int ILoadable.MaxSurfaceCount => 1;

        void ILoadable.Initialize(int width, int height) {
            bitmap = new Bitmap(width, height);
        }

        void ILoadable.AddLayer() {
        }

        void ILoadable.SetPixelBgra(int x, int y, uint bgra) {
            /*
            uint argb = ((bgra & 0x000000FF) << 24) |
                        ((bgra & 0x0000FF00) << 8) |
                        ((bgra & 0x00FF0000) >> 8) |
                        ((bgra & 0xFF000000) >> 24);
                        */
            bitmap.SetPixel(x, y, Color.FromArgb((int)bgra));
        }

        public Bitmap GetBitmap() {
            return bitmap;
        }
    }

    static class Program {
        static void Main(string[] args) {
            TraverseDirectory(args.Length == 0 ? Directory.GetCurrentDirectory() : args[0], true);
        }

        static void TraverseDirectory(string directory, bool recursive) {
            foreach (string file in Directory.GetFiles(directory)) {
                if (Path.GetExtension(file).Equals(".raw", StringComparison.OrdinalIgnoreCase)) {
                    ConvertFile(file);
                }
            }
            if (recursive) {
                foreach (string subDirectory in Directory.GetDirectories(directory)) {
                    TraverseDirectory(subDirectory, recursive);
                }
            }
        }

        static void ConvertFile(string file) {
            Console.Write("Converting '{0}' to png... ", file);
            Bitmap bitmap = null;
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                BitmapWrapper output = new BitmapWrapper();
                Load(fs, output);
                bitmap = output.GetBitmap();
            }
            if (bitmap != null) {
                bitmap.Save(file.Replace(".raw", ".png"), ImageFormat.Png);
                Console.WriteLine("DONE");
            } else {
                Console.WriteLine("FAILED!");
            }
        }
    }
}
