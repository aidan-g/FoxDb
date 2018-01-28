using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb
{
    [DebuggerDisplay("{DebugView}")]
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        protected IDictionary<Type, Func<IFragmentBuilder>> Factories { get; private set; }

        protected virtual IDictionary<Type, Func<IFragmentBuilder>> GetFactories(IQueryGraphBuilder graph)
        {
            return new Dictionary<Type, Func<IFragmentBuilder>>()
            {
                //Expressions.
                { typeof(IOutputBuilder), () => new OutputBuilder(this, graph) },
                { typeof(IAddBuilder), () => new AddBuilder(this, graph) },
                { typeof(IUpdateBuilder), () => new UpdateBuilder(this, graph) },
                { typeof(IDeleteBuilder), () => new DeleteBuilder(this, graph) },
                { typeof(ISourceBuilder), () => new SourceBuilder(this, graph) },
                { typeof(IFilterBuilder), () => new FilterBuilder(this, graph) },
                { typeof(IAggregateBuilder), () => new AggregateBuilder(this, graph) },
                { typeof(ISortBuilder), () => new SortBuilder(this, graph) },
                //Fragments.
                { typeof(IUnaryExpressionBuilder), () => new UnaryExpressionBuilder(this, this.Graph) },
                { typeof(IBinaryExpressionBuilder), () => new BinaryExpressionBuilder(this, graph) },
                { typeof(ITableBuilder), () => new TableBuilder(this, graph) },
                { typeof(IRelationBuilder), () => new RelationBuilder(this, graph) },
                { typeof(ISubQueryBuilder), () => new SubQueryBuilder(this, graph) },
                { typeof(IColumnBuilder), () => new ColumnBuilder(this, graph) },
                { typeof(IParameterBuilder), () => new ParameterBuilder(this, graph) },
                { typeof(IFunctionBuilder), () => new FunctionBuilder(this, graph) },
                { typeof(IOperatorBuilder), () => new OperatorBuilder(this, graph) },
                { typeof(IConstantBuilder), () => new ConstantBuilder(this, graph) },
            };
        }

        protected FragmentBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
        {
            this.Parent = parent;
            this.Graph = graph;
            this.Factories = this.GetFactories(graph);
        }

        public IFragmentBuilder Parent { get; private set; }

        public IQueryGraphBuilder Graph { get; private set; }

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

        public T Ancestor<T>() where T : IFragmentBuilder
        {
            var stack = new Stack<IFragmentBuilder>();
            stack.Push(this);
            while (stack.Count > 0)
            {
                var expression = stack.Pop();
                if (expression is T)
                {
                    return (T)expression;
                }
                if (expression.Parent != null)
                {
                    stack.Push(expression.Parent);
                }
            }
            return default(T);
        }

        public T Fragment<T>() where T : IFragmentBuilder
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
                throw new InvalidOperationException(string.Format("Table is transient and cannot be queried: ", table));
            }
            return this.Fragment<ITableBuilder>().With(builder => builder.Table = table);
        }

        public IRelationBuilder CreateRelation(IRelationConfig relation)
        {
            if (relation == null)
            {
                throw new NotImplementedException();
            }
            return this.Fragment<IRelationBuilder>().With(builder => builder.Relation = relation);
        }

        public ISubQueryBuilder CreateSubQuery(IQueryGraphBuilder query)
        {
            if (query == null)
            {
                throw new NotImplementedException();
            }
            return this.Fragment<ISubQueryBuilder>().With(builder => builder.Query = query);
        }

        public IColumnBuilder CreateColumn(IColumnConfig column)
        {
            if (column == null)
            {
                throw new NotImplementedException();
            }
            return this.Fragment<IColumnBuilder>().With(builder => builder.Column = column);
        }

        public IParameterBuilder CreateParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NotImplementedException();
            }
            return this.Fragment<IParameterBuilder>().With(builder => builder.Name = name);
        }

        public IFunctionBuilder CreateFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.Fragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = function;
                builder.AddArguments(arguments);
            });
        }

        public IOperatorBuilder CreateOperator(QueryOperator @operator)
        {
            return this.Fragment<IOperatorBuilder>().With(builder => builder.Operator = @operator);
        }

        public IConstantBuilder CreateConstant(object value)
        {
            return this.Fragment<IConstantBuilder>().With(builder => builder.Value = value);
        }

        public abstract IFragmentBuilder Clone();

        public abstract string DebugView { get; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.FragmentType.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IFragmentBuilder)
            {
                return this.Equals(obj as IFragmentBuilder);
            }
            return base.Equals(obj);
        }

        public virtual bool Equals(IFragmentBuilder other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.FragmentType != other.FragmentType)
            {
                return false;
            }
            return true;
        }

        public static IFragmentBuilder GetProxy(IQueryGraphBuilder graph)
        {
            return new FragmentBuilderProxy(graph);
        }

        public static bool operator ==(FragmentBuilder a, FragmentBuilder b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            if (object.ReferenceEquals((object)a, (object)b))
            {
                return true;
            }
            return a.Equals(b);
        }

        public static bool operator !=(FragmentBuilder a, FragmentBuilder b)
        {
            return !(a == b);
        }

        protected class FragmentBuilderProxy : FragmentBuilder
        {
            public FragmentBuilderProxy(IQueryGraphBuilder graph) : this(null, graph)
            {
            }

            public FragmentBuilderProxy(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
            {
            }

            public override FragmentType FragmentType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override IFragmentBuilder Clone()
            {
                throw new NotImplementedException();
            }

            public override string DebugView
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
