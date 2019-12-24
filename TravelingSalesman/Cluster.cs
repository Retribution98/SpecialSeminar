using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public class Cluster : IEnumerable<Point>
    {
        public Point Center
        {
            get
            {
                double x = Points.Select(p => p.X).Average();
                double y = Points.Select(p => p.Y).Average();
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
