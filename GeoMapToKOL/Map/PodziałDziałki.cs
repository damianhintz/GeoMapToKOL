using System;
using System.Collections.Generic;
using OSGeo.OGR;

namespace GeoMapToKOL.Map
{
    /// <summary>
    /// Kolekcja dzia�ek z tego samego podzia�u.
    /// </summary>
    public class Podzia�Dzia�ki
    {
        Dictionary<string, Dzia�ka> _dzia�ki = new Dictionary<string, Dzia�ka>();
        string _nazwa = "";

        public int Count
        {
            get { return _dzia�ki.Count; }
        }

        public string Nazwa(string nazwa)
        {
            return _dzia�ki[nazwa].nazwa;
        }

        public Geometry scalGeometrie(string numer, bool wyzsze)
        {
            Geometry union = null;

            string nr1 = numer;
            string nr2 = "";

            if (numer.Contains("/"))
            {
                string[] cols = numer.Split('/');
                nr1 = cols[0];
                nr2 = cols[1];
            }

            string numery = "";

            foreach (KeyValuePair<string, Dzia�ka> kv in _dzia�ki)
            {
                string nr2_ = kv.Key;
                Dzia�ka dzialka = kv.Value;
                bool scal = false;

                numery = numery + "," + nr2_;

                if (wyzsze)
                {
                    if (nr2 != "")
                    {
                        int n2_ = int.Parse(nr2_);
                        int n2 = int.Parse(nr2);
                        if (n2_ > n2)
                            scal = true;
                    }
                    else
                        scal = true;
                }
                else
                {
                    scal = true;
                }

                if (!scal)
                    continue;

                if (union == null)
                    union = dzialka.geometry;
                else
                {
                    union = union.Union(dzialka.geometry);
                }
            }

            Geometry buffer = null;

            if (union == null)
            {
                Console.WriteLine("scalenie jest puste dla " + numer + " z " + numery);
            }
            else
            {
                //return union.ConvexHull ();
                buffer = union.Buffer(0.0, 30);

                if (buffer.GetGeometryType() != wkbGeometryType.wkbPolygon)
                {
                    //Console.WriteLine ("scalanie podzia�u " + this.nazwa + " dla " + numer);
                    //Console.WriteLine (" *scalenie nie jest ci�g�ym obszarem " + buffer.GetGeometryType ());
                }
            }

            return buffer;
        }

        public bool dodajDzialke(Dzia�ka dzialka)
        {
            this._nazwa = dzialka.nr1;

            //grupowanie dzialek z podzialu np. 141805_2.0001.52/1 i 141805_2.0001.52/2
            if (!_dzia�ki.ContainsKey(dzialka.nr2))
                _dzia�ki.Add(dzialka.nr2, dzialka);
            else
            {
                Console.WriteLine("Powt�rzony numer podzialu + <" + dzialka.nr2 + ">");
                return false;
            }

            return true;
        }
    }
}