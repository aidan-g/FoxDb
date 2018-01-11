using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        public static readonly IDictionary<Type, Func<IFragmentBuilder>> Factories = new Dictionary<Type, Func<IFragmentBuilder>>()
        {
            { typeof(IBinaryExpressionBuilder), () => new BinaryExpressionBuilder() },
            { typeof(ITableBuilder), () => new TableBuilder() },
            { typeof(IRelationBuilder), () => new RelationBuilder() },
            { typeof(ISubQueryBuilder), () => new SubQueryBuilder() },
            { typeof(IColumnBuilder), () => new ColumnBuilder() },
            { typeof(IParameterBuilder), () => new ParameterBuilder() },
            { typeof(IFunctionBuilder), () => new FunctionBuilder() },
            { typeof(IOperatorBuilder), () => new OperatorBuilder() },
            { typeof(IConstantBuilder), () => new ConstantBuilder() },
        };

        public abstract FragmentType FragmentType { get; }

        public void Touch()
        {
            //Nothing to do.
        }

        public T GetFragment<T>() where T : IFragmentBuilder
        {
            var factory = default(Func<IFragmentBuilder>);
            if (!Factories.TryGetValue(typeof(T), out factory))
            {
                throw new NotImplementedException();
            }
            return (T)factory();
        }

        public ITableBuilder GetTable(ITableConfig table)
        {
            if (table == null)
            {
                throw new NotImplementedException();
            }
            if (table.Flags.HasFlag(TableFlags.Transient))
            {
                throw new InvalidOperationException(string.Format("Table of type \"{0}\" is is transient and cannot be queried.", table.TableType.FullName));
            }
            return this.GetFragment<ITableBuilder>().With(builder => builder.Table = table);
        }

        public IRelationBuilder GetRelation(IRelationConfig relation)
        {
            if (relation == null)
            {
                throw new NotImplementedException();
            }
            return this.GetFragment<IRelationBuilder>().With(builder => builder.Relation = relation);
        }

        public ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query)
        {
            if (query == null)
            {
                throw new NotImplementedException();
            }
            return this.GetFragment<ISubQueryBuilder>().With(builder => builder.Query = query);
        }

        public IColumnBuilder GetColumn(IColumnConfig column)
        {
            if (column == null)
            {
                throw new NotImplementedException();
            }
            return this.GetFragment<IColumnBuilder>().With(builder => builder.Column = column);
        }

        public IParameterBuilder GetParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NotImplementedException();
            }
            return this.GetFragment<IParameterBuilder>().With(builder => builder.Name = name);
        }

        public IFunctionBuilder GetFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.GetFragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = function;
                builder.AddArguments(arguments);
            });
        }

        public IOperatorBuilder GetOperator(QueryOperator @operator)
        {
            return this.GetFragment<IOperatorBuilder>().With(builder => builder.Operator = @operator);
        }

        public IConstantBuilder GetConstant(object value)
        {
            return this.GetFragment<IConstantBuilder>().With(builder => builder.Value = value);
        }
    }
}
