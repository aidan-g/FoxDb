using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public abstract class TableConfig : ITableConfig
    {
        private TableConfig()
        {
            this.Columns = new ConcurrentDictionary<string, IColumnConfig>(StringComparer.OrdinalIgnoreCase);
            this.Indexes = new ConcurrentDictionary<string, IIndexConfig>(StringComparer.OrdinalIgnoreCase);
            this.Relations = new ConcurrentDictionary<string, IRelationConfig>(StringComparer.OrdinalIgnoreCase);
        }

        protected TableConfig(IConfig config, TableFlags flags, string identifier, string tableName, Type tableType)
            : this()
        {
            this.Config = config;
            this.Flags = flags;
            this.Identifier = identifier;
            this.TableName = tableName;
            this.TableType = tableType;
        }

        protected TableConfig(TableConfig table, Type tableType)
        {
            this.Config = table.Config;
            this.Flags = table.Flags;
            this.Identifier = table.Identifier;
            this.TableName = table.TableName;
            this.TableType = tableType;
            this.Columns = new ConcurrentDictionary<string, IColumnConfig>(table.Columns, StringComparer.OrdinalIgnoreCase);
            this.Indexes = new ConcurrentDictionary<string, IIndexConfig>(table.Indexes, StringComparer.OrdinalIgnoreCase);
            this.Relations = new ConcurrentDictionary<string, IRelationConfig>(table.Relations, StringComparer.OrdinalIgnoreCase);
        }

        public IConfig Config { get; private set; }

        public TableFlags Flags { get; protected set; }

        public string Identifier { get; private set; }

        public string TableName { get; set; }

        public Type TableType { get; private set; }

        protected virtual ConcurrentDictionary<string, IColumnConfig> Columns { get; private set; }

        protected virtual ConcurrentDictionary<string, IIndexConfig> Indexes { get; private set; }

        protected virtual ConcurrentDictionary<string, IRelationConfig> Relations { get; private set; }

        IEnumerable<IColumnConfig> ITableConfig.Columns
        {
            get
            {
                return this.Columns.Values;
            }
        }

        public IEnumerable<IColumnConfig> InsertableColumns
        {
            get
            {
                return this.Columns.Values
                    .Except(this.RemoteGeneratedColumns)
                    .Except(this.ConcurrencyColumns)
                    .ToLookup(column => column.ColumnName)
                    .Select(columns => columns.First());
            }
        }

        public IEnumerable<IColumnConfig> UpdatableColumns
        {
            get
            {
                return this.Columns.Values
                    .Except(this.PrimaryKeys)
                    .Except(this.LocalGeneratedColumns)
                    .Except(this.RemoteGeneratedColumns)
                    .Except(this.ConcurrencyColumns)
                    .Distinct();
            }
        }

        public IEnumerable<IColumnConfig> LocalGeneratedColumns
        {
            get
            {
                return this.Columns.Values
                    .Where(column => column.Flags.HasFlag(ColumnFlags.Generated) && !column.ColumnType.IsNumeric);
            }
        }

        public IEnumerable<IColumnConfig> RemoteGeneratedColumns
        {
            get
            {
                return this.Columns.Values
                    .Where(column => column.Flags.HasFlag(ColumnFlags.Generated) && column.ColumnType.IsNumeric);
            }
        }

        public IEnumerable<IColumnConfig> ConcurrencyColumns
        {
            get
            {
                return this.Columns.Values
                    .Where(column => column.IsConcurrencyCheck);
            }
        }

        IEnumerable<IIndexConfig> ITableConfig.Indexes
        {
            get
            {
                return this.Indexes.Values;
            }
        }

        IEnumerable<IRelationConfig> ITableConfig.Relations
        {
            get
            {
                return this.Relations.Values;
            }
        }

        public IColumnConfig PrimaryKey
        {
            get
            {
                return this.PrimaryKeys.SingleOrDefault();
            }
        }

        public IEnumerable<IColumnConfig> PrimaryKeys
        {
            get
            {
                return this.Columns.Values.Where(column => column.IsPrimaryKey);
            }
        }

        public IColumnConfig ForeignKey
        {
            get
            {
                return this.ForeignKeys.SingleOrDefault();
            }
        }

        public IEnumerable<IColumnConfig> ForeignKeys
        {
            get
            {
                return this.Columns.Values.Where(column => column.IsForeignKey);
            }
        }

        public IColumnConfig GetColumn(IColumnSelector selector)
        {
            var existing = default(IColumnConfig);
            var column = Factories.Column.Create(this, selector);
            if (!this.Columns.TryGetValue(column.Identifier, out existing) || !ColumnComparer.ColumnConfig.Equals(column, existing))
            {
                return default(IColumnConfig);
            }
            return existing;
        }

        public IColumnConfig CreateColumn(IColumnSelector selector)
        {
            var column = Factories.Column.Create(this, selector);
            if (!ColumnValidator.Validate(this.Config.Database, this, column))
            {
                throw new InvalidOperationException(string.Format("Table has invalid configuration: {0}", column));
            }
            column = this.Columns.AddOrUpdate(column.Identifier, column);
            return column;
        }

        public bool TryCreateColumn(IColumnSelector selector, out IColumnConfig column)
        {
            column = Factories.Column.Create(this, selector);
            if (!ColumnValidator.Validate(this.Config.Database, this, column))
            {
                return false;
            }
            column = this.Columns.AddOrUpdate(column.Identifier, column);
            return true;
        }

        public IIndexConfig GetIndex(IIndexSelector selector)
        {
            var existing = default(IIndexConfig);
            var index = Factories.Index.Create(this, selector);
            if (!this.Indexes.TryGetValue(index.Identifier, out existing) || !IndexComparer.IndexConfig.Equals(index, existing))
            {
                return default(IIndexConfig);
            }
            return existing;
        }

        public IIndexConfig CreateIndex(IIndexSelector selector)
        {
            var index = Factories.Index.Create(this, selector);
            index = this.Indexes.AddOrUpdate(index.Identifier, index);
            return index;
        }

        public abstract ITableConfig AutoColumns();

        public abstract ITableConfig AutoIndexes();

        public abstract ITableConfig AutoRelations();

        public abstract ITableConfig Extern();

        public abstract ITableConfig<T> CreateProxy<T>();

        public override string ToString()
        {
            return string.Format("{0}", this.TableName);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                if (!string.IsNullOrEmpty(this.TableName))
                {
                    hashCode += this.TableName.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is ITableConfig)
            {
                return this.Equals(obj as ITableConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(ITableConfig other)
        {
            if (other == null)
            {
                return false;
            }
            if (!string.Equals(this.TableName, other.TableName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(TableConfig a, TableConfig b)
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

        public static bool operator !=(TableConfig a, TableConfig b)
        {
            return !(a == b);
        }

        public static ITableSelector By(string tableName, TableFlags? flags = null)
        {
            return By(string.Empty, tableName, flags);
        }

        public static ITableSelector By(string identifier, string tableName, TableFlags? flags = null)
        {
            return TableSelector.By(identifier, tableName, flags);
        }

        public static ITableSelector By(Type tableType, TableFlags? flags = null)
        {
            return By(string.Empty, tableType, flags);
        }

        public static ITableSelector By(string identifier, Type tableType, TableFlags? flags = null)
        {
            return TableSelector.By(identifier, tableType, flags);
        }

        public static ITableSelector By(ITableConfig leftTable, ITableConfig rightTable, TableFlags? flags = null)
        {
            return By(string.Empty, leftTable, rightTable, flags);
        }

        public static ITableSelector By(string identifier, ITableConfig leftTable, ITableConfig rightTable, TableFlags? flags = null)
        {
            return TableSelector.By(identifier, leftTable, rightTable, flags);
        }
    }

    public class TableConfig<T> : TableConfig, ITableConfig<T>
    {
        public TableConfig(IConfig config, TableFlags flags, string identifier, string name)
            : base(config, flags, identifier, name, typeof(T))
        {

        }

        protected TableConfig(TableConfig table)
            : base(table, typeof(T))
        {

        }

        public IRelationConfig GetRelation<TRelation>(IRelationSelector<T, TRelation> selector)
        {
            var existing = default(IRelationConfig);
            var relation = Factories.Relation.Create(this, selector);
            if (!this.Relations.TryGetValue(relation.Identifier, out existing) || !relation.Equals(existing))
            {
                return default(IRelationConfig);
            }
            return existing;
        }

        public IRelationConfig CreateRelation<TRelation>(IRelationSelector<T, TRelation> selector)
        {
            var relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(this.Config.Database, false, relation))
            {
                throw new InvalidOperationException(string.Format("Relation has invalid configuration: {0}", relation));
            }
            relation = this.Relations.AddOrUpdate(relation.Identifier, relation);
            return relation;
        }

        public bool TryCreateRelation<TRelation>(IRelationSelector<T, TRelation> selector, out IRelationConfig relation)
        {
            relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(this.Config.Database, false, relation))
            {
                return false;
            }
            relation = this.Relations.AddOrUpdate(relation.Identifier, relation);
            return true;
        }

        public override ITableConfig AutoColumns()
        {
            var enumerator = new ColumnEnumerator();
            var columns = enumerator.GetColumns(this.Config.Database, this).ToArray();
            for (var a = 0; a < columns.Length; a++)
            {
                var column = columns[a];
                if (this.Columns.ContainsKey(column.Identifier))
                {
                    continue;
                }
                this.Columns.GetOrAdd(column.Identifier, column);
            }
            return this;
        }

        public override ITableConfig AutoIndexes()
        {
            var enumerator = new IndexEnumerator();
            var indexes = enumerator.GetIndexes(this).ToArray();
            for (var a = 0; a < indexes.Length; a++)
            {
                var index = indexes[a];
                if (this.Indexes.ContainsKey(index.Identifier))
                {
                    continue;
                }
                index = this.Indexes.GetOrAdd(index.Identifier, index);
            }
            return this;
        }

        public override ITableConfig AutoRelations()
        {
            var enumerator = new RelationEnumerator();
            var relations = enumerator.GetRelations(this.Config.Database, this).ToArray();
            for (var a = 0; a < relations.Length; a++)
            {
                var relation = relations[a];
                if (this.Relations.ContainsKey(relation.Identifier))
                {
                    continue;
                }
                relation = this.Relations.GetOrAdd(relation.Identifier, relation);
            }
            return this;
        }

        public override ITableConfig Extern()
        {
            return new TableConfig<T>(this.Config, this.Flags | TableFlags.Extern, Unique.New, this.TableName);
        }

        public override ITableConfig<TElement> CreateProxy<TElement>()
        {
            return new TableConfig<TElement>(this);
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            unchecked
            {
                if (!string.IsNullOrEmpty(this.Identifier))
                {
                    hashCode += this.Identifier.GetHashCode();
                }
                if (!string.IsNullOrEmpty(this.TableName))
                {
                    hashCode += this.TableName.GetHashCode();
                }
                if (this.TableType != null)
                {
                    hashCode += this.TableType.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is ITableConfig<T>)
            {
                return this.Equals(obj as ITableConfig<T>);
            }
            return base.Equals(obj);
        }

        public bool Equals(ITableConfig<T> other)
        {
            return base.Equals(other);
        }

        public static bool operator ==(TableConfig<T> a, TableConfig<T> b)
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

        public static bool operator !=(TableConfig<T> a, TableConfig<T> b)
        {
            return !(a == b);
        }
    }

    public class TableConfig<T1, T2> : TableConfig, ITableConfig<T1, T2>
    {
        public TableConfig(IConfig config, TableFlags flags, string identifier, string tableName, ITableConfig<T1> leftTable, ITableConfig<T2> rightTable)
            : base(config, flags, identifier, tableName, typeof(T2))
        {
            this.LeftTable = leftTable;
            this.RightTable = rightTable;
        }

        public IRelationConfig GetRelation<TRelation>(IRelationSelector<T1, TRelation> selector)
        {
            var existing = default(IRelationConfig);
            var relation = Factories.Relation.Create(this, selector);
            if (!this.Relations.TryGetValue(relation.Identifier, out existing) || !relation.Equals(existing))
            {
                return default(IRelationConfig);
            }
            return existing;
        }

        public IRelationConfig CreateRelation<TRelation>(IRelationSelector<T1, TRelation> selector)
        {
            var relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(this.Config.Database, false, relation))
            {
                throw new InvalidOperationException(string.Format("Relation has invalid configuration: {0}", relation));
            }
            relation = this.Relations.AddOrUpdate(relation.Identifier, relation);
            return relation;
        }

        public bool TryCreateRelation<TRelation>(IRelationSelector<T1, TRelation> selector, out IRelationConfig relation)
        {
            relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(this.Config.Database, false, relation))
            {
                return false;
            }
            relation = this.Relations.AddOrUpdate(relation.Identifier, relation);
            return true;
        }

        public override ITableConfig AutoColumns()
        {
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                var column = default(IColumnConfig);
                if (this.TryCreateColumn(ColumnConfig.By(Conventions.RelationColumn(this.LeftTable), this.LeftTable.PrimaryKey.ColumnType), out column))
                {
                    this.LeftForeignKey = column;
                    this.LeftForeignKey.IsForeignKey = true;
                }
            }
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                var column = default(IColumnConfig);
                if (this.TryCreateColumn(ColumnConfig.By(Conventions.RelationColumn(this.RightTable), this.RightTable.PrimaryKey.ColumnType), out column))
                {
                    this.RightForeignKey = column;
                    this.RightForeignKey.IsForeignKey = true;
                }
            }
            return this;
        }

        public override ITableConfig AutoIndexes()
        {
            var enumerator = new IndexEnumerator();
            var indexes = enumerator.GetIndexes(this).ToArray();
            for (var a = 0; a < indexes.Length; a++)
            {
                var index = indexes[a];
                if (this.Indexes.ContainsKey(index.Identifier))
                {
                    continue;
                }
                index = this.Indexes.GetOrAdd(index.Identifier, index);
            }
            return this;
        }

        public override ITableConfig AutoRelations()
        {
            var enumerator = new RelationEnumerator();
            var relations = enumerator.GetRelations(this.Config.Database, this).ToArray();
            for (var a = 0; a < relations.Length; a++)
            {
                var relation = relations[a];
                if (this.Relations.ContainsKey(relation.Identifier))
                {
                    continue;
                }
                relation = this.Relations.GetOrAdd(relation.Identifier, relation);
            }
            return this;
        }

        public override ITableConfig Extern()
        {
            throw new NotImplementedException();
        }

        public override ITableConfig<T> CreateProxy<T>()
        {
            throw new NotImplementedException();
        }

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public IColumnConfig LeftForeignKey { get; set; }

        public IColumnConfig RightForeignKey { get; set; }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            unchecked
            {
                if (this.LeftTable != null)
                {
                    hashCode += this.LeftTable.GetHashCode();
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
            if (obj is ITableConfig)
            {
                return this.Equals(obj as ITableConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(ITableConfig<T1, T2> other)
        {
            return base.Equals(other);
        }

        public static bool operator ==(TableConfig<T1, T2> a, TableConfig<T1, T2> b)
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

        public static bool operator !=(TableConfig<T1, T2> a, TableConfig<T1, T2> b)
        {
            return !(a == b);
        }
    }
}
