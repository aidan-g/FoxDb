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
            this.Columns = new ConcurrentDictionary<string, IColumnConfig>();
            this.Relations = new ConcurrentDictionary<Type, IRelationConfig>();
        }

        protected TableConfig(IConfig config, TableFlags flags, string tableName, Type tableType) : this()
        {
            this.Config = config;
            this.Flags = flags;
            this.TableName = tableName;
            this.TableType = tableType;
        }

        protected TableConfig(TableConfig table, Type tableType)
        {
            this.Config = table.Config;
            this.Flags = table.Flags;
            this.TableName = table.TableName;
            this.TableType = tableType;
            this.Columns = table.Columns;
            this.Relations = table.Relations;
        }

        public IConfig Config { get; private set; }

        public TableFlags Flags { get; private set; }

        public string TableName { get; set; }

        public Type TableType { get; private set; }

        protected virtual ConcurrentDictionary<string, IColumnConfig> Columns { get; private set; }

        protected virtual ConcurrentDictionary<Type, IRelationConfig> Relations { get; private set; }

        IEnumerable<IColumnConfig> ITableConfig.Columns
        {
            get
            {
                return this.Columns.Values;
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
            var column = Factories.Column.Create(this, selector);
            if (!this.Columns.TryGetValue(column.ColumnName, out column))
            {
                return default(IColumnConfig);
            }
            return column;
        }

        public IColumnConfig CreateColumn(IColumnSelector selector)
        {
            var column = Factories.Column.Create(this, selector);
            if (!ColumnValidator.Validate(column))
            {
                throw new InvalidOperationException(string.Format("Table has invalid configuration: {0}", column));
            }
            column = this.Columns.GetOrAdd(column.ColumnName, column);
            return column;
        }

        public bool TryCreateColumn(IColumnSelector selector, out IColumnConfig column)
        {
            column = Factories.Column.Create(this, selector);
            if (!ColumnValidator.Validate(column))
            {
                return false;
            }
            column = this.Columns.GetOrAdd(column.ColumnName, column);
            return true;
        }

        public abstract ITableConfig AutoColumns();

        public abstract ITableConfig AutoRelations();

        public abstract ITableConfig<T> CreateProxy<T>();

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.TableName.GetHashCode();
                hashCode += this.TableType.GetHashCode();
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
            if (this.TableType != other.TableType)
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

        public static ITableSelector By(Type tableType, TableFlags flags)
        {
            return TableSelector.By(tableType, flags);
        }

        public static ITableSelector By(ITableConfig leftTable, ITableConfig rightTable, TableFlags flags)
        {
            return TableSelector.By(leftTable, rightTable, flags);
        }
    }

    public class TableConfig<T> : TableConfig, ITableConfig<T>
    {
        public TableConfig(IConfig config, TableFlags flags, string name) : base(config, flags, name, typeof(T))
        {

        }

        protected TableConfig(TableConfig table) : base(table, typeof(T))
        {

        }

        public IRelationConfig GetRelation<TRelation>(IRelationSelector<T, TRelation> selector)
        {
            var relation = Factories.Relation.Create(this, selector);
            if (!this.Relations.TryGetValue(relation.RelationType, out relation))
            {
                return default(IRelationConfig);
            }
            return relation;
        }

        public IRelationConfig CreateRelation<TRelation>(IRelationSelector<T, TRelation> selector)
        {
            var relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(false, relation))
            {
                throw new InvalidOperationException(string.Format("Relation has invalid configuration: {0}", relation));
            }
            relation = this.Relations.GetOrAdd(relation.RelationType, relation);
            return relation;
        }

        public bool TryCreateRelation<TRelation>(IRelationSelector<T, TRelation> selector, out IRelationConfig relation)
        {
            relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(false, relation))
            {
                return false;
            }
            relation = this.Relations.GetOrAdd(relation.RelationType, relation);
            return true;
        }

        public override ITableConfig AutoColumns()
        {
            var enumerator = new ColumnEnumerator();
            var columns = enumerator.GetColumns(this).ToArray();
            for (var a = 0; a < columns.Length; a++)
            {
                var column = columns[a];
                if (this.Columns.ContainsKey(column.ColumnName))
                {
                    continue;
                }
                column = this.Columns.GetOrAdd(column.ColumnName, column);
                if (string.Equals(column.ColumnName, Conventions.KeyColumn, StringComparison.OrdinalIgnoreCase))
                {
                    column.IsPrimaryKey = true;
                }
            }
            return this;
        }

        public override ITableConfig AutoRelations()
        {
            var enumerator = new RelationEnumerator();
            var relations = enumerator.GetRelations(this).ToArray();
            for (var a = 0; a < relations.Length; a++)
            {
                var relation = relations[a];
                if (this.Relations.ContainsKey(relation.RelationType))
                {
                    continue;
                }
                relation = this.Relations.GetOrAdd(relation.RelationType, relation);
            }
            return this;
        }

        public override ITableConfig<TElement> CreateProxy<TElement>()
        {
            return new TableConfig<TElement>(this);
        }
    }

    public class TableConfig<T1, T2> : TableConfig, ITableConfig<T1, T2>
    {
        public TableConfig(IConfig config, TableFlags flags, string tableName, ITableConfig<T1> leftTable, ITableConfig<T2> rightTable) : base(config, flags, tableName, typeof(T2))
        {
            this.LeftTable = leftTable;
            this.RightTable = rightTable;
        }

        public override ITableConfig AutoColumns()
        {
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                var column = default(IColumnConfig);
                if (this.TryCreateColumn(ColumnConfig.By(Conventions.RelationColumn(this.LeftTable), Defaults.Column.Flags), out column))
                {
                    this.LeftForeignKey = column;
                    this.LeftForeignKey.IsForeignKey = true;
                }
            }
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                var column = default(IColumnConfig);
                if (this.TryCreateColumn(ColumnConfig.By(Conventions.RelationColumn(this.RightTable), Defaults.Column.Flags), out column))
                {
                    this.RightForeignKey = column;
                    this.RightForeignKey.IsForeignKey = true;
                }
            }
            return this;
        }

        public override ITableConfig AutoRelations()
        {
            return this;
        }

        public override ITableConfig<T> CreateProxy<T>()
        {
            throw new NotImplementedException();
        }

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public IColumnConfig LeftForeignKey { get; set; }

        public IColumnConfig RightForeignKey { get; set; }
    }
}
