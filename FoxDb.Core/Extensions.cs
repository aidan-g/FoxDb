using FoxDb.Interfaces;
using System.Collections;
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

        public static string[] ToColumns(this IEnumerable<IColumnConfig> sequence)
        {
            return sequence.Select(element => element.Name).ToArray();
        }

        public static string[] ToColumns(this IEnumerable<IDatabaseQueryCriteria> sequence)
        {
            return sequence.Select(element => element.Column).ToArray();
        }
    }
}
