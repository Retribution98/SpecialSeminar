namespace Utils.Entities
{
    public class Vertice
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Vertice vertice)
            {
                if (vertice.Id == this.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
