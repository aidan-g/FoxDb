using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IFragmentBuilder : IEquatable<IFragmentBuilder>, ICloneable<IFragmentBuilder>
    {
        string Id { get; }

        IFragmentBuilder Parent { get; }

        IQueryGraphBuilder Graph { get; }

        FragmentType FragmentType { get; }

        void Touch();

        T Ancestor<T>() where T : IFragmentBuilder;

        T Fragment<T>() where T : IFragmentBuilder;

        ITableBuilder CreateTable(ITableConfig table);

        IRelationBuilder CreateRelation(IRelationConfig relation);

        ISubQueryBuilder CreateSubQuery(IQueryGraphBuilder query);

        IColumnBuilder CreateColumn(IColumnConfig column);

        IEnumerable<IColumnBuilder> CreateColumns(IEnumerable<IColumnConfig> columns);

        IParameterBuilder CreateParameter(string name, DbType type, int size, byte precision, byte scale, ParameterDirection direction, bool isDeclared, IColumnConfig column, DatabaseQueryParameterFlags flags);

        IFunctionBuilder CreateFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        IOperatorBuilder CreateOperator(QueryOperator @operator);

        IConstantBuilder CreateConstant(object value);

        IIdentifierBuilder CreateIdentifier(string name);

        IBinaryExpressionBuilder CreateBinary(IFragmentBuilder left, QueryOperator @operator, IFragmentBuilder right);

        IBinaryExpressionBuilder CreateBinary(IFragmentBuilder left, IOperatorBuilder @operator, IFragmentBuilder right);

        IUnaryExpressionBuilder CreateUnary(QueryOperator @operator, IFragmentBuilder expression);

        IUnaryExpressionBuilder CreateUnary(IOperatorBuilder @operator, IFragmentBuilder expression);

        ICaseBuilder CreateCase(params ICaseConditionBuilder[] expressions);

        ICaseConditionBuilder CreateCaseCondition(IFragmentBuilder result);

        ICaseConditionBuilder CreateCaseCondition(IFragmentBuilder condition, IFragmentBuilder result);

        ISequenceBuilder CreateSequence(params IExpressionBuilder[] expressions);

        IFragmentBuilder Combine(QueryOperator @operator, params IFragmentBuilder[] expressions);
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
        Index,
        Function,
        WindowFunction,
        Parameter,
        Add,
        Update,
        Delete,
        Output,
        Source,
        Filter,
        Aggregate,
        Sort,
        Sequence,
        Create,
        Alter,
        Drop,
        Identifier,
        Case,
        CaseCondition,
        With,
        CommonTableExpression
    }
}
