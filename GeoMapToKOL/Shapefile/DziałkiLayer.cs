using System;
using OSGeo.OGR;

namespace GeoMapToKOL.Shapefile
{
    public class Dzia�kiLayer : Layer
    {
        public Dzia�kiLayer(string name, DataSource ds) : base(name, ds)
        {
            defineAtrybutTekstowy("DZIALKA", 64);
        }
    }
}