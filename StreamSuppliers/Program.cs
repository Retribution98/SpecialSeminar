using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Entities;

namespace StreamSuppliers
{
    public class Program
    {
        static void Main(string[] args)
        {
            var graph = new WeightedDirectedGraph();
            var root = new Vertice { Id = 0 };
            graph.AddVertice(root);
            graph.AddEdge(new DirectedEdge<Vertice>(root, root, 5));
        }

        public static WeightedDirectedGraph GetStreamGraph(InputDto input)
        {
            var streamGraph = new WeightedDirectedGraph();

            var nextIdVertice = 0;
            var rootVertice = new Vertice { Id = nextIdVertice++ };
            streamGraph.AddVertice(rootVertice);

            var supplierVartices = new Vertice[input.CountSupplier];
            for (var i = 0; i< input.CountSupplier; i++)
            {
                supplierVartices[i] = new Vertice { Id = nextIdVertice++ };
                streamGraph.AddVertice(supplierVartices[i]);
                streamGraph.AddEdge(new DirectedEdge<Vertice>(rootVertice, supplierVartices[i], input.MaxSendBySupplier[i]));
            }

            var supplierInTick = new Dictionary<int, Vertice[]>(input.CountSupplier);
            for (var i = 0; i< input.CountSupplier; i++)
            {
                supplierInTick.Add(i, input.MaxSendBySupplierInTick[i]
                    .Select(x => new Vertice
                    {
                        Id = nextIdVertice++
                    })
                    .ToArray());
                streamGraph.AddVertices(supplierInTick[i]);
                for(var j = 0; j < input.CountTicks; j++)
                {
                    streamGraph.AddEdge(new DirectedEdge<Vertice>(supplierVartices[i], supplierInTick[i][j], input.MaxSendBySupplierInTick[i][j]));
                }
            }

            var maxStream = input.MaxSendBySupplier.Sum();
            var customerInTick = new Dictionary<int, Vertice[]>(input.CountCustomers);
            for (var i = 0; i < input.CountCustomers; i++)
            {
                customerInTick.Add(i, input.MaxGetByCustomerInTick[i]
                    .Select(x => new Vertice
                    {
                        Id = nextIdVertice++
                    })
                    .ToArray());
                streamGraph.AddVertices(customerInTick[i]);
                for (var tick = 0; tick < input.CountTicks; tick++)
                {
                    foreach (var supplierId in input.SupplierIdsForCustomer[i])
                    {
                        streamGraph.AddEdge(new DirectedEdge<Vertice>(supplierInTick[supplierId][tick], customerInTick[i][tick], maxStream));
                    }
                }
            }

            var endVertice = new Vertice { Id = nextIdVertice++ };
            streamGraph.AddVertice(endVertice);
            for (var id = 0; id < input.CountCustomers; id++)
            {
                for (var tick = 0; tick < input.CountTicks; tick++)
                {
                    streamGraph.AddEdge(new DirectedEdge<Vertice>(customerInTick[id][tick], endVertice, input.MaxGetByCustomerInTick[id][tick]));
                }
            }

            return streamGraph;
        }
    }
}
