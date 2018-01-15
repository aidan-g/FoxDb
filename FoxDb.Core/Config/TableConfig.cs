using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public abstract class TableConfig : ITableConfig
    {
        protected TableConfig(IConfig config, TableFlags flags, string tableName, Type tableType)
        {
            this.Config = config;
            this.Flags = flags;
            this.TableName = tableName;
            this.TableType = tableType;
            this.Columns = new Dictionary<string, IColumnConfig>();
            this.Relations = new Dictionary<Type, IRelationConfig>();
        }

        public IConfig Config { get; private set; }

        public TableFlags Flags { get; private set; }

        public string TableName { get; set; }

        public Type TableType { get; private set; }

        protected virtual IDictionary<string, IColumnConfig> Columns { get; private set; }

        protected virtual IDictionary<Type, IRelationConfig> Relations { get; private set; }

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
            return this.Columns[column.ColumnName];
        }

        public IColumnConfig CreateColumn(IColumnSelector selector)
        {
            var column = Factories.Column.Create(this, selector);
            if (!ColumnValidator.Validate(column))
            {
                throw new InvalidOperationException("Column configuration is not valid.");
            }
            this.Columns[column.ColumnName] = column;
            return column;
        }

        public bool TryCreateColumn(IColumnSelector selector, out IColumnConfig column)
        {
            column = Factories.Column.Create(this, selector);
            if (!ColumnValidator.Validate(column))
            {
                return false;
            }
            this.Columns[column.ColumnName] = column;
            return true;
        }

        public abstract ITableConfig AutoColumns();

        public abstract ITableConfig AutoRelations();

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
    }

    public class TableConfig<T> : TableConfig, ITableConfig<T>
    {
        public TableConfig(IConfig config, TableFlags flags, string name) : base(config, flags, name, typeof(T))
        {

        }

        public IRelationConfig GetRelation<TRelation>(IRelationSelector<T, TRelation> selector)
        {
            var relation = Factories.Relation.Create(this, selector);
            return this.Relations[relation.RelationType];
        }

        public IRelationConfig CreateRelation<TRelation>(IRelationSelector<T, TRelation> selector)
        {
            var relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(false, relation))
            {
                throw new InvalidOperationException("Relation configuration is not valid.");
            }
            this.Relations[relation.RelationType] = relation;
            return relation;
        }

        public bool TryCreateRelation<TRelation>(IRelationSelector<T, TRelation> selector, out IRelationConfig relation)
        {
            relation = Factories.Relation.Create(this, selector);
            if (!RelationValidator.Validate(false, relation))
            {
                return false;
            }
            this.Relations[relation.RelationType] = relation;
            return true;
        }

        public override ITableConfig AutoColumns()
        {
            var enumerator = new ColumnEnumerator();
            foreach (var column in enumerator.GetColumns(this))
            {
                if (this.Columns.ContainsKey(column.ColumnName))
                {
                    continue;
                }
                this.Columns.Add(column.ColumnName, column);
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
            foreach (var relation in enumerator.GetRelations(this))
            {
                if (this.Relations.ContainsKey(relation.RelationType))
                {
                    continue;
                }
                this.Relations.Add(relation.RelationType, relation);
            }
            return this;
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

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public IColumnConfig LeftForeignKey { get; set; }

        public IColumnConfig RightForeignKey { get; set; }
    }
}
