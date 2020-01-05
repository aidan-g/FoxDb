using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class ColumnConfig : IColumnConfig
    {
        public ColumnConfig(IConfig config, ColumnFlags flags, string identifier, ITableConfig table, string columnName, ITypeConfig columnType, PropertyInfo property, Func<object, object> getter, Action<object, object> setter, Action<object> incrementor)
        {
            this.Config = config;
            this._Flags = flags;
            this.Identifier = identifier;
            this.Table = table;
            this._ColumnName = columnName;
            this._ColumnType = columnType;
            this.Property = property;
            this.Getter = getter;
            this.Setter = setter;
            this.Incrementor = incrementor;
        }

        public IConfig Config { get; private set; }

        private ColumnFlags _Flags { get; set; }

        public ColumnFlags Flags
        {
            get
            {
                return this._Flags;
            }
            set
            {
                if (this.Flags == value)
                {
                    return;
                }
                this._Flags = value;
                this.Table.Reset();
            }
        }

        public string Identifier { get; private set; }

        public ITableConfig Table { get; private set; }

        private string _ColumnName { get; set; }

        public string ColumnName
        {
            get
            {
                return this._ColumnName;
            }
            set
            {
                if (string.Equals(this.ColumnName, value))
                {
                    return;
                }
                this._ColumnName = value;
                this.Table.Reset();
            }
        }

        private ITypeConfig _ColumnType { get; set; }

        public ITypeConfig ColumnType
        {
            get
            {
                return this._ColumnType;
            }
            set
            {
                if (TypeConfig.Equals(this.ColumnType, value))
                {
                    return;
                }
                this._ColumnType = value;
                this.Table.Reset();
            }
        }

        public PropertyInfo Property { get; set; }

        private bool _IsPrimaryKey { get; set; }

        public bool IsPrimaryKey
        {
            get
            {
                return this._IsPrimaryKey;
            }
            set
            {
                if (this.IsPrimaryKey == value)
                {
                    return;
                }
                this._IsPrimaryKey = value;
                this.Table.Reset();
            }
        }

        private bool _IsForeignKey { get; set; }

        public bool IsForeignKey
        {
            get
            {
                return this._IsForeignKey;
            }
            set
            {
                if (this.IsForeignKey == value)
                {
                    return;
                }
                this._IsForeignKey = value;
                this.Table.Reset();
            }
        }

        public bool IsConcurrencyCheck
        {
            get
            {
                return this.Flags.HasFlag(ColumnFlags.ConcurrencyCheck);
            }
        }

        public object DefaultValue
        {
            get
            {
                return this.ColumnType.DefaultValue;
            }
        }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }

        public Action<object> Incrementor { get; set; }

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

        public static IColumnSelector By(string columnName, ColumnFlags? flags = null)
        {
            return By(string.Empty, columnName, null, flags);
        }

        public static IColumnSelector By(string identifier, string columnName, ColumnFlags? flags = null)
        {
            return By(identifier, columnName, null, flags);
        }

        public static IColumnSelector By(string columnName, ITypeConfig columnType, ColumnFlags? flags = null)
        {
            return By(string.Empty, columnName, columnType, flags);
        }

        public static IColumnSelector By(string identifier, string columnName, ITypeConfig columnType, ColumnFlags? flags = null)
        {
            return ColumnSelector.By(identifier, columnName, columnType, flags);
        }

        public static IColumnSelector By(PropertyInfo property, ColumnFlags? flags = null)
        {
            return By(string.Empty, property, flags);
        }

        public static IColumnSelector By(string identifier, PropertyInfo property, ColumnFlags? flags = null)
        {
            return ColumnSelector.By(identifier, property, flags);
        }

        public static IColumnSelector By<T, TColumn>(Expression<Func<T, TColumn>> expression, ColumnFlags? flags = null)
        {
            return By<T, TColumn>(string.Empty, expression, flags);
        }

        public static IColumnSelector By<T, TColumn>(string identifier, Expression<Func<T, TColumn>> expression, ColumnFlags? flags = null)
        {
            return ColumnSelector.By(identifier, expression, flags);
        }
    }
}
