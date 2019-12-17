using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Entities
{
    public class WeightedDirectedGraph: Graph<Vertice, DirectedEdge<Vertice>>
    {
        public Stream GetMaxStream()
        {
            // Формируем маркеры
            var markedGraph = new MarkedGraph();
            var vertices = Vertices.ToDictionary(x => x.Id, y => new MarkedVertice { Id = y.Id });
            var edges = Edges.Select(x => new MarkedDirectedEdge(vertices[x.FromVertice.Id], vertices[x.ToVertice.Id], x.Weight));
            markedGraph.AddVertices(vertices.Values);
            markedGraph.AddEdges(edges);

            var stockId = Vertices.Max(x => x.Id);
            while (true)
            {
                var selectedVertice = markedGraph.Vertices.First(x => x.Id == 0);
                while (selectedVertice.Id != stockId)
                {
                    var activeEdges = markedGraph.GeActiveEdgesByVertice(selectedVertice);

                     //.OrderByDescending(x => x.FromVertice.Id == verticeId
                     //    ? x.Source
                     //    : x.Reverse)
                     //.FirstOrDefault();
                }

            }
            throw new NotImplementedException();
        }

        private class MarkedGraph: Graph<MarkedVertice, MarkedDirectedEdge>
        {
            public IEnumerable<MarkedDirectedEdge> GeActiveEdgesByVertice(Vertice vertice)
            {
                return Edges
                     .Where(x => ((!x.ToVertice.IsSelected && x.FromVertice.Id == vertice.Id && x.Source > 0)
                         || (!x.FromVertice.IsSelected && x.ToVertice.Id == vertice.Id && x.Reverse > 0)));
            }
        }
        private class MarkedVertice: Vertice
        {
            public int Count { get; private set; }

            public int? FromId { get; private set; }

            public bool IsSelected { get; private set; }

            public void SetMarker(int count, int fromId)
            {
                Count = count;
                FromId = fromId;
                IsSelected = true;
            }

            public void ClearMarker()
            {
                IsSelected = false;
                Count = int.MaxValue;
                FromId = default(int?);
            }           
        }
        private class MarkedDirectedEdge: DirectedEdge<MarkedVertice>
        {
            public int Source { get; private set; }

            public int Reverse { get; private set; }

            public MarkedDirectedEdge(MarkedVertice from, MarkedVertice to, int weight) 
                :base(from, to, weight)
            {
                Source = weight;
                Reverse = 0;
            }

            public void UseSource(int count)
            {
                if (Source < count)
                {
                    throw new ArgumentException("Source is low");
                }

                Source -= count;
                Reverse += count;
            }

            public void UseReverse(int count)
            {
                if (Reverse < count)
                {
                    throw new ArgumentException("Reverse is low");
                }
                Source += count;
                Reverse -= count;
            }
        }
    }
}
