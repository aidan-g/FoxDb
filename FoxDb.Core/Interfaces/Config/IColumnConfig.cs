namespace FoxDb.Interfaces
{
    public interface IColumnConfig
    {
        string Name { get; set; }

        string Property { get; set; }

        bool IsKey { get; set; }
    }
}
