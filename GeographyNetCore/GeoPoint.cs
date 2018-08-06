using CoordinateSharp;
using Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeographyNetCore
{
    public class GeoPoint
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string LatZone { get; private set; }
        public int LongZone { get; private set; }
        public double Easting { get; private set; }
        public double Northing { get; private set; }


        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            var utm = new Coordinate(latitude, longitude).UTM;
            LatZone = utm.LatZone;
            LongZone = utm.LongZone;
            Easting = utm.Easting;
            Northing = utm.Northing;
        }

        public Point2 toPoint2()
        {
            return new Point2(Easting, Northing);
        }

        public GeoPoint(string latZone, int longZone, double easting, double northing)
        {
            LatZone = latZone;
            LongZone = longZone;
            Easting = easting;
            Northing = northing;
            EagerLoad test = new EagerLoad();
            UniversalTransverseMercator utm = new UniversalTransverseMercator(latZone, longZone, easting, northing);
            Coordinate c = UniversalTransverseMercator.ConvertUTMtoLatLong(utm);
            Latitude = c.Latitude.ToDouble();
            Longitude = c.Longitude.ToDouble();
        }

        public bool isInPolygon(GeoPolygon polygon)
        {
            return polygon.containsPoint(this);
        }
    }
}
