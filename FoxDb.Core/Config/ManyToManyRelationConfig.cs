using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class ManyToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public ManyToManyRelationConfig(IConfig config, RelationFlags flags, string identifier, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, IPropertyAccessor<T, ICollection<TRelation>> accessor)
            : base(config, flags, identifier, leftTable, mappingTable, rightTable, accessor)
        {

        }

        public override IRelationConfig AutoExpression()
        {
            var left = this.Expression.SetLeft(this.CreateConstraint());
            var right = this.Expression.SetRight(this.CreateConstraint());
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns) && this.LeftTable.PrimaryKey != null)
            {
                var column = default(IColumnConfig);
                left.Left = this.Expression.CreateColumn(this.LeftTable.PrimaryKey);
                if (this.MappingTable.TryCreateColumn(
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
                    left.Right = this.Expression.CreateColumn(column);
                    column.IsForeignKey = true;
                }
            }
            this.Expression.Operator = this.Expression.CreateOperator(QueryOperator.AndAlso);
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns) && this.RightTable.PrimaryKey != null)
            {
                var column = default(IColumnConfig);
                right.Left = this.Expression.CreateColumn(this.RightTable.PrimaryKey);
                if (this.MappingTable.TryCreateColumn(
                    ColumnConfig.By(
                        Conventions.RelationColumn(this.RightTable),
                        Factories.Type.Create(
                            TypeConfig.By(
                                this.RightTable.PrimaryKey.ColumnType.Type,
                                this.RightTable.PrimaryKey.ColumnType.Size,
                                this.RightTable.PrimaryKey.ColumnType.Precision,
                                this.RightTable.PrimaryKey.ColumnType.Scale,
                                this.Flags.HasFlag(RelationFlags.AllowNull)
                            )
                        )
                    ),
                    out column
                ))
                {
                    right.Right = this.Expression.CreateColumn(column);
                    column.IsForeignKey = true;
                }
            }
            return this;
        }
    }
}