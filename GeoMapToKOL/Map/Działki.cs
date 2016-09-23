using System;
using System.Collections.Generic;
using OSGeo.OGR;
using GeoMapToKOL.Shapefile;
using GeoMapToKOL.KERG;

namespace GeoMapToKOL.Map
{
    public class Dzia³ki
    {
        RobotyShapefile shp;
        Dictionary<string, Dzia³ka> dzialki = new Dictionary<string, Dzia³ka>();
        Dictionary<string, Podzia³Dzia³ki> podzialy = new Dictionary<string, Podzia³Dzia³ki>();

        string gminaName;
        Geometry gminaGeom;

        public Geometry Gmina
        {
            get
            {
                if (gminaGeom == null)
                {
                    foreach (KeyValuePair<string, Dzia³ka> kv in dzialki)
                    {
                        string numer = kv.Key;
                        Dzia³ka dzialka = kv.Value;
                        if (dzialka.typ == "O")
                        {
                            Geometry geom = dzialka.geometry;
                            if (gminaGeom == null)
                            {
                                gminaGeom = geom;
                                gminaName = dzialka.teryt;
                            }
                            else
                            {
                                gminaGeom = gminaGeom.Union(geom);
                                if (gminaName != dzialka.teryt)
                                    throw new Exception("Teryt obrêbu uleg³ zmianie " + gminaName + " -> " + dzialka.teryt);
                            }
                        }
                    }
                    return this.gminaGeom;
                }
                else
                {
                    return this.gminaGeom;
                }
            }
        }

        public Dzia³ki(RobotyShapefile shp)
        {
            this.shp = shp;
            this.gminaGeom = null;
        }

        public bool analizujRoboty(Dictionary<string, ObiektyKerg> roboty)
        {
            int nBezZakres = 0, nZakres = 0;
            string teryt = "";
            string obreb = "";

            foreach (KeyValuePair<string, ObiektyKerg> kv in roboty)
            {
                ObiektyKerg mapKerg = kv.Value;
                string kerg = mapKerg.kerg;

                Geometry union = null;

                foreach (string o in mapKerg.obiekty)
                {
                    string obiekt = o;

                    if (kerg == "332/92")
                        Console.Error.WriteLine(obiekt);

                    Geometry geom = null;

                    if (obiekt.Contains("."))
                    {
                        string[] cols = obiekt.Split('.');
                        teryt = cols[0];
                        obreb = cols[1];
                    }

                    if (obiekt[obiekt.Length - 1] == '.')
                    {
                        string obiektN = obiekt.Substring(0, obiekt.Length - 1);
                        Console.WriteLine(obiekt + " -> " + obiektN);
                        obiekt = obiektN;
                    }

                    if (dzialki.ContainsKey(obiekt))
                    {
                        //dla tego obiektu znaleziono zakres roboty
                        Dzia³ka dzialka = dzialki[obiekt];
                        geom = dzialka.geometry;
                    }
                    else
                    {
                        //dla tego obiektu nie znaleziono zakresu roboty, jezeli to dzialka to mogla zostac podzielona
                        string nr1 = obiekt;
                        bool nr2 = false;

                        if (obiekt.Contains("/"))
                        {
                            string[] cols = obiekt.Split('/');
                            nr1 = cols[0];
                            nr2 = true;
                        }

                        if (podzialy.ContainsKey(obiekt))
                        {
                            //dla tego obiektu znaleziono podzia³, trzeba bedzie scalic geometrie wszystkich dzialek w podziale aby dostac zakres roboty
                            Podzia³Dzia³ki podzial = podzialy[obiekt];
                            geom = podzial.scalGeometrie(obiekt, false);
                        }
                        else
                        {
                            if (nr2 && podzialy.ContainsKey(nr1))
                            {
                                //dla tego obiektu znaleziono podzia³, trzeba bedzie scalic geometrie wszystkich dzialek w podziale aby dostac zakres roboty
                                Podzia³Dzia³ki podzial = podzialy[nr1];
                                geom = podzial.scalGeometrie(obiekt, false);
                            }
                        }
                    }

                    if (geom != null)
                    {
                        if (union == null)
                            union = geom;
                        else
                        {
                            union = union.Union(geom);
                        }
                    }
                }

                if (union == null)
                {
                    //dla tego obiektu brak zakresu roboty, mozemy dac zakres calego obrebu
                    if (teryt.Length == 8 && obreb.Length == 4)
                    {
                        string numer = teryt + "." + obreb;
                        if (dzialki.ContainsKey(numer))
                        {
                            //dla tego obiektu znaleziono zakres roboty obrebu
                            Dzia³ka dzialka = dzialki[numer];
                            union = dzialka.geometry;
                        }
                    }
                }

                if (union != null)
                {
                    shp.dodajRobote(union, mapKerg);
                    nZakres++;
                }
                else
                {


                    shp.dodajKerg(mapKerg, "robota bez zakresu (" + mapKerg.obiekty.Count + " obiekty)");
                    nBezZakres++;
                }
            }

            Console.WriteLine(" Roboty z zakresem {0}/{1}", nZakres, roboty.Count);
            Console.WriteLine(" Roboty bez zakresu {0}", nBezZakres);

            return true;
        }

        public bool dodajDzialke(Dzia³ka dzialka)
        {
            //wszystkie dzialki w pliku map
            if (!dzialki.ContainsKey(dzialka.nazwa))
                dzialki.Add(dzialka.nazwa, dzialka);
            else
                Console.WriteLine("Powtórzony numer dzia³ki " + dzialka.nazwa);

            //tylko dzialki po podziale
            if (dzialka.nr2 == "")
                return true;

            //inicjowanie nowego podzialu dzialki
            if (!podzialy.ContainsKey(dzialka.nr1))
            {
                Podzia³Dzia³ki podzial = new Podzia³Dzia³ki();
                podzialy.Add(dzialka.nr1, podzial);
            }

            //grupowanie dzialek z podzialu np. 141805_2.0001.52/1 i 141805_2.0001.52/2
            if (!podzialy[dzialka.nr1].dodajDzialke(dzialka))
                return false;

            return true;
        }

        public void opis()
        {
            int nPodzialy = 0, nDzialki = 0;
            int nPodzialy0 = 0, nDzialki0 = 0;
            int nPodzialy1 = 0, nDzialki1 = 0;
            int nWszystkie = 0;

            foreach (KeyValuePair<string, Podzia³Dzia³ki> kvp in podzialy)
            {
                string nr1 = kvp.Key;
                Podzia³Dzia³ki podzial = kvp.Value;

                if (podzial.Count > 1)
                {
                    nPodzialy1++;
                    nDzialki1 += podzial.Count;
                }
                else
                if (podzial.Count > 0)
                {
                    nPodzialy0++;
                    nDzialki0 += podzial.Count;
                    //Console.WriteLine (nr1);
                }
                else
                {
                    nPodzialy++;
                    nDzialki++;
                }

                nWszystkie += podzial.Count;
            }

            Console.WriteLine(" Obiekty *5216 i *5214 w pliku map {0} = ({1} + {2})", dzialki.Count, nWszystkie, dzialki.Count - nWszystkie);
            Console.WriteLine(" Dzia³ki niepodzielone {0}", dzialki.Count - nWszystkie);
            Console.WriteLine(" Wszystkie dzia³ki w podzia³ach {0}", nWszystkie);
            Console.WriteLine("");
            Console.WriteLine(" Podzia³y dzia³ek {0} = ({1} + {2})", podzialy.Count, nPodzialy1, nPodzialy0);
            Console.WriteLine(" Podzia³y z wiêcej ni¿ jedn¹ dzia³k¹ {0} ({1} dzia³ki)", nPodzialy1, nDzialki1);
            Console.WriteLine(" Podzia³y z jedn¹ dzia³k¹ {0} ({1} dzia³ki)", nPodzialy0, nDzialki0);
            Console.WriteLine(" Podzia³y bez dzia³ki {0} ({1} dzia³ki)", nPodzialy, nDzialki);

        }
    }
}