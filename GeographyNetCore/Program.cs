using System;
using CoordinateSharp;
using Spatial;

namespace GeographyNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            test();
            Console.WriteLine("Hello World!");
        }

        static void test()
        {
            var testOfTest = new Coordinate(55.02285, 82.93245);
            Console.WriteLine(testOfTest.UTM);
            var firstLineFirstPoint = new Point2(1, 1);
            var firstLineSecondPoint = new Point2(5, 5);
            var secondLineFirstPoint = new Point2(0, 2);
            var secondLineSecondPoint = new Point2(0, 5);
            Point2 intersectPoint;
            Line2.LineIntersectWithLine(firstLineFirstPoint, firstLineSecondPoint, secondLineFirstPoint, secondLineSecondPoint, out intersectPoint);

            //GeoPoint[] BiggerPolygonPoints = {new GeoPoint()}
            //var BiggerPolygon = new GeoPolygon()
            var firstGeoPoint = new GeoPoint(55.02317, 82.9336);
            var secondGeoPoint = new GeoPoint(55.02278, 82.93259);

            var testGeoLine = new GeoLineSegment(firstGeoPoint, secondGeoPoint);
            var result = testGeoLine.getPerpendicularLine(0.05).resizeFromMiddle(20);
            Console.WriteLine(result.Length());
            
           // Reproject.ReprojectPoints()
            var test = 0;
        }
    }
}
