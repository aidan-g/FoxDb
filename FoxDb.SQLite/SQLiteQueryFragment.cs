using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteQueryFragment
    {
        public const FragmentType Limit = (FragmentType)100;

        public const FragmentType Offset = (FragmentType)101;

        protected static IDictionary<FragmentType, byte> Priorities = new Dictionary<FragmentType, byte>()
        {
            { FragmentType.Add, 10 },
            { FragmentType.Update, 20 },
            { FragmentType.Delete, 30 },
            { FragmentType.Output, 40 },
            { FragmentType.Source, 50 },
            { FragmentType.Filter, 60 },
            { FragmentType.Sort, 70 },
            { Limit, 80 },
            { Offset, 90 }
        };

        public SQLiteQueryFragment(IFragmentTarget target) : this(target.CommandText, GetPriority(target))
        {

        }

        public SQLiteQueryFragment(string commandText, byte priority)
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
