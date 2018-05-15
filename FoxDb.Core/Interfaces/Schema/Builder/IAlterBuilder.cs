namespace FoxDb.Interfaces
{
    public interface IAlterBuilder : IFragmentBuilder
    {
        ITableBuilder LeftTable { get; set; }

        ITableBuilder SetLeftTable(ITableConfig table);

        ITableBuilder RightTable { get; set; }

        ITableBuilder SetRightTable(ITableConfig table);
    }
}
