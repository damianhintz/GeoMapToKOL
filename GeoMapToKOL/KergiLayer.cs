using System;
using OSGeo.OGR;

public class KergiLayer : Layer
{
    public KergiLayer(string name, DataSource ds) : base(name, ds)
    {
        this.defineAtrybutTekstowy("TERYT", 32);
        this.defineAtrybutTekstowy("KERG", 250);
        this.defineAtrybutTekstowy("RODZAJ", 1);
        this.defineAtrybutTekstowy("OBIEKT", 64);
        this.defineAtrybutTekstowy("OPIS", 64);
    }
}
