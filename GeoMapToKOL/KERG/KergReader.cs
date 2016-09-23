using System;
using System.IO;
using System.Collections.Generic;

namespace GeoMapToKOL.KERG
{
    public class KergReader
    {
        public Dictionary<string, ObiektyKerg> Roboty = new Dictionary<string, ObiektyKerg>();

        public KergReader(string fileName)
        {
            ObiektyKerg mapKerg = null;
            string teryt, kerg, rodzaj, obiekt;
            int nBledy = 0, nKergPusty = 0, nObiektPusty = 0, nObiekty = 0;
            int nRodzajPusty = 0;

            Console.WriteLine("Trwa wczytywanie danych KERG...");

            if (string.IsNullOrEmpty(fileName))
                throw new Exception("-FAILED-");

            StreamReader reader = new StreamReader(fileName);
            string wiersz = "";

            while ((wiersz = reader.ReadLine()) != null)
            {
                wiersz = wiersz.Trim();

                if (string.IsNullOrEmpty(wiersz))
                    continue;

                string[] cols = wiersz.Split('\t');

                if (cols.Length < 4)
                {
                    Console.WriteLine("Pominiêto wiersz bez 4 kolumn.");
                    nBledy++;
                    continue;
                }

                teryt = cols[0].Trim();
                kerg = cols[1].Trim();
                rodzaj = cols[2];
                obiekt = cols[3].Trim();

                if (kerg == "")
                {
                    nKergPusty++;
                    continue;
                }

                if (obiekt == "")
                {
                    nObiektPusty++;
                    continue;
                }

                if (rodzaj == "")
                {
                    nRodzajPusty++;
                    continue;
                }

                mapKerg = new ObiektyKerg(teryt, kerg, rodzaj);

                string klucz = mapKerg.Klucz;
                if (!Roboty.ContainsKey(klucz))
                {
                    Roboty.Add(klucz, mapKerg);
                }
                else
                    mapKerg = Roboty[klucz];

                mapKerg.dodajObiekt(obiekt);
                nObiekty++;
            }

            reader.Close();

            if (nBledy > 0)
                Console.WriteLine("Obiekty pominiête powtórzone " + nBledy);

            if (nKergPusty > 0)
                Console.WriteLine("Obiekty pominiête bez KERG " + nKergPusty);

            if (nObiektPusty > 0)
                Console.WriteLine("Obiekty pominiête bez ID " + nObiektPusty);

            if (nRodzajPusty > 0)
                Console.WriteLine("Obiekty pominiête bez RODZAJ " + nRodzajPusty);

            Console.WriteLine("Obiekty wczytane {0}/{1}", Roboty.Count, nObiekty);
        }
    }
}