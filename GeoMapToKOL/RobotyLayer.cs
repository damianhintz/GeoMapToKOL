using System;
using OSGeo.OGR;

public class RobotyLayer : Layer
{
    public RobotyLayer(string name, DataSource ds) : base(name, ds)
    {
        this.defineAtrybutTekstowy("KERG", 250);
        this.defineAtrybutTekstowy("TERYTGMINY", 32);
    }
}
