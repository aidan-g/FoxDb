using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDbCommand CreateCommand(this IDatabase database, IDatabaseQuery query, out IDatabaseParameters parameters, ITransactionSource transaction = null)
        {
            var command = database.Connection.CreateCommand();
            command.CommandText = query.CommandText;
            parameters = database.CreateParameters(command, query);
            if (transaction != null)
            {
                transaction.Bind(command);
            }
            return command;
        }

        public static IDbCommand CreateCommand(this IDatabase database, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            var command = database.Connection.CreateCommand();
            command.CommandText = query.CommandText;
            database.CreateParameters(command, query, parameters);
            if (transaction != null)
            {
                transaction.Bind(command);
            }
            return command;
        }

        public static IDatabaseParameters CreateParameters(this IDatabase database, IDbCommand command, IDatabaseQuery query, DatabaseParameterHandler handler = null)
        {
            foreach (var parameter in query.Parameters)
            {
                if (parameter.Type == ParameterType.None)
                {
                    continue;
                }
                CreateParameter(command, parameter.Name, parameter.Type);
            }
            var parameters = new DatabaseParameters(database, query, command.Parameters);
            if (handler != null)
            {
                handler(parameters);
            }
            return parameters;
        }

        public static void CreateParameter(IDbCommand command, string name, ParameterType type)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
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
    }
}
