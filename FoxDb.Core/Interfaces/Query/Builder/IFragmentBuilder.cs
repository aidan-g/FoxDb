namespace FoxDb.Interfaces
{
    public interface IFragmentBuilder
    {
        FragmentType FragmentType { get; }

        void Touch();

        T GetFragment<T>() where T : IFragmentBuilder;

        ITableBuilder GetTable(ITableConfig table);

        IRelationBuilder GetRelation(IRelationConfig relation);

        ISubQueryBuilder GetSubQuery(IDatabaseQuery query);

        IColumnBuilder GetColumn(IColumnConfig column);

        IParameterBuilder GetParameter(string name);

        IFunctionBuilder GetFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        IOperatorBuilder GetOperator(QueryOperator @operator);

        IConstantBuilder GetConstant(object value);
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
        Insert,
        Update,
        Delete,
        Select,
        From,
        Where,
        OrderBy
    }
}
