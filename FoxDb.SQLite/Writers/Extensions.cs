using FoxDb.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IEnumerable<ITableConfigContainer> Prioritize(this IEnumerable<ITableConfigContainer> containers, ISQLiteQueryWriter writer)
        {
            var result = new List<ITableConfigContainer>();
            var remaining = containers.ToList();
            var source = writer.GetContext<ISourceBuilder>();
            foreach (var table in source.Tables)
            {
                foreach (var container in remaining)
                {
                    if (container.Contains(table.Table))
                    {
                        result.Add(container);
                        remaining.Remove(container);
                        break;
                    }
                }
            }
            var contains = new Func<ITableConfigContainer, bool>(container =>
            {
                return result.Any(existing => existing.Any(table => container.Contains(table)));
            });
            while (remaining.Count > 0)
            {
                foreach (var container in remaining)
                {
                    if (contains(container))
                    {
                        result.Add(container);
                        remaining.Remove(container);
                        break;
                    }
                }
            }
            return result;
        }
    }
}
