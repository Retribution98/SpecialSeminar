namespace Utils.Entities
{
    public interface IEdge<TVertice>
        where TVertice: Vertice
    {
        TVertice FromVertice { get; }

        TVertice ToVertice { get; }
    }
}
