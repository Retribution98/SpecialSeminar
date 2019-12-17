using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Entities
{
    public abstract class Graph<TVertice, TEdge>
        where TVertice: Vertice
        where TEdge : IEdge<TVertice>
    {
        private readonly IList<TVertice> vertices;

        private readonly IList<TEdge> edges;

        public virtual IReadOnlyList<TVertice> Vertices => (IReadOnlyList<TVertice>)vertices;

        public virtual IReadOnlyList<TEdge> Edges => (IReadOnlyList<TEdge>)edges;

        public Graph()
        {
            vertices = new List<TVertice>();
            edges = new List<TEdge>();
        }

        public virtual void AddVertice(TVertice vertice)
        {
            if (vertices.Contains(vertice))
            {
                throw new ArgumentException("This vertice has already been added");
            }

            vertices.Add(vertice);
        }

        public virtual void AddVertices(IEnumerable<TVertice> vertices)
        {
            foreach(var vertice in vertices)
            {
                AddVertice(vertice);
            }
        }

        public virtual void AddEdge(TEdge edge)
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

        public virtual void AddEdges(IEnumerable<TEdge> edges)
        {
            foreach(var edge in edges)
            {
                AddEdge(edge);
            }
        }
    }
}
