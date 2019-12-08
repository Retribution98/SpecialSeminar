using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Entities
{
    public class DirectedEdge: IEdge
    {
        public int Weight { get; }

        public Vertice FromVertice { get; }

        public Vertice ToVertice { get; }

        public DirectedEdge(Vertice from, Vertice to, int weight)
        {
            this.FromVertice = from;
            this.ToVertice = to;
            this.Weight = weight;
        }
    }
}
