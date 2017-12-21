using FoxDb.Interfaces;

namespace FoxDb
{
    public class ParameterHandlerStrategy<T> : IParameterHandlerStrategy
    {
        public ParameterHandlerStrategy(T item)
        {
            if (item == null)
            {

            }
            this.Item = item;
        }

        public T Item { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                var resolutionStrategy = new EntityPropertyResolutionStrategy<T>();
                return new DatabaseParameterHandler(parameters =>
                {
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