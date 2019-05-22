using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
                database.CreateParameter(command, parameter.Name, parameter.Type, parameter.Size, parameter.Precision, parameter.Scale, parameter.Direction);
            }
            return new DatabaseParameters(database, query, command.Parameters);
        }

        public static void CreateParameter(this IDatabase database, IDbCommand command, string name, DbType type, int size, byte precision, byte scale, ParameterDirection direction)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = database.Translation.GetRemoteType(type);
            parameter.Size = size;
            parameter.Precision = precision;
            parameter.Scale = scale;
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

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> sequence)
        {
            foreach (var element in sequence)
            {
                collection.Add(element);
            }
        }

        public static void EnqueueRange<T>(this Queue<T> collection, IEnumerable<T> sequence)
        {
            foreach (var element in sequence)
            {
                collection.Enqueue(element);
            }
        }

        public static void PushRange<T>(this Stack<T> collection, IEnumerable<T> sequence)
        {
            foreach (var element in sequence)
            {
                collection.Push(element);
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

        public static T CreateDelegate<T>(this MethodInfo method)
        {
            //return (T)(object)method.CreateDelegate(typeof(T));
            throw new NotImplementedException();
        }

        public static PropertyInfo GetLambdaProperty<T>(this Expression expression)
        {
            return expression.GetLambdaProperty(typeof(T));
        }

        public static PropertyInfo GetLambdaProperty(this Expression expression, ITableConfig table)
        {
            if (table is IMappingTableConfig)
            {
                return expression.GetLambdaProperty((table as IMappingTableConfig).LeftTable.TableType);
            }
            return expression.GetLambdaProperty(table.TableType);
        }

        public static PropertyInfo GetLambdaProperty(this Expression expression, Type type)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new NotImplementedException();
            }
            var lambda = expression as LambdaExpression;
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotImplementedException();
            }
            var member = lambda.Body as MemberExpression;
            if (!(member.Member is PropertyInfo))
            {
                throw new NotImplementedException();
            }
            var property = member.Member as PropertyInfo;
            if (property.DeclaringType == type)
            {
                return property;
            }
            return PropertyResolutionStrategy.GetProperty(type, property.Name);
        }

        public static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> sequence) where T : IEnumerable<TResult>
        {
            return sequence.SelectMany(element => element);
        }

        public static bool Contains<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2)
        {
            //TODO: Why the fuck does this not work?
            //return sequence1.Intersect(sequence2).Count() == values.Count();
            var count = 0;
            foreach (var element in sequence2)
            {
                if (sequence1.Contains(element))
                {
                    count++;
                }
            }
            return count == sequence2.Count();
        }

        public static bool Contains<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2, IEqualityComparer<T> comparer)
        {
            //TODO: Why the fuck does this not work?
            //return sequence1.Intersect(sequence2, comparer).Count() == values.Count();
            var count = 0;
            foreach (var element in sequence2)
            {
                if (sequence1.Contains(element, comparer))
                {
                    count++;
                }
            }
            return count == sequence2.Count();
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence1, IEnumerable<IEnumerable<T>> sequence2)
        {
            return sequence1.Concat(sequence2.SelectMany<IEnumerable<T>, T>());
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Concat(sequence2.AsEnumerable());
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Except(sequence2.AsEnumerable());
        }

        public static IQueryable<T> Except<T>(this IQueryable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Except(sequence2.AsEnumerable());
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2, bool ignoreOrder)
        {
            if (!ignoreOrder)
            {
                return sequence1.SequenceEqual(sequence2);
            }
            return !sequence1.Except(sequence2).Any() && !sequence2.Except(sequence1).Any();
        }

        public static IEnumerable<string> Split(this string sequence, string separator, StringComparison comparisonType)
        {
            return sequence.Split(separator, comparisonType, StringSplitOptions.None);
        }

        public static IEnumerable<string> Split(this string sequence, string separator, StringComparison comparisonType, StringSplitOptions options)
        {
            var element = default(string);
            for (int a = 0, b = sequence.Length, c = separator.Length, d = 0; a < b; a = d)
            {
                d = sequence.IndexOf(separator, a, comparisonType);
                if (d == -1)
                {
                    element = sequence.Substring(a, b - a);
                    d = b;
                }
                else
                {
                    element = sequence.Substring(a, d - a);
                    d += c;
                }
                if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && string.IsNullOrWhiteSpace(element))
                {
                    continue;
                }
                yield return element.Trim();
            }
        }

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> sequence, TKey key)
        {
            var value = default(TValue);
            return sequence.TryRemove(key, out value);
        }
    }
}
