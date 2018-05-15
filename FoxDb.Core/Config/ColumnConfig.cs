using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class ColumnConfig : IColumnConfig
    {
        public ColumnConfig(IConfig config, ColumnFlags flags, string identifier, ITableConfig table, string columnName, ITypeConfig columnType, PropertyInfo property, Func<object, object> getter, Action<object, object> setter)
        {
            this.Config = config;
            this.Flags = flags;
            this.Identifier = identifier;
            this.Table = table;
            this.ColumnName = columnName;
            this.ColumnType = columnType;
            this.Property = property;
            this.Getter = getter;
            this.Setter = setter;
        }

        public IConfig Config { get; private set; }

        public ColumnFlags Flags { get; private set; }

        public string Identifier { get; private set; }

        public ITableConfig Table { get; private set; }

        public string ColumnName { get; set; }

        public ITypeConfig ColumnType { get; set; }

        public PropertyInfo Property { get; set; }

        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }

        public object DefaultValue
        {
            get
            {
                if (this.Property != null)
                {
                    return this.Property.PropertyType.DefaultValue();
                }
                throw new NotImplementedException();
            }
        }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Table, this.ColumnName);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.Table.GetHashCode();
                hashCode += this.ColumnName.GetHashCode();
                if (this.Property != null)
                {
                    hashCode += this.Property.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IColumnConfig)
            {
                return this.Equals(obj as IColumnConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(IColumnConfig other)
        {
            if (other == null)
            {
                return false;
            }
            if ((TableConfig)this.Table != (TableConfig)other.Table)
            {
                return false;
            }
            if (!string.Equals(this.ColumnName, other.ColumnName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(ColumnConfig a, ColumnConfig b)
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

        public static bool operator !=(ColumnConfig a, ColumnConfig b)
        {
            return !(a == b);
        }

        public static IColumnSelector By(string columnName, ColumnFlags flags)
        {
            return By(string.Empty, columnName, flags);
        }

        public static IColumnSelector By(string identifier, string columnName, ColumnFlags flags)
        {
            return ColumnSelector.By(identifier, columnName, flags);
        }

        public static IColumnSelector By(PropertyInfo property, ColumnFlags flags)
        {
            return By(string.Empty, property, flags);
        }

        public static IColumnSelector By(string identifier, PropertyInfo property, ColumnFlags flags)
        {
            return ColumnSelector.By(identifier, property, flags);
        }

        public static IColumnSelector By<T, TColumn>(Expression<Func<T, TColumn>> expression, ColumnFlags flags)
        {
            return By<T, TColumn>(string.Empty, expression, flags);
        }

        public static IColumnSelector By<T, TColumn>(string identifier, Expression<Func<T, TColumn>> expression, ColumnFlags flags)
        {
            return ColumnSelector.By(identifier, expression, flags);
        }
    }
}
