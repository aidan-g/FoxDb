using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        public IColumnConfig Column(string name)
        {
            var column = default(IColumnConfig);
            if (!this.Columns.TryGetValue(name, out column))
            {
                column = Factories.Column.Create(this, name);
                this.Columns[column.ColumnName] = column;
            }
            return column;
        }

        public IColumnConfig Column(PropertyInfo property)
        {
            foreach (var column in this.Columns.Values)
            {
                if (column.Property == property)
                {
                    return column;
                }
            }
            {
                var column = Factories.Column.Create(this, property);
                this.Columns[column.ColumnName] = column;
                return column;
            }
        }
    }

    public class TableConfig<T> : TableConfig, ITableConfig<T>
    {
        public TableConfig(IConfig config, TableFlags flags, string name) : base(config, flags, name, typeof(T))
        {
            if (flags.HasFlag(TableFlags.AutoColumns))
            {
                this.AutoColumns();
            }
            if (flags.HasFlag(TableFlags.AutoRelations))
            {
                this.AutoRelations();
            }
        }

        protected virtual ITableConfig<T> AutoColumns()
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

        protected virtual ITableConfig<T> AutoRelations()
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

        public IRelationConfig Relation<TRelation>(Expression<Func<T, TRelation>> expression)
        {
            return this.Relation(expression, Defaults.Relation.Flags);
        }

        public IRelationConfig Relation<TRelation>(Expression<Func<T, TRelation>> expression, RelationFlags flags)
        {
            var relation = Factories.Relation.Create<T, TRelation>(this, expression, flags);
            return this.Relations[relation.RelationType] = relation;
        }
    }

    public class TableConfig<T1, T2> : TableConfig, ITableConfig<T1, T2>
    {
        public TableConfig(IConfig config, TableFlags flags, string tableName, ITableConfig<T1> leftTable, ITableConfig<T2> rightTable) : base(config, flags, tableName, typeof(T2))
        {
            this.LeftTable = leftTable;
            this.RightTable = rightTable;
            if (flags.HasFlag(TableFlags.AutoColumns))
            {
                if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
                {
                    (this.LeftForeignKey = this.Column(Conventions.RelationColumn(this.LeftTable))).IsForeignKey = true;
                }
                if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
                {
                    (this.RightForeignKey = this.Column(Conventions.RelationColumn(this.RightTable))).IsForeignKey = true;
                }
            }
        }

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public IColumnConfig LeftForeignKey { get; set; }

        public IColumnConfig RightForeignKey { get; set; }
    }
}
