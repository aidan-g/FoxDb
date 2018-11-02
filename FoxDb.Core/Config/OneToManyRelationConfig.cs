using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class OneToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public OneToManyRelationConfig(IConfig config, RelationFlags flags, string identifier, ITableConfig leftTable, ITableConfig rightTable, IPropertyAccessor<T, ICollection<TRelation>> accessor)
            : base(config, flags, identifier, leftTable, null, rightTable, accessor)
        {

        }

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
    }
}
