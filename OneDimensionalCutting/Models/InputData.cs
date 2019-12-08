using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Entities;

namespace OneDimensionalCutting.Models
{
    public class InputData
    {
        public RodDto Rod { get; set; }

        public IList<BlankDto> Blanks { get; set; }

        public InputData(int rod, IEnumerable<int> blanks)
        {
            Rod = new RodDto { Length = rod };
            var nextId = 1;
            Blanks = blanks
                .Select(x => new BlankDto
                {
                    Id = nextId++,
                    Length = x
                })
                .ToList();
        }

        public int GetCountRod(Transposition plane)
        {
            if (plane.Count != Blanks.Count)
            {
                throw new ArgumentOutOfRangeException("Incorrect transposition");
            }

            var bars = new List<RodDto>();
            foreach(var number in plane)
            {
                var len = Blanks[number - 1];
                var added = false;
                foreach (var bar in bars)
                {
                    if (bar.Length >= len.Length)
                    {
                        bar.Length -= len.Length;
                        added = true;
                        break;
                    }
                }

                if (added == false)
                {
                    bars.Add(new RodDto { Length = Rod.Length - len.Length});
                }
            }

            return bars.Count;
        }
    }
}
