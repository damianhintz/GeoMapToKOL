using System;
using OSGeo.OGR;
//using OSGeo.OSR;

public class RobotyShapefile
{
	private string name = "ROBOTYGEOD";
	private Driver drv = null;
	private DataSource ds = null;
	
	private Dzia�kiLayer wDzialki;
	private RobotyLayer wRoboty;
	private KergiLayer wKerg;
	
	public RobotyShapefile (string name)
	{
		this.name = name;
		
		Console.WriteLine ("Tworzenie katalogu danych ROBOTYGEOD");
		
		//Ogr.RegisterAll ();
		this.drv = Ogr.GetDriverByName ("ESRI Shapefile");
		
		if (this.drv == null) 
			throw new Exception ("-FAILED-");
		
		this.ds = drv.CreateDataSource (name, new string[] {});
		
		if (this.ds == null)
		    throw new Exception ("-FAILED-");
		
		this.wRoboty = new RobotyLayer ("ROBOTY", this.ds);
		this.wDzialki = new Dzia�kiLayer ("DZIALKI", this.ds);
		this.wKerg = new KergiLayer ("KERG", this.ds);
	}
	
	public bool dodajRobote (Geometry geom, ObiektyKerg kerg)
	{
		return wRoboty.dodajObiekt (geom, kerg.kerg, kerg.teryt);
	}
	
	public bool dodajKerg (ObiektyKerg kerg, string opis)
	{
		return wKerg.dodajObiekt (null, kerg.teryt, kerg.kerg, kerg.rodzaj, kerg.obiekty[0], opis);
	}
	
	public bool dodajDzialke (Dzia�ka dzialka)
	{
		return wDzialki.dodajObiekt (dzialka.geometry, dzialka.nazwa);
	}
}
