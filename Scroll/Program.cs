using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CommandLine;
namespace Scroll
{
    internal class Program
    {
        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "Input file containing region vertices")]
            public string Input { get; set; } = "input.txt";
            [Option('o', "output", HelpText = "Output folder")]
            public string Output { get; set; } = "output";
            [Option('c', "combined", HelpText = "Generate map atlas")]
            public bool Atlas { get; set; } = false;
            [Option("min", HelpText = "Minimum scale level, Default = 3")]
            public int Min { get; set; } = 3;
            [Option("max", HelpText = "Maximum scale level, Default = 18")]
            public int Max { get; set; } = 18;
            [Option('l', "level", HelpText = "Certain scale level")]
            public int Level { get; set; } = -1;
            [Option('s', "scaler", HelpText = "Scaler for map tiles, 1=256x, 2=512x")]
            public int Scaler { get; set; } = 1;
        }
        
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunWithOptions(opts));
        }

        public static void RunWithOptions(Options opt)
        {
            if (opt.Scaler < 0 || opt.Scaler > 2)
            {
                Console.WriteLine("Scaler must be 1 or 2");
                return;
            }
            StreamReader input = new StreamReader(opt.Input);
            string vertex;
            var region = new Region();
            while ((vertex = input.ReadLine()) != null)
            {
                string[] coordinates = vertex.Split(',');
                double lng = Convert.ToDouble(coordinates[0]);
                double lat = Convert.ToDouble(coordinates[1]);
                region.AddVertex(new Coordinate {Latitude = lat, Longtitude = lng});
            }

            region.GetBoundingBox(out var coordMin, out var coordMax);

            if (opt.Level > 0)
                opt.Max = opt.Min = opt.Level;
            for (int z = opt.Min; z <= opt.Max; z++)
            {
                Console.WriteLine($"Proccessing level {z}");
                PointI tMin = CoordinateConvertor.ToTileIndex(coordMin, z, opt.Scaler);
                PointI tMax = CoordinateConvertor.ToTileIndex(coordMax, z, opt.Scaler);
                Bitmap[] images = null;
                int nTileX = tMax.x - tMin.x + 1, nTileY = tMax.y - tMin.y + 1;
                if (opt.Atlas)
                {
                    images = new Bitmap[nTileX*nTileY];
                }

                var saveFolder = Path.Combine(opt.Output, "level"+z.ToString());
                Directory.CreateDirectory(saveFolder);
                for (int iTileX = tMin.x; iTileX <= tMax.x; iTileX++)
                {
                    for (int iTileY = tMin.y; iTileY <= tMax.y; iTileY++)
                    {
                        var bytes = Downloader.DownloadTile(iTileX, iTileY, z, opt.Scaler);
                        var saveName = $"Tile@({iTileX-tMin.x},{iTileY-tMin.y}).png";
                        File.WriteAllBytes(Path.Combine(saveFolder, saveName), bytes);
                        if (opt.Atlas)
                        {
                            using (MemoryStream imageStream = new MemoryStream(bytes))
                            {
                                images[(iTileX-tMin.x)*nTileY+iTileY-tMin.y]= new Bitmap(imageStream);
                            }
                        }
                    }
                }
                if (opt.Atlas)
                {
                    int tileSize = 256 * opt.Scaler;
                    // merge images
                    var bitmap = new Bitmap(nTileX*tileSize, nTileY*tileSize);
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        for (int x = 0; x < nTileX; x++)
                        {
                            for (int y = 0; y < nTileY; y++)
                            {
                                // gdi+坐标中上方y为0, 与tile中相反
                                g.DrawImage(images[x*nTileY+y], x*tileSize, (nTileY-y-1)*tileSize);
                            }
                        }
                    }
                    bitmap.Save(Path.Combine(opt.Output,$"Level{z}.png"));
                }
            }
            
        }
    }
}