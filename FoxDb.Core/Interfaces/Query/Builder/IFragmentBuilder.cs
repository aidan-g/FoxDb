namespace FoxDb.Interfaces
{
    public interface IFragmentBuilder
    {
        FragmentType FragmentType { get; }

        void Touch();

        T CreateFragment<T>() where T : IFragmentBuilder;

        ITableBuilder CreateTable(ITableConfig table);

        IRelationBuilder CreateRelation(IRelationConfig relation);

        ISubQueryBuilder CreateSubQuery(IQueryGraphBuilder query);

        IColumnBuilder CreateColumn(IColumnConfig column);

        IParameterBuilder CreateParameter(string name);

        IFunctionBuilder CreateFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        IOperatorBuilder CreateOperator(QueryOperator @operator);

        IConstantBuilder CreateConstant(object value);
    }

    public enum FragmentType : byte
    {
        None,
        Unary,
        Binary,
        Operator,
        Constant,
        Table,
        Relation,
        SubQuery,
        Column,
        Function,
        Parameter,
        Add,
        Update,
        Delete,
        Output,
        Source,
        Filter,
        Aggregate,
        Sort
    }
}
