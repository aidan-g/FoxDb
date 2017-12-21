namespace FoxDb.Interfaces
{
    public interface IDatabaseParameters
    {
        int Count { get; }

        bool Contains(string name);

        object this[string name] { get; set; }
    }
}
