using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDatabaseCommand CreateCommand(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return CreateCommand(database, query, DatabaseCommandFlags.None, transaction);
        }

        public static IDatabaseCommand CreateCommand(this IDatabase database, IDatabaseQuery query, DatabaseCommandFlags flags, ITransactionSource transaction = null)
        {
            var factory = new Func<IDatabaseCommand>(() =>
            {
                var command = database.Connection.CreateCommand();
                command.CommandText = query.CommandText;
                var parameters = database.CreateParameters(command, query);
                if (transaction != null)
                {
                    transaction.Bind(command);
                }
                return new DatabaseCommand(command, parameters, flags);
            });
            if (transaction != null && !flags.HasFlag(DatabaseCommandFlags.NoCache))
            {
                var command = transaction.CommandCache.GetOrAdd(query, factory);
                command.Parameters.Reset();
                return command;
            }
            return factory();
        }

        public static IDatabaseParameters CreateParameters(this IDatabase database, IDbCommand command, IDatabaseQuery query)
        {
            foreach (var parameter in query.Parameters)
            {
                if (parameter.IsDeclared)
                {
                    continue;
                }
                CreateParameter(command, parameter.Name, parameter.Type, parameter.Direction);
            }
            return new DatabaseParameters(database, query, command.Parameters);
        }

        public static void CreateParameter(IDbCommand command, string name, DbType type, ParameterDirection direction)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Direction = direction;
            command.Parameters.Add(parameter);
        }

        public static object DefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return FastActivator.Instance.Activate(type);
            }
            return null;
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> sequence)
        {
            foreach (var element in sequence.ToArray()) //Buffer to void "Collection was modified..."
            {
                collection.Remove(element);
            }
        }

        public static T With<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }

        public static T With<T>(this T value, Func<T, T> func)
        {
            return func(value);
        }

        public static bool IsScalar(this Type type)
        {
            return type.IsPrimitive || type.IsValueType || typeof(string).IsAssignableFrom(type);
        }

        public static bool IsGeneric(this Type type, out Type elementType)
        {
            if (!type.IsGenericType)
            {
                elementType = null;
                return false;
            }
            var arguments = type.GetGenericArguments();
            if (arguments.Length != 1)
            {
                elementType = null;
                return false;
            }
            elementType = arguments[0];
            return true;
        }

        public static bool IsCollection(this Type type, out Type elementType)
        {
            if (!type.IsGeneric(out elementType))
            {
                return false;
            }
            if (type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return true;
            }
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType)
                {
                    continue;
                }
                if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return true;
                }
            }
            return false;
        }

        public static IDictionary<string, object> ToDictionary(this IDataReader reader)
        {
            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var a = 0; a < reader.FieldCount; a++)
            {
                var name = reader.GetName(a);
                var value = reader.GetValue(a);
                data[name] = value;
            }
            return data;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> sequence)
        {
            foreach (var element in sequence)
            {
                collection.Add(element);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var element in sequence)
            {
                action(element);
            }
        }

        public static T Get<T>(this IDatabaseReaderRecord record, IColumnConfig column)
        {
            return record.Get<T>(column.Identifier);
        }

        public static TResult Using<T, TResult>(this T value, Func<T, TResult> action, Func<bool> dispose) where T : IDisposable
        {
            try
            {
                return action(value);
            }
            finally
            {
                if (dispose())
                {
                    value.Dispose();
                }
            }
        }
    }
}
