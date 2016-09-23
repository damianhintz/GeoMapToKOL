using System;
using OSGeo.OGR;
using OSGeo.OSR;

public class Layer
{
    protected string name = "unnamed";
    protected DataSource ds = null;
    public OSGeo.OGR.Layer layer = null;

    //2000 21
    protected static string proj21 = "+proj=tmerc +lat_0=0 +lon_0=21 +k=0.999923 +x_0=7500000 +y_0=0 +ellps=GRS80 +units=m +no_defs";
    protected static SpatialReference srs21 = new SpatialReference("");

    public Layer(string name, DataSource ds)
    {
        this.ds = ds;
        this.name = name;
        Console.WriteLine(" Zak³adanie warstwy " + name);

        srs21.ImportFromProj4(proj21);

        layer = ds.CreateLayer(name, srs21, wkbGeometryType.wkbPolygon, new string[] { });

        if (layer == null)
            throw new Exception("-FAILED-");
    }

    public virtual bool dodajObiekt(Geometry geom, params string[] fields)
    {
        Feature feature = new Feature(this.layer.GetLayerDefn());
        int i = 0;

        foreach (string field in fields)
            feature.SetField(i++, field);

        if (geom != null)
        {
            if (feature.SetGeometry(geom) != 0)
                return false;
        }

        return this.layer.CreateFeature(feature) == 0;
    }

    public void defineAtrybutTekstowy(string fieldName, int fieldSize)
    {
        FieldDefn fdefn = new FieldDefn(fieldName, FieldType.OFTString);
        fdefn.SetWidth(fieldSize);

        Console.WriteLine("  Dodawanie atrybutu " + fieldName);

        if (this.layer.CreateField(fdefn, 1) != 0)
            throw new Exception("-FAILED-");
    }

    public void opiszWarstwe()
    {
        FeatureDefn def = this.layer.GetLayerDefn();
        Console.WriteLine("Nazwa warstwy: " + def.GetName());
        Console.WriteLine("Liczba obiektów: " + this.layer.GetFeatureCount(1));

        Envelope ext = new Envelope();
        layer.GetExtent(ext, 1);
        Console.WriteLine("Zasiêg: " + ext.MinX + "," + ext.MaxX + "," + ext.MinY + "," + ext.MaxY);

        SpatialReference sr = this.layer.GetSpatialRef();
        string srs_wkt = "(unknown)";
        if (sr != null)
            sr.ExportToPrettyWkt(out srs_wkt, 1);
        Console.WriteLine("Uk³ad przestrzenny warstwy: " + srs_wkt);

        Console.WriteLine("Definicje atrybutów:");
        for (int iAttr = 0; iAttr < def.GetFieldCount(); iAttr++)
        {
            FieldDefn fdef = def.GetFieldDefn(iAttr);
            Console.WriteLine(fdef.GetNameRef() + ": " + fdef.GetFieldTypeName(fdef.GetFieldType()) + " (" + fdef.GetWidth() + "." + fdef.GetPrecision() + ")");
        }
    }
}