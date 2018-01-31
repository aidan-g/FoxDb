using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb
{
    [DebuggerDisplay("{DebugView}")]
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        protected static readonly IDictionary<Type, FragmentBuilderHandler> Factories = GetFactories();

        protected static IDictionary<Type, FragmentBuilderHandler> GetFactories()
        {
            return new Dictionary<Type, FragmentBuilderHandler>()
            {
                //Expressions.
                { typeof(IOutputBuilder), (parent, graph) => new OutputBuilder(parent, graph) },
                { typeof(IAddBuilder), (parent, graph) => new AddBuilder(parent, graph) },
                { typeof(IUpdateBuilder), (parent, graph) => new UpdateBuilder(parent, graph) },
                { typeof(IDeleteBuilder), (parent, graph) => new DeleteBuilder(parent, graph) },
                { typeof(ISourceBuilder), (parent, graph) => new SourceBuilder(parent, graph) },
                { typeof(IFilterBuilder), (parent, graph) => new FilterBuilder(parent, graph) },
                { typeof(IAggregateBuilder), (parent, graph) => new AggregateBuilder(parent, graph) },
                { typeof(ISortBuilder), (parent, graph) => new SortBuilder(parent, graph) },
                //Fragments.
                { typeof(IUnaryExpressionBuilder), (parent, graph) => new UnaryExpressionBuilder(parent, graph) },
                { typeof(IBinaryExpressionBuilder), (parent, graph) => new BinaryExpressionBuilder(parent, graph) },
                { typeof(ITableBuilder), (parent, graph) => new TableBuilder(parent, graph) },
                { typeof(IRelationBuilder), (parent, graph) => new RelationBuilder(parent, graph) },
                { typeof(ISubQueryBuilder), (parent, graph) => new SubQueryBuilder(parent, graph) },
                { typeof(IColumnBuilder), (parent, graph) => new ColumnBuilder(parent, graph) },
                { typeof(IParameterBuilder), (parent, graph) => new ParameterBuilder(parent, graph) },
                { typeof(IFunctionBuilder), (parent, graph) => new FunctionBuilder(parent, graph) },
                { typeof(IOperatorBuilder), (parent, graph) => new OperatorBuilder(parent, graph) },
                { typeof(IConstantBuilder), (parent, graph) => new ConstantBuilder(parent, graph) },
            };
        }

        protected FragmentBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
        {
            this.Parent = parent;
            this.Graph = graph;
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
            var factory = default(FragmentBuilderHandler);
            if (!Factories.TryGetValue(typeof(T), out factory))
            {
                throw new NotImplementedException();
            }
            return (T)factory(this, this.Graph);
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

        public delegate IFragmentBuilder FragmentBuilderHandler(IFragmentBuilder parent, IQueryGraphBuilder graph);
    }
}
