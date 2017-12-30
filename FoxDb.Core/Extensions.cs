using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = query.CommandText;
            command.CreateParameters(query, parameters);
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            return command;
        }

        public static IDatabaseParameters CreateParameters(this IDbCommand command, IDatabaseQuery query, DatabaseParameterHandler handler)
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
                return Activator.CreateInstance(type);
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
    }
}
