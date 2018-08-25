using System;
using System.Collections;
using System.Data;

namespace FoxDb
{
    public class ParameterCollection : IDataParameterCollection
    {
        public ParameterCollection(IDataParameterCollection parameters)
        {
            this.InnerParameters = parameters;
        }

        public IDataParameterCollection InnerParameters { get; private set; }

        public virtual object this[string parameterName]
        {
            get
            {
                return this.InnerParameters[parameterName];
            }
            set
            {
                this.InnerParameters[parameterName] = value;
            }
        }

        public virtual object this[int index]
        {
            get
            {
                return this.InnerParameters[index];
            }
            set
            {
                this.InnerParameters[index] = value;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return this.InnerParameters.IsReadOnly;
            }
        }

        public virtual bool IsFixedSize
        {
            get
            {
                return this.InnerParameters.IsFixedSize;
            }
        }

        public virtual int Count
        {
            get
            {
                return this.InnerParameters.Count;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                return this.InnerParameters.SyncRoot;
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return this.InnerParameters.IsSynchronized;
            }
        }

        public virtual int Add(object value)
        {
            return this.InnerParameters.Add(value);
        }

        public virtual void Clear()
        {
            this.InnerParameters.Clear();
        }

        public virtual bool Contains(string parameterName)
        {
            return this.InnerParameters.Contains(parameterName);
        }

        public virtual bool Contains(object value)
        {
            return this.InnerParameters.Contains(value);
        }

        public virtual void CopyTo(Array array, int index)
        {
            this.InnerParameters.CopyTo(array, index);
        }

        public virtual int IndexOf(string parameterName)
        {
            return this.InnerParameters.IndexOf(parameterName);
        }

        public virtual int IndexOf(object value)
        {
            return this.InnerParameters.IndexOf(value);
        }

        public virtual void Insert(int index, object value)
        {
            this.InnerParameters.Insert(index, value);
        }

        public virtual void Remove(object value)
        {
            this.InnerParameters.Remove(value);
        }

        public virtual void RemoveAt(string parameterName)
        {
            this.InnerParameters.RemoveAt(parameterName);
        }

        public virtual void RemoveAt(int index)
        {
            this.InnerParameters.RemoveAt(index);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return this.InnerParameters.GetEnumerator();
        }
    }
}
