namespace FoxDb.Interfaces
{
    public interface IDatabaseParameters
    {
        IDatabase Database { get; }

        IDatabaseQuery Query { get; }

        int Count { get; }

        bool Contains(string name);

        bool Contains(IColumnConfig column);

        void Reset();

        object this[string name] { get; set; }

        object this[IColumnConfig column] { get; set; }
    }
}
