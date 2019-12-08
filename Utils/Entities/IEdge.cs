namespace Utils.Entities
{
    public interface IEdge
    {
        Vertice FromVertice { get; }

        Vertice ToVertice { get; }
    }
}
