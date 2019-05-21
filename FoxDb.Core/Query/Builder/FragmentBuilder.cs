using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        protected static readonly IDictionary<Type, FragmentBuilderHandler> Factories = GetFactories();

        protected static IDictionary<Type, FragmentBuilderHandler> GetFactories()
        {
            return new Dictionary<Type, FragmentBuilderHandler>()
            {
                //Schema.
                { typeof(ICreateBuilder), (parent, graph) => new CreateBuilder(parent, graph) },
                { typeof(IAlterBuilder), (parent, graph) => new AlterBuilder(parent, graph) },
                { typeof(IDropBuilder), (parent, graph) => new DropBuilder(parent, graph) },
                //Expressions.
                { typeof(IOutputBuilder), (parent, graph) => new OutputBuilder(parent, graph) },
                { typeof(IAddBuilder), (parent, graph) => new AddBuilder(parent, graph) },
                { typeof(IUpdateBuilder), (parent, graph) => new UpdateBuilder(parent, graph) },
                { typeof(IDeleteBuilder), (parent, graph) => new DeleteBuilder(parent, graph) },
                { typeof(ISourceBuilder), (parent, graph) => new SourceBuilder(parent, graph) },
                { typeof(IFilterBuilder), (parent, graph) => new FilterBuilder(parent, graph) },
                { typeof(IAggregateBuilder), (parent, graph) => new AggregateBuilder(parent, graph) },
                { typeof(ISortBuilder), (parent, graph) => new SortBuilder(parent, graph) },
                { typeof(IWithBuilder), (parent, graph) => new WithBuilder(parent, graph) },
                //Fragments.
                { typeof(IUnaryExpressionBuilder), (parent, graph) => new UnaryExpressionBuilder(parent, graph) },
                { typeof(IBinaryExpressionBuilder), (parent, graph) => new BinaryExpressionBuilder(parent, graph) },
                { typeof(ITableBuilder), (parent, graph) => new TableBuilder(parent, graph) },
                { typeof(IRelationBuilder), (parent, graph) => new RelationBuilder(parent, graph) },
                { typeof(ISubQueryBuilder), (parent, graph) => new SubQueryBuilder(parent, graph) },
                { typeof(IColumnBuilder), (parent, graph) => new ColumnBuilder(parent, graph) },
                { typeof(IIndexBuilder), (parent, graph) => new IndexBuilder(parent, graph) },
                { typeof(IParameterBuilder), (parent, graph) => new ParameterBuilder(parent, graph) },
                { typeof(IFunctionBuilder), (parent, graph) => new FunctionBuilder(parent, graph) },
                { typeof(IWindowFunctionBuilder), (parent, graph) => new WindowFunctionBuilder(parent, graph) },
                { typeof(IOperatorBuilder), (parent, graph) => new OperatorBuilder(parent, graph) },
                { typeof(IConstantBuilder), (parent, graph) => new ConstantBuilder(parent, graph) },
                { typeof(ISequenceBuilder), (parent, graph) => new SequenceBuilder(parent, graph) },
                { typeof(IIdentifierBuilder), (parent, graph) => new IdentifierBuilder(parent, graph) },
                { typeof(ICaseBuilder), (parent, graph) => new CaseBuilder(parent, graph) },
                { typeof(ICaseConditionBuilder), (parent, graph) => new CaseConditionBuilder(parent, graph) },
                { typeof(ICommonTableExpressionBuilder), (parent, graph) => new CommonTableExpressionBuilder(parent, graph) }
            };
        }

        private FragmentBuilder()
        {
            this.Id = Unique.New;
        }

        protected FragmentBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : this()
        {
            this.Parent = parent;
            this.Graph = graph;
        }

        public string Id { get; private set; }

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

        public IEnumerable<IColumnBuilder> CreateColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                yield return this.CreateColumn(column);
            }
        }

        public IIndexBuilder CreateIndex(IIndexConfig index)
        {
            if (index == null)
            {
                throw new NotImplementedException();
            }
            return this.Fragment<IIndexBuilder>().With(builder => builder.Index = index);
        }

        public IParameterBuilder CreateParameter(string name, DbType type, int size, byte precision, byte scale, ParameterDirection direction, bool isDeclared, IColumnConfig column, DatabaseQueryParameterFlags flags)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NotImplementedException();
            }
            return this.Fragment<IParameterBuilder>().With(builder =>
            {
                builder.Name = name;
                builder.Type = type;
                builder.Size = size;
                builder.Precision = precision;
                builder.Scale = scale;
                builder.Direction = direction;
                builder.IsDeclared = isDeclared;
                builder.Column = column;
                builder.Flags = flags;
            });
        }

        public IFunctionBuilder CreateFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.Fragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = function;
                builder.AddArguments(arguments);
            });
        }

        public IWindowFunctionBuilder CreateWindowFunction(QueryWindowFunction function, params IExpressionBuilder[] arguments)
        {
            return this.Fragment<IWindowFunctionBuilder>().With(builder =>
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

        public IBinaryExpressionBuilder CreateBinary(IFragmentBuilder left, QueryOperator @operator, IFragmentBuilder right)
        {
            return this.CreateBinary(left, this.CreateOperator(@operator), right);
        }

        public IBinaryExpressionBuilder CreateBinary(IFragmentBuilder left, IOperatorBuilder @operator, IFragmentBuilder right)
        {
            return this.Fragment<IBinaryExpressionBuilder>().With(builder =>
            {
                builder.Left = left;
                builder.Operator = @operator;
                builder.Right = right;
            });
        }

        public IUnaryExpressionBuilder CreateUnary(QueryOperator @operator, IFragmentBuilder expression)
        {
            return this.CreateUnary(this.CreateOperator(@operator), expression);
        }

        public IUnaryExpressionBuilder CreateUnary(IOperatorBuilder @operator, IFragmentBuilder expression)
        {
            return this.Fragment<IUnaryExpressionBuilder>().With(builder =>
            {
                builder.Operator = @operator;
                builder.Expression = expression;
            });
        }

        public ISequenceBuilder CreateSequence(params IExpressionBuilder[] expressions)
        {
            return this.Fragment<ISequenceBuilder>().With(builder =>
            {
                foreach (var expression in expressions)
                {
                    builder.Write(expression);
                }
            });
        }

        public IIdentifierBuilder CreateIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new NotImplementedException();
            }
            return this.Fragment<IIdentifierBuilder>().With(builder => builder.Identifier = identifier);
        }

        public ICaseBuilder CreateCase(params ICaseConditionBuilder[] expressions)
        {
            return this.Fragment<ICaseBuilder>().With(builder =>
            {
                foreach (var expression in expressions)
                {
                    builder.Write(expression);
                }
            });
        }

        public ICaseConditionBuilder CreateCaseCondition(IFragmentBuilder result)
        {
            return this.CreateCaseCondition(null, result);
        }

        public ICaseConditionBuilder CreateCaseCondition(IFragmentBuilder condition, IFragmentBuilder result)
        {
            return this.Fragment<ICaseConditionBuilder>().With(builder =>
            {
                builder.Condition = condition;
                builder.Result = result;
            });
        }

        public ICommonTableExpressionBuilder CreateCommonTableExpression(params IQueryGraphBuilder[] expressions)
        {
            return this.Fragment<ICommonTableExpressionBuilder>().With(builder =>
            {
                foreach (var expression in expressions)
                {
                    builder.Write(this.CreateSubQuery(expression));
                }
            });
        }

        public IFragmentBuilder Combine(QueryOperator @operator, params IFragmentBuilder[] expressions)
        {
            var builder = default(IFragmentBuilder);
            foreach (var expression in expressions)
            {
                if (builder == null)
                {
                    builder = expression;
                }
                else
                {
                    builder = this.CreateBinary(builder, @operator, expression);
                }
            }
            return builder;
        }

        public abstract IFragmentBuilder Clone();

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
            public FragmentBuilderProxy(IQueryGraphBuilder graph)
                : this(null, graph)
            {
            }

            public FragmentBuilderProxy(IFragmentBuilder parent, IQueryGraphBuilder graph)
                : base(parent, graph)
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
        }

        public delegate IFragmentBuilder FragmentBuilderHandler(IFragmentBuilder parent, IQueryGraphBuilder graph);
    }
}
