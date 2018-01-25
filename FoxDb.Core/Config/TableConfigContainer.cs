using FoxDb.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class TableConfigContainer : ITableConfigContainer
    {
        public TableConfigContainer(IEnumerable<ITableConfig> tables)
        {
            this.Tables = tables.Where(table => table != null).ToArray();
        }

        public TableConfigContainer(params ITableConfig[] tables) : this(tables.AsEnumerable())
        {

        }

        public IEnumerable<ITableConfig> Tables { get; private set; }

        public IEnumerator<ITableConfig> GetEnumerator()
        {
            return this.Tables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(", ", this.Tables);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                foreach (var table in this.Tables)
                {
                    hashCode += table.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is ITableConfigContainer)
            {
                return this.Equals(obj as ITableConfigContainer);
            }
            return base.Equals(obj);
        }

        public bool Equals(ITableConfigContainer other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Contains(other);
        }

        public static bool operator ==(TableConfigContainer a, TableConfigContainer b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            if (object.ReferenceEquals((object)a, (object)b))
            {
                return true;
            }
            return a.Equals(b);
        }

        public static bool operator !=(TableConfigContainer a, TableConfigContainer b)
        {
            return !(a == b);
        }
    }
}
