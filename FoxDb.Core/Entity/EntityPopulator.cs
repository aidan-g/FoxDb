using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityPopulator<T> : IEntityPopulator<T>
    {
        public static readonly IEntityPopulatorStrategy[] Strategies = new IEntityPopulatorStrategy[]
        {
            new IdentifierEntityPopulatorStrategy(),
            new ColumnNameEntityPopulatorStrategy()
        };

        public EntityPopulator(ITableConfig table, IEntityMapper mapper)
        {
            this.Table = table;
            this.Mapper = mapper;
        }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public void Populate(T item, IDatabaseReaderRecord record)
        {
            foreach (var column in this.Table.Columns)
            {
                foreach (var strategy in Strategies)
                {
                    if (strategy.Populate(item, column, record))
                    {
                        break;
                    }
                }
            }
        }

        protected abstract class EntityPopulatorStrategy : IEntityPopulatorStrategy
        {
            public static readonly IValueUnpackerStrategy Strategy = new NullableValueUnpackerStrategy();

            public bool Populate(object item, IColumnConfig column, IDatabaseReaderRecord record)
            {
                if (column.Setter == null)
                {
                    return false;
                }
                var value = default(object);
                if (!this.TryGetValue(column, record, out value))
                {
                    return false;
                }
                column.Setter(item, Strategy.Unpack(column.Property.PropertyType, value));
                return true;
            }

            protected abstract bool TryGetValue(IColumnConfig column, IDatabaseReaderRecord record, out object value);
        }

        protected class IdentifierEntityPopulatorStrategy : EntityPopulatorStrategy
        {
            protected override bool TryGetValue(IColumnConfig column, IDatabaseReaderRecord record, out object value)
            {
                return record.TryGetValue(column.Identifier, out value);
            }
        }

        protected class ColumnNameEntityPopulatorStrategy : EntityPopulatorStrategy
        {
            protected override bool TryGetValue(IColumnConfig column, IDatabaseReaderRecord record, out object value)
            {
                return record.TryGetValue(column.ColumnName, out value);
            }
        }
    }
}
