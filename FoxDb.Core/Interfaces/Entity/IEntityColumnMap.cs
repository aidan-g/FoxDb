namespace FoxDb.Interfaces
{
    public interface IEntityColumnMap
    {
        IColumnConfig Column { get; }

        string Identifier { get; }
    }
}
