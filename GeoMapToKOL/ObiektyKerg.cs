using System;
using System.IO;
using System.Collections.Generic;

public class ObiektyKerg
{
    public string Klucz { get { return teryt + " " + kerg + " " + rodzaj; } }
    public string teryt = "";
    public string kerg = "";
    public string rodzaj = "";
    
    public List<string> obiekty = new List<string>();

    public ObiektyKerg(string teryt, string kerg, string rodzaj)
    {
        this.teryt = teryt;
        this.kerg = kerg;
        this.rodzaj = rodzaj;
    }

    public bool dodajObiekt(string obiekt)
    {
        obiekty.Add(obiekt);
        return true;
    }
}
