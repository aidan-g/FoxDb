using System.Collections.Generic;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IEntityPropertyResolutionStrategy<T>
    {
        IEnumerable<PropertyInfo> Properties { get; }

        PropertyInfo Resolve(string name);
    }
}
