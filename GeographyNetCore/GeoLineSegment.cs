using Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeographyNetCore
{
    public class GeoLineSegment
    {
        public GeoPoint First {get; set; }
        public GeoPoint Second { get; set; }

        public GeoLineSegment(GeoPoint first, GeoPoint second)
        {
            First = first;
            Second = second;
        }

        public GeoLineSegment(double firstLatitude, double firstLongitude, double secondLatitude, double secondLongitude)
        {
            First = new GeoPoint(firstLatitude, firstLongitude);
            Second = new GeoPoint(secondLatitude, secondLongitude);
        }

        public GeoPoint getIntersectionPoint(GeoLineSegment another)
        {
            Point2 intersectionPoint;
            Point2 a0 = First.toPoint2();
            Point2 b0 = Second.toPoint2();
            Point2 a1 = another.First.toPoint2();
            Point2 b1 = another.Second.toPoint2();
            Line2.LineIntersectWithLine(a0, b0, a1, b1, out intersectionPoint);
            if (intersectionPoint.X == 0 && intersectionPoint.Y == 0)
            {
                return null;
            }
            return new GeoPoint(First.LatZone, First.LongZone, intersectionPoint.X, intersectionPoint.Y);
        }

        public GeoLineSegment resizeFromMiddle(double toLength)
        {
            var line2 = ToLine2();
            var middlePoint = Line2.Middle(line2.A, line2.B);
            var geoMiddlePoint = new GeoPoint(First.LatZone, First.LongZone, middlePoint.X, middlePoint.Y);
            var firstAnchor = new GeoLineSegment(geoMiddlePoint, First);
            var secondAnchor = new GeoLineSegment(geoMiddlePoint, Second);
            var firstResSegment = firstAnchor.resizeFromFirst(toLength / 2);
            var secondResSegment = secondAnchor.resizeFromFirst(toLength / 2);
            return new GeoLineSegment(firstResSegment.Second, secondResSegment.Second);
        }

        private GeoLineSegment resizeFromFirst(double toLength)
        {
            var curLength = Length();
            var c = toLength / curLength;
            var dX = Second.Easting - First.Easting;
            var dY = Second.Northing - First.Northing;
            return new GeoLineSegment(First, new GeoPoint(First.LatZone, First.LongZone, First.Easting + dX * c, First.Northing + dY * c));

        }

        public bool intersectsWith(GeoLineSegment another)
        {
            return getIntersectionPoint(another) != null;
        }

        public Line2 ToLine2()
        {
            Point2 first = First.toPoint2();
            Point2 second = Second.toPoint2();
            return new Line2(first, second);
        }

        public double Length()
        {
            return First.toPoint2().DistanceTo(Second.toPoint2());
        }

        public GeoLineSegment getPerpendicularLine(double cFromFirst)
        {
            const double eps = 2;
            SimplePoint firstIntersectionPoint;
            SimplePoint secondIntersectionPoint;
            var res = findCircleCircleIntersections(First.Easting, First.Northing, Length() * cFromFirst + eps, Second.Easting, Second.Northing, Length() * (1.0 - cFromFirst) + eps, out firstIntersectionPoint, out secondIntersectionPoint);
            if (res != 2)
            {
                throw new Exception(); // ??
            }
            var firstGeoPoint = new GeoPoint(First.LatZone, First.LongZone, firstIntersectionPoint.X, firstIntersectionPoint.Y);
            var secondGeoPoint = new GeoPoint(First.LatZone, First.LongZone, secondIntersectionPoint.X, secondIntersectionPoint.Y);
            return new GeoLineSegment(firstGeoPoint, secondGeoPoint);
        }

        private int findCircleCircleIntersections(double cx0, double cy0, double radius0, double cx1, double cy1, double radius1, out SimplePoint intersection1, out SimplePoint intersection2)
        {
            // Find the distance between the centers.  
            double dx = cx0 - cx1;
            double dy = cy0 - cy1;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            // See how manhym solutions there are.  
            if (dist > radius0 + radius1)
            {
                // No solutions, the circles are too far apart.       
                intersection1 = new SimplePoint(double.NaN, double.NaN);
                intersection2 = new SimplePoint(double.NaN, double.NaN);
                return 0;
            }
            if (dist < Math.Abs(radius0 - radius1))
            {
                // No solutions, one circle contains the other.     
                intersection1 = new SimplePoint(double.NaN, double.NaN);
                intersection2 = new SimplePoint(double.NaN, double.NaN);
                return 0;
            }
            if ((dist == 0) && (radius0 == radius1))
            {
                // No solutions, the circles coincide.    
                intersection1 = new SimplePoint(double.NaN, double.NaN);
                intersection2 = new SimplePoint(double.NaN, double.NaN);
                return 0;
            }
            // Find a and h.    
            double a = (radius0 * radius0 - radius1 * radius1 + dist * dist) / (2 * dist);
            double h = Math.Sqrt(radius0 * radius0 - a * a);
            // Find P2.      
            double cx2 = cx0 + a * (cx1 - cx0) / dist;
            double cy2 = cy0 + a * (cy1 - cy0) / dist;
            // Get the points P3.      
            intersection1 = new SimplePoint((double)(cx2 + h * (cy1 - cy0) / dist), (double)(cy2 - h * (cx1 - cx0) / dist));
            intersection2 = new SimplePoint((double)(cx2 - h * (cy1 - cy0) / dist), (double)(cy2 + h * (cx1 - cx0) / dist));
            // See if we have 1 or 2 solutions.     
            if (dist == radius0 + radius1) return 1;
            return 2;
        }
    }
}
