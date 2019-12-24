using System;

namespace TravelingSalesman
{
    public class Point
    {
        public double X { get; }

        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static double GetLength(Point firstPoint, Point secondPoint)
        {
            var dx = firstPoint.X - secondPoint.X;
            var dy = firstPoint.Y - secondPoint.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public override bool Equals(object obj)
        {
            return obj is Point && this == (Point)obj;
        }

        public static bool operator ==(Point point1, Point point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        public override int GetHashCode()
        {
            return (X, Y).GetHashCode();
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
