using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Utils.Entities;
using StreamSuppliers;
using Xunit;
using System.Diagnostics;
using Xunit.Abstractions;

namespace StreamSuppliers.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Task3()
        {
            string zipPath = @"..\..\..\Task4.zip";
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries.Take(7))
                {
                    if (!entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var inputDto = GetInputDto(reader);
                        var maxSizeStorage = inputDto
                            .MaxGetByCustomerInTick
                            .Select(x => x.Value.Sum())
                            .Max();
                        var needStream = inputDto
                            .MaxGetByCustomerInTick
                            .Sum(x => x.Value.Sum());
                        var left = 0;
                        var right = maxSizeStorage;

                        while(left != right)
                        {
                            var sizeStorage = (left + right) / 2;
                            var streamGraph = Program.GetStreamGraph(inputDto, sizeStorage);
                            var streams = streamGraph.GetMaxStream();
                            if (streams.Sum(x => x.Size) < needStream)
                            {
                                left = sizeStorage + 1;
                            }
                            else
                            {
                                right = sizeStorage;
                            }
                        }

                        Debug.WriteLine($"{entry.Name}: {left}");
                    }
                }
            }
        }

        [Fact]
        public void Task4()
        {
            string zipPath = @"..\..\..\Task4.zip";
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries.Take(7))
                {
                    if (!entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var inputDto = GetInputDto(reader);
                        var needStream = inputDto
                            .MaxGetByCustomerInTick
                            .Sum(x => x.Value.Sum());

                        var streamGraph = Program.GetStreamGraph(inputDto, int.MaxValue);
                        var streams = streamGraph.GetMaxStream();

                        if (streams.Sum(x => x.Size) < needStream)
                        {
                            Debug.WriteLine($"{entry.Name}: Customer's needs are too much");
                        }
                        else
                        {
                            var customersIds = new List<int>();
                            for (var i = 0; i < inputDto.CountCustomers; i++)
                            {
                                customersIds.Add(i);
                            }

                            for (var i = 0; i < inputDto.CountCustomers; i++)
                            {
                                streamGraph = Program.GetStreamGraph(inputDto, int.MaxValue, customersIds.Where(x => x != i));
                                streams = streamGraph.GetMaxStream();
                                if (streams.Sum(x => x.Size) == needStream)
                                {
                                    customersIds.Remove(i);
                                }
                            }

                            Debug.WriteLine($"{entry.Name}: Need {customersIds.Count} storages");
                        }
                    }
                }
            }
        }

        [Fact]
        public void MaxStreams()
        {
            string zipPath = @"..\..\..\Task4.zip";
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries.Skip(10))
                {
                    if (!entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var inputDto = GetInputDto(reader);

                        var streamGraph = Program.GetStreamGraph(inputDto, inputDto.MaxSendBySupplier.Sum());

                        var streams = streamGraph.GetMaxStream();

                        Debug.WriteLine($"{entry.Name}: {streams.Sum(x => x.Size)}");
                    }
                }
            }
        }

        [Fact]
        public void GetMaxStreams_NoException()
        {
            // Arrange
            var graph = new WeightedDirectedGraph();
            var vertice0 = new Vertice { Id = 0 };
            var vertice1 = new Vertice { Id = 1 };
            var vertice2 = new Vertice { Id = 2 };
            var vertice3 = new Vertice { Id = 3 };
            var vertice4 = new Vertice { Id = 4 };
            graph.AddVertices(new[] { vertice0, vertice1, vertice2, vertice3, vertice4 });
            graph.AddEdge(new DirectedEdge<Vertice>(vertice0, vertice1, 20));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice0, vertice2, 30));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice0, vertice3, 10));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice1, vertice2, 40));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice1, vertice4, 30));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice2, vertice3, 10));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice2, vertice4, 20));
            graph.AddEdge(new DirectedEdge<Vertice>(vertice3, vertice4, 20));

            // Act
            var streams = graph.GetMaxStream();

            // Assert
            Assert.Equal(60, streams.Sum(x => x.Size));
        }
        private InputDto GetInputDto(StreamReader reader)
        {
            var inputDto = new InputDto();
            inputDto.CountSupplier = int.Parse(reader.ReadLine());
            inputDto.CountCustomers = int.Parse(reader.ReadLine());
            inputDto.CountTicks = int.Parse(reader.ReadLine());
            reader.ReadLine();
            inputDto.MaxSendBySupplier = reader.ReadLine()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x))
                .ToArray();
            reader.ReadLine();
            inputDto.MaxSendBySupplierInTick = new Dictionary<int, int[]>(inputDto.CountSupplier);
            for (var i = 0; i < inputDto.CountSupplier; i++)
            {
                inputDto.MaxSendBySupplierInTick.Add(i, reader.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x))
                    .ToArray());
            }
            reader.ReadLine();
            inputDto.MaxGetByCustomerInTick = new Dictionary<int, int[]>(inputDto.CountCustomers);
            for (var i = 0; i < inputDto.CountCustomers; i++)
            {
                inputDto.MaxGetByCustomerInTick.Add(i, reader.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x))
                    .ToArray());
            }
            reader.ReadLine();
            inputDto.SupplierIdsForCustomer = new Dictionary<int, int[]>(inputDto.CountCustomers);
            for (var i = 0; i < inputDto.CountCustomers; i++)
            {
                inputDto.SupplierIdsForCustomer.Add(i, reader.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x) - 1)
                    .ToArray());
            }

            return inputDto;
        }
    }
}
