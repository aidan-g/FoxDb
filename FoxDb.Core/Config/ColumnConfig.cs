using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public class ColumnConfig : IColumnConfig
    {
        public ColumnConfig(IConfig config, ColumnFlags flags, ITableConfig table, string columnName, PropertyInfo property, Func<object, object> getter, Action<object, object> setter)
        {
            this.Config = config;
            this.Flags = flags;
            this.Table = table;
            this.ColumnName = columnName;
            this.Property = property;
            this.Getter = getter;
            this.Setter = setter;
        }

        public IConfig Config { get; private set; }

        public ColumnFlags Flags { get; private set; }

        public ITableConfig Table { get; private set; }

        public string Identifier
        {
            get
            {
                return string.Format("{0}_{1}", this.Table.TableName, this.ColumnName);
            }
        }

        public string ColumnName { get; set; }

        public PropertyInfo Property { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }

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
            if (this.Table != other.Table)
            {
                return false;
            }
            if (!string.Equals(this.ColumnName, other.ColumnName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (this.Property != other.Property)
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
            return ColumnSelector.By(columnName, flags);
        }

        public static IColumnSelector By(PropertyInfo property, ColumnFlags flags)
        {
            return ColumnSelector.By(property, flags);
        }
    }
}
