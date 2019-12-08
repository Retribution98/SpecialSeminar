using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;
using static OrderPortfolioFormation.PortfolioFast;

namespace OrderPortfolioFormation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string zipPath = @"..\..\..\Task3.zip";
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
                        var portfolioWight = int.Parse(reader.ReadLine());
                        var countOrder = int.Parse(reader.ReadLine());
                        var weights = reader.ReadLine()
                            .Split(" ")
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => int.Parse(x))
                            .ToList();
                        var prices = reader.ReadLine()
                            .Split(" ")
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => int.Parse(x))
                            .ToList();
                        var orders = new List<Order>();
                        for (var i = 0; i < countOrder; i++)
                        {
                            orders.Add(new Order
                            {
                                Id = i,
                                Price = prices[i],
                                Weight = weights[i]
                            });
                        }
                        var portfolioReq = new PortfolioReqursion(orders);
                        var portfolioTable = new PortfolioTable(orders);

                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        var optReq = portfolioReq.GetOrders(portfolioWight);
                        stopWatch.Stop();
                        var timeReq = stopWatch.Elapsed;

                        stopWatch.Restart();
                        var optTabl = portfolioTable.GetOrders(portfolioWight);
                        stopWatch.Stop();
                        var timeTabl = stopWatch.Elapsed;

                        Debug.WriteLine($"{entry.Name}\t{timeReq.TotalMilliseconds}\t{timeTabl.TotalMilliseconds}\t{2 * (timeReq - timeTabl) / (timeReq + timeTabl)}");

                        Equals(optReq, optTabl);
                    }
                }
            }
        }

        [Fact]
        public void Test2()
        {
            string zipPath = @"..\..\..\Task3.zip";
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
                        var portfolioWight = int.Parse(reader.ReadLine());
                        var countOrder = int.Parse(reader.ReadLine());
                        var weights = reader.ReadLine()
                            .Split(" ")
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => int.Parse(x))
                            .ToList();
                        var prices = reader.ReadLine()
                            .Split(" ")
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => int.Parse(x))
                            .ToList();
                        var orders = new List<Order>();
                        for (var i = 0; i < countOrder; i++)
                        {
                            orders.Add(new Order
                            {
                                Id = i,
                                Price = prices[i],
                                Weight = weights[i]
                            });
                        }
                        var portfolioFast = new PortfolioFast(orders);
                        var portfolioReq = new PortfolioReqursion(orders);

                        var optReq = portfolioReq.GetOrders(portfolioWight).Sum(x => x.Price);
                        Debug.Write($"{entry.Name}\t");
                        foreach (FastPercent perc in Enum.GetValues(typeof(FastPercent)))
                        {
                            var optFast = portfolioFast.GetOrdersFast(portfolioWight, perc).Sum(x => x.Price);
                            Debug.Write($"{(optReq - optFast)}\t");
                        }

                        Debug.WriteLine("");
                    }
                }
            }
        }
    }
}
