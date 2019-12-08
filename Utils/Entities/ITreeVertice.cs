using System.Collections.Generic;

namespace Utils.Entities
{
    public interface ITreeVertice<T>
    {
        IReadOnlyList<T> SelectedVertices { get; }

        IReadOnlyList<T> AvailableVertices { get; }

        int HighBound { get; }

        int LowBound { get; }

        ITreeVertice<T> HighResponse { get; }

        IEnumerable<ITreeVertice<T>> GetNextVertices();
    }
}
