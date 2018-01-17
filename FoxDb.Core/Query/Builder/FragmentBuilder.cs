using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        public static readonly IDictionary<Type, Func<IFragmentBuilder>> Factories = new Dictionary<Type, Func<IFragmentBuilder>>()
        {
            //Expressions.
            { typeof(IOutputBuilder), () => new OutputBuilder() },
            { typeof(IAddBuilder), () => new AddBuilder() },
            { typeof(IUpdateBuilder), () => new UpdateBuilder() },
            { typeof(IDeleteBuilder), () => new DeleteBuilder() },
            { typeof(ISourceBuilder), () => new SourceBuilder() },
            { typeof(IFilterBuilder), () => new FilterBuilder() },
            { typeof(ISortBuilder), () => new SortBuilder() },
            //Fragments.
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

        public virtual string CommandText
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Touch()
        {
            //Nothing to do.
        }

        public T CreateFragment<T>() where T : IFragmentBuilder
        {
            var factory = default(Func<IFragmentBuilder>);
            if (!Factories.TryGetValue(typeof(T), out factory))
            {
                throw new NotImplementedException();
            }
            return (T)factory();
        }

        public ITableBuilder CreateTable(ITableConfig table)
        {
            if (table == null)
            {
                throw new NotImplementedException();
            }
            if (table.Flags.HasFlag(TableFlags.Transient))
            {
                throw new InvalidOperationException(string.Format("Table of type \"{0}\" is is transient and cannot be queried.", table.TableType.FullName));
            }
            return this.CreateFragment<ITableBuilder>().With(builder => builder.Table = table);
        }

        public IRelationBuilder CreateRelation(IRelationConfig relation)
        {
            if (relation == null)
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<IRelationBuilder>().With(builder => builder.Relation = relation);
        }

        public ISubQueryBuilder CreateSubQuery(IQueryGraphBuilder query)
        {
            if (query == null)
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<ISubQueryBuilder>().With(builder => builder.Query = query);
        }

        public IColumnBuilder CreateColumn(IColumnConfig column)
        {
            if (column == null)
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<IColumnBuilder>().With(builder => builder.Column = column);
        }

        public IParameterBuilder CreateParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<IParameterBuilder>().With(builder => builder.Name = name);
        }

        public IFunctionBuilder CreateFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.CreateFragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = function;
                builder.AddArguments(arguments);
            });
        }

        public IOperatorBuilder CreateOperator(QueryOperator @operator)
        {
            return this.CreateFragment<IOperatorBuilder>().With(builder => builder.Operator = @operator);
        }

        public IConstantBuilder CreateConstant(object value)
        {
            return this.CreateFragment<IConstantBuilder>().With(builder => builder.Value = value);
        }
    }
}
