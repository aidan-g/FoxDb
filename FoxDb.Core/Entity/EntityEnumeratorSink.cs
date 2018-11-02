using FoxDb.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumeratorSink : IEntityEnumeratorSink
    {
        private EntityEnumeratorSink()
        {
            this.Items = new List<object>();
        }

        public EntityEnumeratorSink(ITableConfig table)
            : this()
        {
            this.Table = table;
        }

        public ITableConfig Table { get; private set; }

        public IList<object> Items { get; private set; }

        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        public void Add(object item)
        {
            this.Items.Add(item);
        }

        public void Clear()
        {
            this.Items.Clear();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
