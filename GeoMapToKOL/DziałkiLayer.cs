using System;
using OSGeo.OGR;

public class Dzia�kiLayer : Layer
{
    public Dzia�kiLayer(string name, DataSource ds) : base(name, ds)
    {
        this.defineAtrybutTekstowy("DZIALKA", 64);
    }
}
