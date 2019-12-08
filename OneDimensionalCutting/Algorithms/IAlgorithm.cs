using OneDimensionalCutting.Models;
using Utils.Entities;

namespace OneDimensionalCutting.Algorithms
{
    interface IAlgorithm
    {
        Transposition GetTransposition(InputData data);
    }
}
