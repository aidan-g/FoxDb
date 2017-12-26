using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FoxDb
{
    public abstract class TableConfig : ITableConfig
    {
        protected TableConfig(IDatabase database, string tableName, Type tableType)
        {
            this.Database = database;
            this.TableName = tableName;
            this.TableType = tableType;
            this.Columns = new Dictionary<string, IColumnConfig>();
            this.Relations = new Dictionary<Type, IRelationConfig>();
        }

        public IDatabase Database { get; private set; }

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

        public IColumnConfig Column(string columnName)
        {
            if (!this.Columns.ContainsKey(columnName))
            {
                var column = ColumnFactory.Create(this, columnName);
                this.Columns.Add(columnName, column);
            }
            return this.Columns[columnName];
        }

        public IColumnConfig Column(PropertyInfo property)
        {
            foreach (var column in this.Columns.Values)
            {
                if (string.Equals(column.PropertyName, property.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }
            return this.Column(property.Name);
        }
    }

    public class TableConfig<T> : TableConfig, ITableConfig<T>
    {
        public TableConfig(IDatabase database) : base(database, Conventions.TableName(typeof(T)), typeof(T))
        {

        }

        public ITableConfig<T> UseDefaultColumns()
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (!this.Database.Schema.GetColumnNames<T>().Contains(property.Name))
                {
                    continue;
                }
                var column = this.Column(property.Name);
                if (string.Equals(column.ColumnName, Conventions.KeyColumn, StringComparison.OrdinalIgnoreCase))
                {
                    column.IsPrimaryKey = true;
                }
            }
            return this;
        }

        public IRelationConfig<T, TRelation> Relation<TRelation>(Func<T, TRelation> getter, Action<T, TRelation> setter, bool useDefaultColumns = true)
        {
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var config = new RelationConfig<T, TRelation>(this.Database.Config.Table<TRelation>(), getter, setter);
                this.Relations.Add(key, config);
                if (useDefaultColumns)
                {
                    config.UseDefaultColumns();
                }
            }
            return this.Relations[key] as IRelationConfig<T, TRelation>;
        }

        public ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter, bool useDefaultColumns = true)
        {
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var config = new CollectionRelationConfig<T, TRelation>(this.Database.Config.Table<TRelation>(), getter, setter);
                this.Relations.Add(typeof(TRelation), config);
                if (useDefaultColumns)
                {
                    config.UseDefaultColumns();
                }
            }
            return this.Relations[key] as ICollectionRelationConfig<T, TRelation>;
        }
    }

    public class TableConfig<T1, T2> : TableConfig, ITableConfig<T1, T2>
    {
        public TableConfig(IDatabase database) : base(database, Conventions.RelationTableName(typeof(T1), typeof(T2)), typeof(T2))
        {

        }

        public IColumnConfig LeftForeignKey { get; set; }

        public IColumnConfig RightForeignKey { get; set; }

        public ITableConfig<T1, T2> UseDefaultColumns()
        {
            (this.LeftForeignKey = this.Column(Conventions.RelationColumn(typeof(T1)))).IsForeignKey = true;
            (this.RightForeignKey = this.Column(Conventions.RelationColumn(typeof(T2)))).IsForeignKey = true;
            return this;
        }
    }
}
