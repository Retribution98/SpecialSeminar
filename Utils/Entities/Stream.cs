using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Entities
{
    public class Stream
    {
        public int Size { get; }

        public IReadOnlyList<Vertice> Vertices { get; }

        public Stream(IList<DirectedEdge<Vertice>> edges)
        {
            if (edges.Distinct().Count() != edges.Count)
            {
                throw new ArgumentException("This is not stream, because edges be dublicated");
            }
            for (var i = 0; i< edges.Count - 1; i++)
            {
                if (edges[i].ToVertice != edges[i + 1].FromVertice)
                {
                    throw new ArgumentException("This is not stream, because TOVertice not eqal FromVertice next edges");
                }
            }
            Size = edges.Min(x => x.Weight);
            Vertices = edges
                .Select(x => x.FromVertice)
                .Concat(new Vertice[] { edges.Last().ToVertice })
                .ToList();
        }

        public override string ToString()
        {
            var str = new StringBuilder($"Size: {Size}   ");
            str.AppendJoin(" -> ", Vertices.Select(x => x.Id));
            return str.ToString();
        }
    }
}
