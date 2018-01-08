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
        protected TableConfig(IConfig config, string tableName, Type tableType)
        {
            this.Config = config;
            this.TableName = tableName;
            this.TableType = tableType;
            this.Columns = new Dictionary<string, IColumnConfig>();
            this.Relations = new Dictionary<Type, IRelationConfig>();
        }

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
        public TableConfig(IConfig config, string name) : base(config, name, typeof(T))
        {

        }

        public ITableConfig<T> UseDefaultColumns()
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

        public ITableConfig<T> UseDefaultRelations()
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

        public IRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, TRelation>> expression)
        {
            if (typeof(TRelation).IsGenericType)
            {
                throw new NotImplementedException();
            }
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var relation = Factories.Relation.Create(this, expression);
                this.Relations.Add(key, relation);
            }
            return this.Relations[key] as IRelationConfig<T, TRelation>;
        }

        public ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, ICollection<TRelation>>> expression, RelationMultiplicity multiplicity)
        {
            var key = typeof(TRelation);
            if (!this.Relations.ContainsKey(key))
            {
                var relation = Factories.Relation.Create(this, expression, multiplicity);
                this.Relations.Add(key, relation);
            }
            return this.Relations[key] as ICollectionRelationConfig<T, TRelation>;
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
