using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class IndexConfig : IIndexConfig
    {
        private IndexConfig(IConfig config)
        {
            this.Config = config;
            this.Builder = FragmentBuilder.GetProxy(null);
            this.Expression = this.CreateConstraint();
        }

        public IndexConfig(IConfig config, IndexFlags flags, string identifier, ITableConfig table, string indexName, IEnumerable<IColumnConfig> columns) : this(config)
        {
            this.Flags = flags;
            this.Identifier = identifier;
            this.Table = table;
            this.IndexName = indexName;
            this.Columns = columns;
        }

        public IConfig Config { get; private set; }

        public IFragmentBuilder Builder { get; private set; }

        public IndexFlags Flags { get; private set; }

        public string Identifier { get; private set; }

        public ITableConfig Table { get; private set; }

        public string IndexName { get; set; }

        public IEnumerable<IColumnConfig> Columns { get; set; }

        public IBinaryExpressionBuilder Expression { get; set; }

        public virtual IBinaryExpressionBuilder CreateConstraint()
        {
            return this.Fragment<IBinaryExpressionBuilder>();
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

        public override string ToString()
        {
            return string.Format("{0}()", this.Table, string.Join(", ", this.Columns));
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.Table.GetHashCode();
                foreach (var column in this.Columns)
                {
                    hashCode += column.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IIndexConfig)
            {
                return this.Equals(obj as IIndexConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(IIndexConfig other)
        {
            if (other == null)
            {
                return false;
            }
            if ((TableConfig)this.Table != (TableConfig)other.Table)
            {
                return false;
            }
            if (!Enumerable.SequenceEqual(this.Columns, other.Columns))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(IndexConfig a, IndexConfig b)
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

        public static bool operator !=(IndexConfig a, IndexConfig b)
        {
            return !(a == b);
        }

        public static IIndexSelector By(string indexName, IEnumerable<IColumnConfig> columns, IndexFlags flags)
        {
            return By(string.Empty, indexName, columns, flags);
        }

        public static IIndexSelector By(string identifier, string indexName, IEnumerable<IColumnConfig> columns, IndexFlags flags)
        {
            return IndexSelector.By(identifier, indexName, columns, flags);
        }

        public static IIndexSelector By(string indexName, IEnumerable<string> columnNames, IndexFlags flags)
        {
            return By(string.Empty, indexName, columnNames, flags);
        }

        public static IIndexSelector By(string identifier, string indexName, IEnumerable<string> columnNames, IndexFlags flags)
        {
            return IndexSelector.By(identifier, indexName, columnNames, flags);
        }
    }
}
