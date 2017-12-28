using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityColumnMap : IEntityColumnMap
    {
        public EntityColumnMap(IColumnConfig column, bool createUniqueIdentifier)
        {
            this.Column = column;
            this.CreateUniqueIdentifier = createUniqueIdentifier;
        }

        public IColumnConfig Column { get; private set; }

        public bool CreateUniqueIdentifier { get; private set; }

        public string Identifier
        {
            get
            {
                if (!this.CreateUniqueIdentifier)
                {
                    return this.Column.ColumnName;
                }
                return string.Format("{0}_{1}", this.Column.Table.TableName, this.Column.ColumnName);
            }
        }
    }
}
