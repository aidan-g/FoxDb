using FoxDb.Interfaces;

namespace FoxDb
{
    public class SimpleParameterHandlerStrategy<T> : IParameterHandlerStrategy
    {
        public SimpleParameterHandlerStrategy(T item)
        {
            this.Item = item;
        }

        public T Item { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                return new DatabaseParameterHandler(parameters =>
                {
                    var resolutionStrategy = new EntityPropertyResolutionStrategy<T>();
                    foreach (var property in resolutionStrategy.Properties)
                    {
                        if (parameters.Contains(property.Name))
                        {
                            parameters[property.Name] = property.GetValue(this.Item);
                        }
                    }
                });
            }
        }
    }
}
