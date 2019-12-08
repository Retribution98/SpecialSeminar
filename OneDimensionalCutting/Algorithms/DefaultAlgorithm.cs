using System.Linq;
using OneDimensionalCutting.Models;
using Utils.Entities;

namespace OneDimensionalCutting.Algorithms
{
    public class DefaultAlgorithm : IAlgorithm
    {
        public Transposition GetTransposition(InputData data)
        {
            var ids = data.Blanks.ToList()
                .OrderByDescending(x => x.Length)
                .Select(x => x.Id)
                .ToList();
            return new Transposition(ids);
        }
    }
}
