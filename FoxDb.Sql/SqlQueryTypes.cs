using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SqlQueryTypes : IDatabaseQueryTypes
    {
        public SqlQueryTypes(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        protected static IDictionary<DbType, SqlQueryTypesHandler> Types = new Dictionary<DbType, SqlQueryTypesHandler>()
        {
            //Integer.
            { DbType.Int16, types => types.DefaultIntegralType },
            { DbType.Int32, types => types.DefaultIntegralType },
            { DbType.Int64, types => types.DefaultIntegralType },
            { DbType.UInt16, types => types.DefaultIntegralType },
            { DbType.UInt32, types => types.DefaultIntegralType },
            { DbType.UInt64, types => types.DefaultIntegralType },
            //Floating.
            { DbType.Single, types => types.DefaultFloatingType },
            { DbType.Double, types => types.DefaultFloatingType },
            { DbType.Decimal, types => types.DefaultFloatingType },
            { DbType.VarNumeric, types => types.DefaultFloatingType },
            //Boolean
            { DbType.Boolean, types => types.DefaultBooleanType },
            //String.
            { DbType.AnsiString, types => types.DefaultStringType },
            { DbType.AnsiStringFixedLength, types => types.DefaultStringType },
            { DbType.String, types => types.DefaultStringType },
            { DbType.StringFixedLength, types => types.DefaultStringType },
            //Other
            { DbType.Binary, types => types.DefaultBinaryType },
            { DbType.Guid, types => types.DefaultGuidType }
        };

        protected static IDictionary<string, DatabaseQueryTypeArguments> Arguments = new Dictionary<string, DatabaseQueryTypeArguments>()
        {
        };

        public string GetType(ITypeConfig type)
        {
            var handler = default(SqlQueryTypesHandler);
            if (!Types.TryGetValue(type.Type, out handler))
            {
                throw new NotImplementedException();
            }
            var name = handler(this);
            var builder = new StringBuilder(name);
            var arguments = this.GetArguments(name, type);
            if (arguments.Any())
            {
                builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
                var first = true;
                foreach (var argument in arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.LIST_DELIMITER);
                    }
                    builder.AppendFormat("{0}", argument);
                }
                builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
            }
            return builder.ToString();
        }

        protected virtual IEnumerable<object> GetArguments(string name, ITypeConfig type)
        {
            var arguments = default(DatabaseQueryTypeArguments);
            Arguments.TryGetValue(name, out arguments);

            //Size.
            if (arguments.HasFlag(DatabaseQueryTypeArguments.Size) && type.Size == 0)
            {
                yield return this.DefaultSize;
            }
            else if (type.Size != 0)
            {
                yield return type.Size;
            }

            //Precision.
            if (arguments.HasFlag(DatabaseQueryTypeArguments.Precision) && type.Precision == 0)
            {
                yield return this.DefaultPrecision;
            }
            else if (type.Precision != 0)
            {
                yield return type.Precision;
            }

            //Scale.
            if (arguments.HasFlag(DatabaseQueryTypeArguments.Scale) && type.Scale == 0)
            {
                yield return this.DefaultScale;
            }
            else if (type.Scale != 0)
            {
                yield return type.Scale;
            }
        }

        protected virtual string DefaultIntegralType
        {
            get
            {
                return "INTEGER";
            }
        }

        protected virtual string DefaultFloatingType
        {
            get
            {
                return "FLOAT";
            }
        }

        protected virtual string DefaultBooleanType
        {
            get
            {
                return "BIT";
            }
        }

        protected virtual string DefaultStringType
        {
            get
            {
                return "TEXT";
            }
        }

        protected virtual string DefaultBinaryType
        {
            get
            {
                return "BINARY";
            }
        }

        protected virtual string DefaultGuidType
        {
            get
            {
                return "TEXT";
            }
        }

        protected virtual int DefaultSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected virtual int DefaultPrecision
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected virtual int DefaultScale
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public delegate string SqlQueryTypesHandler(SqlQueryTypes types);
    }
}
