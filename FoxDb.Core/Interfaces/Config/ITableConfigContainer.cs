using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ITableConfigContainer : IEnumerable<ITableConfig>, IEquatable<ITableConfigContainer>
    {

    }
}
