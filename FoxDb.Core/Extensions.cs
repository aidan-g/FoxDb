using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, IDatabaseQuery query, out IDatabaseParameters parameters, ITransactionSource transaction = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = query.CommandText;
            parameters = command.CreateParameters(query);
            if (transaction != null)
            {
                transaction.Bind(command);
            }
            return command;
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = query.CommandText;
            command.CreateParameters(query, parameters);
            if (transaction != null)
            {
                transaction.Bind(command);
            }
            return command;
        }

        public static IDatabaseParameters CreateParameters(this IDbCommand command, IDatabaseQuery query, DatabaseParameterHandler handler = null)
        {
            foreach (var parameterName in query.ParameterNames)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                command.Parameters.Add(parameter);
            }
            var parameters = new DatabaseParameters(command.Parameters);
            if (handler != null)
            {
                handler(parameters);
            }
            return parameters;
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
