using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class Behaviour : IBehaviour
    {
        public Behaviour()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        protected DynamicMethod Members { get; private set; }

        public abstract BehaviourType BehaviourType { get; }

        public abstract void Invoke<T>(BehaviourType behaviourType, IDatabaseSet<T> set, T item);
    }
}
