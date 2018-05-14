namespace FoxDb.Interfaces
{
    public interface IDropBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        ITableBuilder SetTable(ITableConfig table);
    }
}
