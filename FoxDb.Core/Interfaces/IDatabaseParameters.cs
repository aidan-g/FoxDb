namespace FoxDb.Interfaces
{
    public interface IDatabaseParameters
    {
        int Count { get; }

        bool Contains(string name);

        void Reset();

        object this[string name] { get; set; }
    }
}
