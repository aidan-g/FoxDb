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
        private TableConfig()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        protected TableConfig(IConfig config, string tableName, Type tableType) : this()
        {
            this.Config = config;
            this.TableName = tableName;
            this.TableType = tableType;
            this.Columns = new Dictionary<string, IColumnConfig>();
            this.Relations = new Dictionary<Type, IRelationConfig>();
        }

        protected DynamicMethod Members { get; private set; }

        public IConfig Config { get; private set; }

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
                column = this.CreateColumn(name);
                this.Columns[column.ColumnName] = column;
            }
            return column;
        }

        protected virtual IColumnConfig CreateColumn(string name)
        {
            return Factories.Column.Create(this, name);
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
                var column = this.CreateColumn(property);
                this.Columns[column.ColumnName] = column;
                return column;
            }
        }

        protected virtual IColumnConfig CreateColumn(PropertyInfo property)
        {
            return Factories.Column.Create(this, property);
        }
    }

    public class TableConfig<T> : TableConfig, ITableConfig<T>
    {
        public TableConfig(IConfig config, string name) : base(config, name, typeof(T))
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
                var column = this.CreateColumn(property);
                if (this.Columns.ContainsKey(column.ColumnName))
                {
                    continue;
                }
                if (!this.Config.Database.Schema.GetColumnNames(this.TableName).Contains(column.ColumnName))
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

        public ITableConfig<T> UseDefaultRelations()
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsScalar())
                {
                    continue;
                }
                if (!this.Config.Database.Schema.GetTableNames().Contains(Conventions.TableName(property.PropertyType)))
                {
                    continue;
                }
                this.Relation(property);
            }
            return this;
        }

        protected virtual IRelationConfig Relation(PropertyInfo property)
        {
            var elementType = default(Type);
            if (property.PropertyType.IsCollection(out elementType))
            {
                return (IRelationConfig)this.Members.Invoke(this, "Relation", elementType, PropertyAccessorFactory.Create(property));
            }
            else
            {
                return (IRelationConfig)this.Members.Invoke(this, "Relation", property.PropertyType, PropertyAccessorFactory.Create(property));
            }
        }

        public IRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, TRelation>> expression)
        {
            if (typeof(TRelation).IsGenericType)
            {
                throw new NotImplementedException();
            }
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var relation = this.CreateRelation(expression);
                this.Relations.Add(key, relation);
            }
            return this.Relations[key] as IRelationConfig<T, TRelation>;
        }

        protected virtual IRelationConfig<T, TRelation> CreateRelation<TRelation>(Expression<Func<T, TRelation>> expression)
        {
            return Factories.Relation.Create<T, TRelation>(this, expression);
        }

        public ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, ICollection<TRelation>>> expression, RelationMultiplicity multiplicity)
        {
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var relation = this.CreateRelation(expression, multiplicity);
                this.Relations.Add(key, relation);
            }
            return this.Relations[key] as ICollectionRelationConfig<T, TRelation>;
        }

        protected virtual ICollectionRelationConfig<T, TRelation> CreateRelation<TRelation>(Expression<Func<T, ICollection<TRelation>>> expression, RelationMultiplicity multiplicity)
        {
            return Factories.Relation.Create<T, TRelation>(this, expression, multiplicity);
        }
    }

    public class TableConfig<T1, T2> : TableConfig, ITableConfig<T1, T2>
    {
        public TableConfig(IConfig config) : base(config, Conventions.RelationTableName(typeof(T1), typeof(T2)), typeof(T2))
        {
            this.LeftTable = config.Table<T1>();
            this.RightTable = config.Table<T2>();
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
