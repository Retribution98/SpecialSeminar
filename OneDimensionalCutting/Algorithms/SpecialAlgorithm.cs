using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OneDimensionalCutting.Models;
using Utils.Entities;

namespace OneDimensionalCutting.Algorithms
{
    public class SpecialAlgorithm : IAlgorithm
    {
        public Transposition GetTransposition(InputData data)
        {
            var transposition = new List<int>();
            var blankTypes = data.Blanks
                .GroupBy(x => x.Length, y => y)
                .OrderByDescending(x => x.Key)
                .ToDictionary(x => x.Key, y => new Stack<BlankDto>(y.ToList()));

            var difference = 0;
            while (difference < data.Rod.Length && blankTypes.Any(x => x.Value.Any()))
            {
                var subTransposition = GetSubTransposition(data.Rod.Length - difference, blankTypes);
                if (subTransposition != null && subTransposition.Any())
                {
                    transposition.AddRange(subTransposition);
                }
                difference++;
            }

            return new Transposition(transposition);
        }

        private IList<int> GetSubTransposition(int rodLength, IDictionary<int, Stack<BlankDto>> blankTypes)
        {
            var subTransposition = new List<int>();
            var skip = 0;
            while (true)
            {
                var lengths = blankTypes
                    .Where(x => x.Value.Count > 0)
                    .Select(x => x.Key)
                    .Skip(skip);

                if (lengths.Count() == 0)
                {
                    break;
                }

                var combine = GetCombine(rodLength, lengths);
                if (combine == null)
                {
                    skip++;
                    continue;
                }

                var countCombine = combine.GroupBy(x => x).Select(x => blankTypes[x.Key].Count / x.Count()).Min();
                if (countCombine == 0)
                {
                    skip++;
                    continue;
                }

                for (var i = 0; i < countCombine; i++)
                {
                    foreach (var el in combine)
                    {
                        var blank = blankTypes[el].Pop();
                        subTransposition.Add(blank.Id);
                    }
                }
            }
            return subTransposition;
        }

        private IList<int> GetCombine(int sum, IEnumerable<int> items)
        {
            foreach (var item in items)
            {
                if(item == sum)
                {
                    return new List<int> { item };
                }

                var difference = sum - item;
                if (items.Contains(difference))
                {
                    return new List<int> { item, difference };
                }

                var subCombine = GetCombine(difference, items.Where(x => x < difference));
                if (subCombine != null)
                {
                    subCombine.Add(item);
                    return subCombine;
                }
            }
            return null;
        }
    }
}
