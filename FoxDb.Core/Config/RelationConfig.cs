using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        private RelationConfig(IConfig config)
        {
            this.Config = config;
            this.Builder = FragmentBuilder.GetProxy(null);
            this.Expression = this.CreateConstraint();
        }

        protected RelationConfig(IConfig config, RelationFlags flags, string identifier, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable)
            : this(config)
        {
            this.Flags = flags;
            this.Identifier = identifier;
            this.LeftTable = leftTable;
            this.MappingTable = mappingTable;
            this.RightTable = rightTable;
        }

        public IConfig Config { get; private set; }

        public IFragmentBuilder Builder { get; private set; }

        public RelationFlags Flags { get; private set; }

        public string Identifier { get; private set; }

        public ITableConfig LeftTable { get; private set; }

        public IMappingTableConfig MappingTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public IBinaryExpressionBuilder Expression { get; set; }

        public abstract Type RelationType { get; }

        public abstract IRelationConfig AutoExpression();

        public virtual IBinaryExpressionBuilder CreateConstraint()
        {
            return this.CreateConstraint(null, null);
        }

        public virtual IBinaryExpressionBuilder CreateConstraint(IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            return this.Fragment<IBinaryExpressionBuilder>().With(builder =>
            {
                if (leftColumn != null)
                {
                    builder.Left = builder.CreateColumn(leftColumn);
                }
                builder.Operator = builder.CreateOperator(QueryOperator.Equal);
                if (rightColumn != null)
                {
                    builder.Right = builder.CreateColumn(rightColumn);
                }
            });
        }

        protected virtual T Fragment<T>() where T : IFragmentBuilder
        {
            if (this.Expression != null)
            {
                return this.Expression.Fragment<T>();
            }
            else
            {
                return this.Builder.Fragment<T>();
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                if (this.LeftTable != null)
                {
                    hashCode += this.LeftTable.GetHashCode();
                }
                if (this.MappingTable != null)
                {
                    hashCode += this.MappingTable.GetHashCode();
                }
                if (this.RightTable != null)
                {
                    hashCode += this.RightTable.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IRelationConfig)
            {
                return this.Equals(obj as IRelationConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(IRelationConfig other)
        {
            if (other == null)
            {
                return false;
            }
            if ((TableConfig)this.LeftTable != (TableConfig)other.LeftTable)
            {
                return false;
            }
            if ((TableConfig)this.MappingTable != (TableConfig)other.MappingTable)
            {
                return false;
            }
            if ((TableConfig)this.RightTable != (TableConfig)other.RightTable)
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(RelationConfig a, RelationConfig b)
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

        public static bool operator !=(RelationConfig a, RelationConfig b)
        {
            return !(a == b);
        }

        public ITableConfig GetOppositeTable(ITableConfig relativeTable)
        {
            if (TableComparer.TableConfig.Equals(this.LeftTable, relativeTable))
            {
                return this.RightTable;
            }
            if (TableComparer.TableConfig.Equals(this.RightTable, relativeTable))
            {
                return this.LeftTable;
            }
            throw new InvalidOperationException(string.Format("Table does not appear to be related: {0}", relativeTable));
        }

        public static IRelationSelector By(PropertyInfo property, RelationFlags? flags = null)
        {
            return By(string.Empty, property, flags);
        }

        public static IRelationSelector By(string identifier, PropertyInfo property, RelationFlags? flags = null)
        {
            return RelationSelector.By(identifier, property, flags);
        }

        public static IRelationSelector By(Expression expression, RelationFlags? flags = null)
        {
            return By(string.Empty, expression, flags);
        }

        public static IRelationSelector By(string identifier, Expression expression, RelationFlags? flags = null)
        {
            return RelationSelector.By(identifier, expression, flags);
        }

        public static IRelationSelector<T, TRelation> By<T, TRelation>(Expression<Func<T, TRelation>> expression, RelationFlags? flags = null)
        {
            return By(string.Empty, expression, flags);
        }

        public static IRelationSelector<T, TRelation> By<T, TRelation>(string identifier, Expression<Func<T, TRelation>> expression, RelationFlags? flags = null)
        {
            return RelationSelector<T, TRelation>.By(identifier, expression, flags);
        }
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        public RelationConfig(IConfig config, RelationFlags flags, string identifier, ITableConfig parent, ITableConfig table, IPropertyAccessor<T, TRelation> accessor)
            : base(config, flags, identifier, parent, null, table)
        {
            this.Accessor = accessor;
        }

        public override Type RelationType
        {
            get
            {
                return typeof(TRelation);
            }
        }

        public IPropertyAccessor<T, TRelation> Accessor { get; private set; }

        public override IRelationConfig AutoExpression()
        {
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns) && this.LeftTable.PrimaryKey != null)
            {
                this.Expression.Left = this.Expression.CreateColumn(this.LeftTable.PrimaryKey);
            }
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns) && this.LeftTable.PrimaryKey != null)
            {
                var column = default(IColumnConfig);
                if (this.RightTable.TryCreateColumn(
                    ColumnConfig.By(
                        Conventions.RelationColumn(this.LeftTable),
                        Factories.Type.Create(
                            TypeConfig.By(
                                this.LeftTable.PrimaryKey.ColumnType.Type,
                                this.LeftTable.PrimaryKey.ColumnType.Size,
                                this.LeftTable.PrimaryKey.ColumnType.Precision,
                                this.LeftTable.PrimaryKey.ColumnType.Scale,
                                this.Flags.HasFlag(RelationFlags.AllowNull)
                            )
                        )
                    ),
                    out column
                ))
                {
                    this.Expression.Right = this.Expression.CreateColumn(column);
                    column.IsForeignKey = true;
                }
            }
            return this;
        }

        public static IRelationSelector<T, TRelation> By(Expression<Func<T, TRelation>> expression, RelationFlags? flags = null)
        {
            return By(string.Empty, expression, flags);
        }

        public static IRelationSelector<T, TRelation> By(string identifier, Expression<Func<T, TRelation>> expression, RelationFlags? flags = null)
        {
            return RelationSelector<T, TRelation>.By(identifier, expression, flags);
        }
    }
}
