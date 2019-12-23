using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelingSalesman
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static async Task<IList<Point>> GetHamiltonianPath(IEnumerable<Point> points, int countClusster, int countStep)
        {
            if (points.Count() <= 3)
            {
                return points.ToList();
            }

            if (countStep < 1 || points.Count() <= countClusster)
            {
                // жаднный подход
                throw new NotImplementedException();
            }

            // найти две наиболее отдаленные точки
            var clusters = GetTwoMostDistantPoint(points)
                .Select(x => new Cluster(x))
                .ToList();
            while (clusters.Count < countClusster)
            {
                double mostDistant = 0;
                Point nextClusterPoint = null;
                double distant = 0;
                foreach(var point in points.Where(x => !clusters.SelectMany(x => x.Points).Contains(x)))
                {
                    foreach(var clusster in clusters)
                    {
                        distant += Point.GetLength(point, clusster.Center);
                    }

                    if (distant > mostDistant)
                    {
                        mostDistant = distant;
                        nextClusterPoint = point;
                    }
                }
                clusters.Add(new Cluster(nextClusterPoint));
            }

            foreach (var point in points.Where(x => !clusters.SelectMany(c => c.Points).Contains(x)))
            {
                
            }

                throw new NotImplementedException();
        }

        private static IList<Point> GetTwoMostDistantPoint(IEnumerable<Point> points)
        {
            if(points.Count() < 2)
            {
                throw new ArgumentException("count points < 2");
            }
            double maxDistant = 0;
            Point first = null;
            Point second = null;
            double distant;
            foreach(var p1 in points)
            {
                foreach(var p2 in points)
                {
                    distant = Point.GetLength(p1, p2);
                    if (distant > maxDistant)
                    {
                        maxDistant = distant;
                        first = p1;
                        second = p2;
                    }
                }
            }
            return new List<Point> { first, second };
        }
        
        public class Point
        {
            public double X { get; }

            public double Y { get;  }

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
                throw new NotImplementedException();
            }
        }

        public class Cluster : IEnumerable<Point>
        {
            public Point Center {
                get
                {
                    double x = Points.Select(p => p.X).Average();
                    double y = Points.Select(p => p.y).Average();
                    return new Point(x, y);
                }
            }

            public IList<Point> Points { get; }

            public Cluster(Point center)
            {
                Points = new List<Point> { center };
            }

            public IEnumerator<Point> GetEnumerator()
            {
                return Points.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Points.GetEnumerator();
            }
        }
    }
}
