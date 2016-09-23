using System;
using System.Collections.Generic;
using OSGeo.OGR;

//141805_2.0001.52/1
//141805_2.0001.52/2
//kolekcja dzia³ek z tego samego podzia³u
public class Podzia³Dzia³ki
{
    private Dictionary<string, Dzia³ka> dzialki = new Dictionary<string, Dzia³ka>();
    private string nazwa = "";

    public int Count
    {
        get { return dzialki.Count; }
    }

    public string Nazwa(string nazwa)
    {
        return dzialki[nazwa].nazwa;
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

        foreach (KeyValuePair<string, Dzia³ka> kv in dzialki)
        {
            string nr2_ = kv.Key;
            Dzia³ka dzialka = kv.Value;
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
                //Console.WriteLine ("scalanie podzia³u " + this.nazwa + " dla " + numer);
                //Console.WriteLine (" *scalenie nie jest ci¹g³ym obszarem " + buffer.GetGeometryType ());
            }
        }

        return buffer;
    }

    public bool dodajDzialke(Dzia³ka dzialka)
    {
        this.nazwa = dzialka.nr1;

        //grupowanie dzialek z podzialu np. 141805_2.0001.52/1 i 141805_2.0001.52/2
        if (!dzialki.ContainsKey(dzialka.nr2))
            dzialki.Add(dzialka.nr2, dzialka);
        else
        {
            Console.WriteLine("Powtórzony numer podzialu + <" + dzialka.nr2 + ">");
            return false;
        }

        return true;
    }
}
