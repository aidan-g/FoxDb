using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class SqlQueryFragment
    {
        public const FragmentType Join = (FragmentType)100;

        protected static IDictionary<FragmentType, byte> Priorities = new Dictionary<FragmentType, byte>()
        {
            //Query.
            { FragmentType.With, 5 },
            { FragmentType.Add, 10 },
            { FragmentType.Update, 20 },
            { FragmentType.Delete, 30 },
            { FragmentType.Output, 40 },
            { FragmentType.Source, 50 },
            { Join, 60 },
            { FragmentType.Filter, 70 },
            { FragmentType.Aggregate, 80 },
            { FragmentType.Sort, 90 },
            //Schema.
            { FragmentType.Create, 10 },
            { FragmentType.Alter, 10 },
            { FragmentType.Drop, 10 }
        };

        public SqlQueryFragment(IFragmentTarget target)
            : this(target.CommandText, GetPriority(target))
        {

        }

        public SqlQueryFragment(string commandText, byte priority)
        {
            this.CommandText = commandText;
            this.Priority = priority;
        }

        public string CommandText { get; private set; }

        public byte Priority { get; private set; }

        public static byte GetPriority(IFragmentTarget target)
        {
            var priority = default(byte);
            if (!Priorities.TryGetValue(target.FragmentType, out priority))
            {
                throw new NotImplementedException();
            }
            return priority;
        }
    }
}
