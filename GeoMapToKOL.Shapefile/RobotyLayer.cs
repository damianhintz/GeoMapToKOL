using System;
using OSGeo.OGR;

namespace GeoMapToKOL.Shapefile
{
    public class RobotyLayer : Layer
    {
        public RobotyLayer(string name, DataSource ds) : base(name, ds)
        {
            defineAtrybutTekstowy("KERG", 250);
            defineAtrybutTekstowy("TERYTGMINY", 32);
        }
    }
}