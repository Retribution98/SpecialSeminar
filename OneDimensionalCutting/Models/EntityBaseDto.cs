using System;
using System.Collections.Generic;
using System.Text;

namespace OneDimensionalCutting.Models
{
    public abstract class EntityBaseDto: IComparable<EntityBaseDto>
    {
        public int Id { get; set; }

        public int Length { get; set; }

        public int CompareTo(EntityBaseDto other)
        {
            return Length.CompareTo(other.Length);
        }
    }
}
