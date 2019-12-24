using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TravelingSalesman.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            string zipPath = @"..\..\..\Task5.zip";
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var points = new List<Point>();
                        var optimalValue = double.Parse(reader.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var splits = line
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            var coordinates = splits
                                .Skip(1)
                                .Take(2)
                                .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                                .ToList();
                            points.Add(new Point(coordinates[0], coordinates[1]));
                        }

                        var hamiltomBuilder1 = new HamiltonPathBuilder();
                        var hamiltomPath1 = await hamiltomBuilder1.GetHamiltonianPath(points, 10, 3);

                        Func<IEnumerable<Cluster>, Point, double> minDistantClustersToPoint = 
                            (IEnumerable<Cluster> clusters, Point point) =>
                            {
                                var distant = 0d;
                                var minDistant = double.MaxValue;
                                foreach (var clusster in clusters)
                                {
                                    distant = Point.GetLength(point, clusster.Center);
                                    if (distant < minDistant)
                                    {
                                        minDistant = distant;
                                    }
                                }
                                return minDistant;
                            };
                        var hamiltomBuilder2 = new HamiltonPathBuilder(minDistantClustersToPoint);
                        var hamiltomPath2 = await hamiltomBuilder2.GetHamiltonianPath(points, 10, 3);
                        Debug.WriteLine($@"{entry.Name}: {optimalValue} 
    {hamiltomPath1.Length} ({(hamiltomPath1.Length - optimalValue) / optimalValue}) 
    {hamiltomPath2.Length} ({(hamiltomPath2.Length - optimalValue) / optimalValue})");
                        Assert.Equal(points.Count, hamiltomPath1.Points.Count);
                        Assert.Equal(points.Count, hamiltomPath2.Points.Count);
                    }
                }
            }
        }
    }
}
