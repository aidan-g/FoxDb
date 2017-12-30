namespace FoxDb.Interfaces
{
    public interface IQueryGraphBuilder
    {
        ISelectBuilder Select { get; }

        IInsertBuilder Insert { get; }

        IUpdateBuilder Update { get; }

        IDeleteBuilder Delete { get; }

        IFromBuilder From { get; }

        IWhereBuilder Where { get; }

        IOrderByBuilder OrderBy { get; }

        T Fragment<T>() where T : IFragmentBuilder;

        IQueryGraph Build();
    }
}