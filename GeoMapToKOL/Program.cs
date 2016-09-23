using System;
using GeoMapToKOL.KERG;
using GeoMapToKOL.Shapefile;
using GeoMapToKOL.Map;

namespace GeoMapToKOL
{
    class Program
    {
        static KergReader _krgReader = null;
        static RobotyShapefile _roboty = null;
        static GeoMapReader _mapReader = null;

        public static void Main(string[] args)
        {
            GdalConfiguration.ConfigureOgr();
            GdalConfiguration.ConfigureGdal();
            if (args.Length < 3)
            {
                Console.WriteLine("GeoMapToKOL.exe <GeoMap file> <KERG file> <Shapefile folder>");
                Environment.Exit(-1);
            }
            _roboty = new RobotyShapefile(args[2]);
            _mapReader = new GeoMapReader(_roboty);
            _mapReader.Wczytaj(fileName: args[0]);
            _krgReader = new KergReader(args[1]);
            _mapReader.PrzygotujKergi(_krgReader);
            Console.WriteLine("Koniec.");
            Console.Read();
        }
    }
}
