namespace FoxDb.Interfaces
{
    public interface ITableHintBuilder : IFragmentBuilder
    {
        LockHints LockHints { get; set; }
    }
}
