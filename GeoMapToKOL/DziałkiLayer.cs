using System;
using OSGeo.OGR;

public class Dzia³kiLayer : Layer
{
    public Dzia³kiLayer(string name, DataSource ds) : base(name, ds)
    {
        this.defineAtrybutTekstowy("DZIALKA", 64);
    }
}
