using Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeographyNetCore
{

    public class GeoPolygon
    {
        public List<GeoPoint> Points {get; private set; }
        public List<GeoLineSegment> Segments { get; private set; }

        public GeoPolygon(GeoPoint[] points)
        {
            Points = new List<GeoPoint>(points);
            for(UInt32 index = 0; index < Points.Count - 1; ++index)
            {
                Segments.Add(new GeoLineSegment(points[index], points[index + 1]));
            }
        }

        public Polygon2 toPolygon2()
        {
            List<Point2> point2List = new List<Point2>();
            foreach(var geoPoint in Points)
            {
                point2List.Add(geoPoint.toPoint2());
            }
            return new Polygon2(point2List.ToArray());
        }

        public bool containsPoint(GeoPoint another)
        {
            return toPolygon2().Contains(another.toPoint2().X, another.toPoint2().Y);
        }

        public List<GeoPoint> getIntersectionPoints(GeoPolygon another)
        {
            List<GeoPoint> result = new List<GeoPoint>();
            foreach(var selfSegment in Segments)
            {
                foreach(var anotherSegment in another.Segments)
                {
                    var resPoint = selfSegment.getIntersectionPoint(anotherSegment);
                    if(resPoint != null)
                    {
                        result.Add(resPoint);
                    }
                }
            }
            return result;
        }

        private Int32 amountOfPointsInPolygon(List<GeoPoint> points)
        {
            return points.FindAll(x => x.isInPolygon(this)).Count;
        }

        public bool intersectsWith(GeoPolygon another)
        {
            var pointsInAnother = Points.FindAll(x => x.isInPolygon(another)).Count;
            return pointsInAnother > 0 && pointsInAnother < Points.Count;
        }

        public bool contains(GeoPolygon another)
        {
            return Points.FindAll(x => x.isInPolygon(another)).Count == Points.Count;
        }
    }

}
