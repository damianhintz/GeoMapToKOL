using System;
using System.IO;
using System.Text;
using OSGeo.OGR;
using GeoMapToKOL.Shapefile;
using GeoMapToKOL.KERG;

namespace GeoMapToKOL.Map
{
    public class GeoMapReader
    {
        int _inneCount = 0;
        RobotyShapefile _roboty;
        Dzia³ka _dzia³ka = null;
        Dzia³ki _dzia³ki = null;

        public GeoMapReader(RobotyShapefile roboty)
        {
            _roboty = roboty;
            _dzia³ki = new Dzia³ki(roboty);
        }

        public void Wczytaj(string fileName)
        {
            int nPunkty = 0;
            string wiersz = "";
            string a2 = "";
            string a3 = "";
            string nr = "";
            string wkt = "";
            string typ = "";

            Console.WriteLine("Trwa wczytywanie pliku GeoMap...");
            var reader = new StreamReader(fileName, Encoding.GetEncoding(1250));
            while (null != (wiersz = reader.ReadLine()))
            {
                if (wiersz.Length == 0)
                {
                    Console.WriteLine("-KONIEC-");
                    continue;
                }

                //punkty oparcia obiektu
                //P status(widocznoœæ) x y h(opcjonalna) numer (opcjonalny, je¿eli jest to zaczyna siê od ,)
                //P 1   -31620.780    12477.180 ,O.141801_5.288

                char znak1 = wiersz[0];

                switch (znak1)
                {
                    case ';': //komentarz
                              //;Operator: PSobczak
                        break;
                    case '*': //rekord obiektu
                        {
                            //*1246   0  21 0.00000000  #
                            if (nPunkty > 0)
                                DodajGeometriê(wkt);

                            nPunkty = 0;
                            a2 = "";
                            a3 = "";
                            wkt = "";
                            typ = "";
                            //kod obiektu jest czterocyfrowy
                            nr = wiersz.Substring(1, 4);

                            switch (nr)
                            {
                                case "5216": //dzia³ka
                                    typ = "D";
                                    break;
                                case "5214": //obrêb
                                    typ = "O";
                                    break;
                                default:
                                    _inneCount++;
                                    break;
                            }
                        }
                        break;
                    case ':': //atrybut
                              //:A2[141805_2.0001.52/8]
                        if (wiersz.StartsWith(":A2["))
                        {
                            a2 = wiersz.Substring(4);
                            a2 = a2.Replace(".AR_1.", ".");
                            a2 = a2.Replace(".AR_2.", ".");
                            a2 = a2.Replace(".AR_3.", ".");
                            a2 = a2.Replace(".AR_4.", ".");
                            //pozbywamy siê jeszcze nawiasu na koncu
                            a2 = a2.Substring(0, a2.Length - 1);

                            if (typ != "")
                            {
                                _dzia³ka = new Dzia³ka(a2, typ);
                            }

                        }
                        else
                        if (wiersz.StartsWith(":A3["))
                        {
                            a3 = wiersz.Substring(4);
                            //pozbywamy siê jeszcze nawiasu na koncu
                            a3 = a3.Substring(0, a3.Length - 1);
                            /*
                            if (typ != "")
                            {
                                if (typ == "D")
                                    dzialka = new MapDzialka (a3 + "." + a2, typ);
                                else
                                    dzialka = new MapDzialka (a2, typ);
                            }
                            */
                        }
                        break;
                    case 'L': //etykieta
                              //L 1 0.375 0.375 0.0000000 1.0000000 7
                        break;
                    case 'P': //punkt oparcia obiektu
                        {
                            //P 1   -32998.960    10739.320 ,K.141801_5
                            if (typ != "")
                            {
                                string[] cols = wiersz.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                string status = cols[1];
                                //double x = double.Parse (cols[2]);
                                //double y = double.Parse (cols[3]);
                                string sy = cols[2];
                                string sx = cols[3];

                                //("POLYGON((5757349.870 7504603.332,5757314.580 7504610.632,5757344.200 7504575.912))");
                                if (wkt.Length == 0)
                                    wkt += sx + " " + sy;
                                else
                                    wkt += "," + sx + " " + sy;

                                nPunkty++;
                            }
                        }
                        break;
                }
            }

            if (nPunkty > 0) DodajGeometriê(wkt);
            if (_inneCount > 0) Console.WriteLine(" Obiekty inne {0}", _inneCount);

            _dzia³ki.opis();
            reader.Close();
        }

        bool DodajGeometriê(string wkt)
        {
            string polygon = "POLYGON((" + wkt + "))";
            Geometry geom = null;
            try
            {
                geom = Geometry.CreateFromWkt(polygon);
            }
            catch (Exception ex)
            {
                Console.WriteLine("NOT POLYGON: " + _dzia³ka.nazwa);
                throw ex;
            }

            if (geom == null)
                return false;

            try
            {
                geom.CloseRings();
                if (!geom.IsValid())
                {
                    Console.WriteLine("NOT VALID: " + _dzia³ka.nazwa);
                    geom = geom.Buffer(0.0, 30);
                    //geom = geom.GetBoundary ();
                    geom = geom.ConvexHull();
                    geom.CloseRings();
                    //geom = geom.Simplify (1.0);
                    //geom = Ogr.forceToPolygon (geom);
                    //geom = Ogr.BuildPolygonFromEdges (geom, 1, 1, 0.1);

                    if (geom.IsRing() == false)
                    {
                        if (_dzia³ka.typ != "O")
                        {
                            //geom = null;
                            Envelope env = new Envelope();
                            geom.GetEnvelope(env);
                            //geom = new Geometry (wkbGeometryType.wkbPolygon);
                            //geom.AddPoint_2D (env.MinX, env.MinY);
                            //geom.AddPoint_2D (env.MinX, env.MaxY);
                            //geom.AddPoint_2D (env.MaxX, env.MaxY);
                            //geom.AddPoint_2D (env.MaxX, env.MinY);
                            //geom.AddPoint_2D (env.MinX, env.MinY);

                            wkt = "" +
                                env.MinX + " " + env.MinY + "," +
                                env.MinX + " " + env.MaxY + "," +
                                env.MaxX + " " + env.MaxY + "," +
                                env.MaxX + " " + env.MinY + "," +
                                env.MinX + " " + env.MinY;

                            polygon = "POLYGON((" + wkt + "))";
                            geom = Geometry.CreateFromWkt(polygon);
                            //("POLYGON((5757349.870 7504603.332,5757314.580 7504610.632,5757344.200 7504575.912))");
                            //geom.ExportToWkt (out wktenv);
                            //Console.Error.WriteLine (polygon);
                            //geom.CloseRings ();
                        }
                    }
                }

                if (geom == null)
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR VALID: " + _dzia³ka.nazwa);
                throw ex;
            }

            _dzia³ka.geometry = geom;

            try
            {
                _roboty.DodajDzia³kê(_dzia³ka.geometry, _dzia³ka.nazwa);
            }
            catch (Exception ex)
            {
                Console.WriteLine("dodajDzialke: " + _dzia³ka.nazwa);
                throw ex;
            }

            return _dzia³ki.dodajDzialke(_dzia³ka);
        }

        public bool PrzygotujKergi(KergReader kergi)
        {
            return _dzia³ki.analizujRoboty(kergi.Roboty);
        }
    }
}