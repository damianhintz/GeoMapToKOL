using System;
using OSGeo.OGR;

namespace GeoMapToKOL.Shapefile
{
    public class RobotyShapefile
    {
        string name = "ROBOTYGEOD";
        Driver drv = null;
        DataSource ds = null;

        Dzia³kiLayer wDzialki;
        RobotyLayer wRoboty;
        KergiLayer wKerg;

        public RobotyShapefile(string name)
        {
            this.name = name;
            Console.WriteLine("Tworzenie katalogu danych ROBOTYGEOD");
            this.drv = Ogr.GetDriverByName("ESRI Shapefile");

            if (this.drv == null) throw new Exception("-FAILED-");

            this.ds = drv.CreateDataSource(name, new string[] { });

            if (this.ds == null) throw new Exception("-FAILED-");

            this.wRoboty = new RobotyLayer("ROBOTY", this.ds);
            this.wDzialki = new Dzia³kiLayer("DZIALKI", this.ds);
            this.wKerg = new KergiLayer("KERG", this.ds);
        }

        public bool DodajRobotê(Geometry geom, string kerg, string kergTeryt)
        {
            return wRoboty.dodajObiekt(geom, kerg, kergTeryt);
        }

        public bool DodajKERG(string kerg, string teryt, string rodzaj, string obiekt, string opis)
        {
            return wKerg.dodajObiekt(null, teryt, kerg, rodzaj, obiekt, opis);
        }

        public bool DodajDzia³kê(Geometry geometry, string nazwa)
        {
            return wDzialki.dodajObiekt(geometry, nazwa);
        }
    }
}