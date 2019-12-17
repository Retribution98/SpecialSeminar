using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Entities
{
    public class DirectedEdge<TVertice>: IEdge<TVertice>
        where TVertice: Vertice
    {
        public int Weight { get; }

        public TVertice FromVertice { get; }

        public TVertice ToVertice { get; }

        public DirectedEdge(TVertice from, TVertice to, int weight)
        {
            this.FromVertice = from;
            this.ToVertice = to;
            this.Weight = weight;
        }
    }
}
