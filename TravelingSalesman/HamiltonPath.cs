using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public class HamiltonPath
    {
        public IReadOnlyList<Point> Points { get; }

        public double Length {
            get
            {
                var len = 0d;
                if (Points.Count > 1)
                {
                    for (var i = 0; i < Points.Count - 1; i++)
                    {
                        len += Point.GetLength(Points[i], Points[i + 1]);
                    }
                    len += Point.GetLength(Points.Last(), Points.First());
                }
                return len;
            }
        }

        public HamiltonPath(List<Point> points)
        {
            Points = points.AsReadOnly();
        }

        public IList<Point> StartBy(Point point)
        {
            var index = 0;
            for (index = 0; index < Points.Count; index++)
            {
                if (Points[index] == point)
                {
                    break;
                }
            }

            var path = new List<Point>();
            path.AddRange(Points.Skip(index).Take(Points.Count - index));
            path.AddRange(Points.Take(index));
            return path;
        }

        public IList<Point> EndBy(Point point)
        {
            var index = 0;
            for (index = 0; index < Points.Count; index++)
            {
                if (Points[index] == point)
                {
                    break;
                }
            }

            var path = new List<Point>();
            path.AddRange(Points.Skip(index + 1).Take(Points.Count - index - 1));
            path.AddRange(Points.Take(index + 1));
            return path;
        }
    }
}
