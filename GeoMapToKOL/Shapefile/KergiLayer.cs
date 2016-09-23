using System;
using OSGeo.OGR;

namespace GeoMapToKOL.Shapefile
{
    public class KergiLayer : Layer
    {
        public KergiLayer(string name, DataSource ds) : base(name, ds)
        {
            defineAtrybutTekstowy("TERYT", 32);
            defineAtrybutTekstowy("KERG", 250);
            defineAtrybutTekstowy("RODZAJ", 1);
            defineAtrybutTekstowy("OBIEKT", 64);
            defineAtrybutTekstowy("OPIS", 64);
        }
    }
}