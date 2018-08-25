using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public abstract class Provider : IProvider
    {
        protected Provider()
        {
            this.ValueMappings = new HashSet<ProviderValueMapping>();
            this.ValueMappings.Add(new ProviderValueMapping((type, value) => value == null, (type, value) => DBNull.Value));
            this.ValueMappings.Add(new ProviderValueMapping((type, value) => type.IsEnum, (type, value) => Convert.ChangeType(value, Enum.GetUnderlyingType(type))));
        }

        public HashSet<ProviderValueMapping> ValueMappings { get; private set; }

        public abstract IDbConnection CreateConnection(IDatabase database);

        public abstract IDatabaseQueryFactory CreateQueryFactory(IDatabase database);

        public abstract IDatabaseSchema CreateSchema(IDatabase database);

        public abstract IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database);

        public virtual DbType GetDbType(IDataParameter parameter, object value)
        {
            return parameter.DbType;
        }

        public virtual object GetDbValue(IDataParameter parameter, object value)
        {
            var type = default(Type);
            if (value != null)
            {
                type = value.GetType();
            }
            foreach (var mapping in this.ValueMappings)
            {
                if (mapping.Predicate(type, value))
                {
                    return mapping.Action(type, value);
                }
            }
            return value;
        }

        public class ProviderValueMapping
        {
            public ProviderValueMapping(Func<Type, object, bool> predicate, Func<Type, object, object> action)
            {
                this.Predicate = predicate;
                this.Action = action;
            }

            public Func<Type, object, bool> Predicate { get; private set; }

            public Func<Type, object, object> Action { get; private set; }
        }
    }
}
