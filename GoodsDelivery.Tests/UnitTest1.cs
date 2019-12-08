using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Utils.Algorithms;
using Utils.Entities;
using Xunit;

namespace GoodsDelivery.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string zipPath = @"..\..\..\Task2.zip";
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
                        var countDileveries = int.Parse(reader.ReadLine());
                        var directiveTimes = reader.ReadLine()
                            .Split(' ')
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => int.Parse(x))
                            .ToList();

                        var timeToOtherDeliveries = new List<IList<int>>(countDileveries);
                        for (var i = 0; i <= countDileveries; i++)
                        {
                            var toOther = reader.ReadLine()
                                .Split('\t')
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Select(x => int.Parse(x))
                                .ToList();
                            timeToOtherDeliveries.Add(toOther);
                        }

                        var deliveries = Delivery.Build(directiveTimes, timeToOtherDeliveries);

                        var startVertice = new DeiveryVerticeMody(deliveries.Where(x => x.Id == 0).ToList(), deliveries.Where(x => x.Id != 0).ToList());

                        //Func<List<IVertice<Delivery>>, IVertice<Delivery>> choiseStratagyBase = x => x.OrderBy(y => y.LowBound).First();

                        //var algorithm = new BranchAndBoundAlgorithm<DeiveryVerticeBase, Delivery>(choiseStratagyBase);
                        //var response = algorithm.Execute(startVertice);

                        Func<List<ITreeVertice<Delivery>>, ITreeVertice<Delivery>> myChoiseStratagy = x => 
                            x.OrderBy(y => y.LowBound)
                            .ThenByDescending(y => y.AvailableVertices.Average(z => z.TimeToOther.Values.Max()))
                            .First();
                        
                        var algorithm2 = new BranchAndBoundAlgorithm<DeiveryVerticeBase, Delivery>(myChoiseStratagy);
                        var response2 = algorithm2.Execute(startVertice);

                        Debug.WriteLine("");

                        //Equals(response.HighBound == response2.HighBound);
                    }
                }
            }
        }
    }
}
