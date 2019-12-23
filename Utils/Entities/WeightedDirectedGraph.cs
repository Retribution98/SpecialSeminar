using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Utils.Entities
{
    public class WeightedDirectedGraph: Graph<Vertice, DirectedEdge<Vertice>>
    {
        public IEnumerable<Stream> GetMaxStream()
        {
            // Формируем маркеры
            var markedGraph = new MarkedGraph();
            var vertices = Vertices.ToDictionary(x => x.Id, y => new MarkedVertice { Id = y.Id });
            var edges = Edges.Select(x => new MarkedDirectedEdge(vertices[x.FromVertice.Id], vertices[x.ToVertice.Id], x.Weight));
            markedGraph.AddVertices(vertices.Values);
            markedGraph.AddEdges(edges);

            var stockId = markedGraph.Vertices.Max(x => x.Id);
            var streams = new List<Stream>();
            var isNotEnd = true;
            while (isNotEnd)
            {
                // нвходим доступные потоки
                // шаг 1
                if (streams.Count % 1000 == 0)
                {
                    Debug.WriteLine($"{streams.Count}: {streams.Sum(x => x.Size)}");
                }
                var selectedVertice = markedGraph.Vertices.First(x => x.Id == 0);
                selectedVertice.SetMarker(int.MaxValue, null);
                var edgesInStream = new List<DirectedEdge<Vertice>>();
                var selectedEdges = new List<MarkedDirectedEdge>();
                while (selectedVertice.Id != stockId)
                {
                    // ищем следующие вершины для потока
                    // шаг 2
                    var activeEdges = markedGraph.GetActiveEdgesByVertice(selectedVertice);

                    if (activeEdges.Any())
                    {
                        // швг 3
                        var selectEdge = activeEdges
                            .OrderByDescending(x => x.ToVertice.Id == stockId)
                            .ThenByDescending(x => x.GetWeightForVertice(selectedVertice))
                            .First();
                        var newSelectedVertice = selectEdge.FromVertice == selectedVertice
                           ? selectEdge.ToVertice
                           : selectEdge.FromVertice;
                        newSelectedVertice.SetMarker(selectEdge.GetWeightForVertice(selectedVertice), selectedVertice);
                        selectedEdges.Add(selectEdge);
                        edgesInStream.Add(new DirectedEdge<Vertice>(selectedVertice, newSelectedVertice, selectEdge.GetWeightForVertice(selectedVertice)));
                        selectedVertice = newSelectedVertice;
                    }
                    else
                    {
                        // шаг 4
                        if (selectedVertice.FromVertice == null)
                        { 
                            // шаг 6 
                            isNotEnd = false;
                            break;
                        }
                        selectedVertice = selectedVertice.FromVertice;
                        selectedEdges.RemoveAt(selectedEdges.Count - 1);
                        edgesInStream.Remove(edgesInStream.Last());
                    }
                }
                // шаг 5
                if (edgesInStream.Any())
                {
                    var stream = new Stream(edgesInStream);
                    streams.Add(stream);
                    // изменим пропускную способность у ребер
                    var vert = markedGraph.Vertices.First(x => x.IsSelected && x.Id == stockId );
                    foreach (var edge in selectedEdges)
                    {
                        if (edge.FromVertice == edge.ToVertice.FromVertice)
                        {
                            edge.UseSource(stream.Size);
                        }
                        else
                        {
                            edge.UseReverse(stream.Size);
                        }
                    }
                }

                markedGraph.ClearVerticeMarkers();
            }
            return streams;
        }

        private class MarkedGraph: Graph<MarkedVertice, MarkedDirectedEdge>
        {
            public IEnumerable<MarkedDirectedEdge> GetActiveEdgesByVertice(Vertice vertice)
            {
                return Edges
                     .Where(x => ((!x.ToVertice.IsSelected && x.FromVertice.Id == vertice.Id && x.Source > 0)
                         || (!x.FromVertice.IsSelected && x.ToVertice.Id == vertice.Id && x.Reverse > 0)));
            }

            public void ClearVerticeMarkers()
            {
                foreach(var vertice in Vertices)
                {
                    vertice.ClearMarker();
                }
            }
        }
        public class MarkedVertice: Vertice
        {
            public int Count { get; private set; }

            public MarkedVertice FromVertice { get; private set; }

            public bool IsSelected { get; private set; }

            public void SetMarker(int count, MarkedVertice fromId)
            {
                Count = count;
                FromVertice = fromId;
                IsSelected = true;
            }

            public void ClearMarker()
            {
                IsSelected = false;
                Count = int.MaxValue;
                FromVertice = null;
            }           
        }
        public class MarkedDirectedEdge: DirectedEdge<MarkedVertice>
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

            public int GetWeightForVertice(MarkedVertice vertice)
            {
                if (vertice == FromVertice)
                {
                    return Source;
                }
                
                if (vertice == ToVertice)
                {
                    return Reverse;
                }

                throw new ArgumentException("This vertice not include in this edge");
            }
        }
    }
}
