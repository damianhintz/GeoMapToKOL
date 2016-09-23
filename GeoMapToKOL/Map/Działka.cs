using System;
using OSGeo.OGR;

namespace GeoMapToKOL.Map
{
    public class Dzia³ka
    {
        public string nazwa;    //141805_2.0001.52/8
        public string teryt;    //141805_2
        public string obreb;    //0001
        public string numer;    //52/8
        public string nr1;      //52
        public string nr2;      //8
        public string typ;      //O - obrêb, D - dzia³ka

        //geometria dzia³ki
        public Geometry geometry;

        public Dzia³ka(string nazwa, string typ)
        {
            switch (typ)
            {
                case "O":
                    break;
                case "D":
                    break;
                default:
                    throw new Exception("Niepoprawny typ obiektu <" + typ + ">");
                    //break;
            }

            this.typ = typ;
            this.nazwa = nazwa;

            string[] cols = nazwa.Split('.');

            this.teryt = cols[0];

            if (cols.Length > 1)
            {
                if (typ == "O")
                {
                    if (cols.Length > 2)
                        Console.WriteLine("Identyfikator obrêbu ma niepoprawny format <" + nazwa + ">");

                    this.obreb = cols[1];
                    this.numer = "";
                    this.nr1 = "";
                    this.nr2 = "";
                }
                else
                if (typ == "D")
                {
                    if (cols.Length > 3)
                        Console.WriteLine("Identyfikator dzia³ki ma niepoprawny format <" + nazwa + ">");

                    this.obreb = cols[1];
                    this.numer = cols[2];
                    this.nr1 = this.teryt + "." + this.obreb + "." + numer;
                    this.nr2 = "";

                    cols = numer.Split('/');

                    switch (cols.Length)
                    {
                        case 1:
                            break;
                        case 2:
                            this.nr1 = this.teryt + "." + this.obreb + "." + cols[0];
                            this.nr2 = cols[1];
                            break;
                        default:
                            throw new Exception("Niepoprawny numer dzia³ki " + numer);
                            //break;
                    }
                }
            }
            else
                Console.WriteLine("Identyfikator obiektu ma niepoprawny format <" + nazwa + ">");
        }
    }
}