using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class Persistable<T> : IPersistable<T>
    {
        public T Id { get; set; }

        object IPersistable.Id
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = Converter.ChangeType<T>(value);
            }
        }

        public bool HasId
        {
            get
            {
                if (this.Id == null)
                {
                    return false;
                }
                return !this.Id.Equals(default(T));
            }
        }

        public bool Equals(IPersistable other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }

        public bool Equals(IPersistable<T> other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }
    }
}
