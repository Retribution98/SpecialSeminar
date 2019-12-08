using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneDimensionalCutting.Models;
using Utils.Entities;

namespace OneDimensionalCutting.Algorithms
{
    public class IterateAlgorithm : IAlgorithm
    {
        private readonly int iterationCount;

        private readonly int skipPercent = 10;

        public IterateAlgorithm(int iterationCount, int skipPercent)
        {
            if(skipPercent > 100 || skipPercent < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.iterationCount = iterationCount;
            this.skipPercent = skipPercent;
        }

        public Transposition GetTransposition(InputData data)
        {
            var bestResult = int.MaxValue;
            var bestTrasnposition = new Transposition(data.Blanks.Count);
            for (var i = 0; i < iterationCount; i++)
            {
                var transposition = GetIterateTransposition2(data);
                var result = data.GetCountRod(transposition);
                if (result < bestResult)
                {
                    bestResult = result;
                    bestTrasnposition = transposition;
                }
            }

            return bestTrasnposition;
        }

        private Transposition GetIterateTransposition(InputData data)
        {
            var rod = new RodDto { Length = data.Rod.Length };
            var blanks = data.Blanks.ToList();
            var rand = new Random();
            var transposition = new List<int>();

            while (blanks.Any())
            {
                var front = blanks
                    .Where(x => x.Length < rod.Length)
                    .ToList();

                if (!front.Any())
                {
                    rod = new RodDto { Length = data.Rod.Length };
                    continue;
                }

                var skip = rand.Next(100) < skipPercent ? rand.Next(front.Count) : 0;
                var blank = front
                    .Skip(skip)
                    .First();

                transposition.Add(blank.Id);
                blanks.RemoveAll(x => x.Id == blank.Id);
            }

            return new Transposition(transposition);
        }

        private Transposition GetIterateTransposition2(InputData data)
        {
            var blanks = data.Blanks.OrderByDescending(x => x.Length).ToList();
            var rand = new Random();
            var transposition = new List<int>();

            while (blanks.Any())
            {
                var skip = rand.Next(100) < skipPercent ? rand.Next(blanks.Count) : 0;
                var blank = blanks
                    .Skip(skip)
                    .First();

                transposition.Add(blank.Id);
                blanks.RemoveAll(x => x.Id == blank.Id);
            }

            return new Transposition(transposition);
        }
    }
}
