using System;
using OSGeo.OGR;

namespace GeoMapToKOL.Shapefile
{
    public class Dzia³kiLayer : Layer
    {
        public Dzia³kiLayer(string name, DataSource ds) : base(name, ds)
        {
            defineAtrybutTekstowy("DZIALKA", 64);
        }
    }
}