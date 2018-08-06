namespace GeographyNetCore
{
    internal class SimplePoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        internal SimplePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

    }
}