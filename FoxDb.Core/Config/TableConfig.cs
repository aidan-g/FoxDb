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
                if (!property.PropertyType.IsScalar())
                {
                    continue;
                }
                if (!this.Database.Schema.GetColumnNames<T>().Contains(property.Name))
                {
                    continue;
                }
                this.TryCreateColumn(property);
            }
            return this;
        }

        protected virtual void TryCreateColumn(PropertyInfo property)
        {
            var column = this.Column(property.Name);
            if (string.Equals(column.ColumnName, Conventions.KeyColumn, StringComparison.OrdinalIgnoreCase))
            {
                column.IsPrimaryKey = true;
            }
        }

        public ITableConfig<T> UseDefaultRelations()
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsScalar())
                {
                    continue;
                }
                this.TryCreateRelation(property);
            }
            return this;
        }

        protected virtual void TryCreateRelation(PropertyInfo property)
        {
            //Nothing to do.
        }

        public IRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, TRelation>> expression, ConfigDefaults defaults = ConfigDefaults.Default)
        {
            if (typeof(TRelation).IsGenericType)
            {
                throw new NotImplementedException();
            }
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var accessor = PropertyAccessorFactory.Create<T, TRelation>(expression);
                var config = new RelationConfig<T, TRelation>(this.Database.Config, this, this.Database.Config.Table<TRelation>(), accessor.Get, accessor.Set);
                this.Relations.Add(key, config);
                if (defaults.HasFlag(ConfigDefaults.DefaultColumns))
                {
                    config.UseDefaultColumns();
                }
            }
            return this.Relations[key] as IRelationConfig<T, TRelation>;
        }

        public ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, ICollection<TRelation>>> expression, RelationMultiplicity multiplicity, ConfigDefaults defaults = ConfigDefaults.Default)
        {
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var accessor = PropertyAccessorFactory.Create<T, ICollection<TRelation>>(expression);
                var collectionFactory = EntityCollectionFactory<TRelation>.Create(accessor.PropertyType);
                var config = default(ICollectionRelationConfig<T, TRelation>);
                switch (multiplicity)
                {
                    case RelationMultiplicity.OneToMany:
                        config = new OneToManyRelationConfig<T, TRelation>(this.Database.Config, this, this.Database.Config.Table<TRelation>(), collectionFactory, accessor.Get, accessor.Set);
                        break;
                    case RelationMultiplicity.ManyToMany:
                        config = new ManyToManyRelationConfig<T, TRelation>(this.Database.Config, this, this.Database.Config.Table<T, TRelation>(), this.Database.Config.Table<TRelation>(), collectionFactory, accessor.Get, accessor.Set);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                this.Relations.Add(typeof(TRelation), config);
                if (defaults.HasFlag(ConfigDefaults.DefaultColumns))
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
            this.LeftTable = database.Config.Table<T1>();
            this.RightTable = database.Config.Table<T2>();
        }

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

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
