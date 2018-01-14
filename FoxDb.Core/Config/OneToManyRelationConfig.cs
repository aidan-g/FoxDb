using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class OneToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public OneToManyRelationConfig(IConfig config, RelationFlags flags, ITableConfig leftTable, ITableConfig rightTable, PropertyInfo property, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, flags, leftTable, null, rightTable, property, getter, setter)
        {

        }

        public override IRelationConfig AutoColumns()
        {
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                this.LeftColumn = this.LeftTable.PrimaryKey;
            }
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                var column = default(IColumnConfig);
                if (this.RightTable.TryCreateColumn(ColumnConfig.By(Conventions.RelationColumn(this.LeftTable), Defaults.Column.Flags), out column))
                {
                    this.RightColumn = column;
                    this.RightColumn.IsForeignKey = true;
                }
            }
            return this;
        }

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
