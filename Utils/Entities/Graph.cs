using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Entities
{
    public abstract class Graph
    {
        private readonly IList<Vertice> vertices;

        private readonly IList<IEdge> edges;

        public IReadOnlyList<Vertice> Vertices => (IReadOnlyList<Vertice>)vertices;

        public IReadOnlyList<IEdge> Edges => (IReadOnlyList<IEdge>)edges;

        public Graph()
        {
            vertices = new List<Vertice>();
            edges = new List<IEdge>();
        }

        public void AddVertice(Vertice vertice)
        {
            if (vertices.Contains(vertice))
            {
                throw new ArgumentException("This vertice has already been added");
            }

            vertices.Add(vertice);
        }

        public void AddEdge(IEdge edge)
        {
            if (edges.Any(x => x.FromVertice == edge.FromVertice 
                            && x.ToVertice == edge.ToVertice))
            {
                throw new ArgumentException("This edge has already been added");
            }

            if (!vertices.Contains(edge.FromVertice))
            {
                throw new ArgumentException($"FromVertice {edge.FromVertice} is not contained in the graph");
            }

            if (!vertices.Contains(edge.ToVertice))
            {
                throw new ArgumentException($"ToVertice {edge.ToVertice} is not contained in the graph");
            }

            edges.Add(edge);
        }
    }
}
