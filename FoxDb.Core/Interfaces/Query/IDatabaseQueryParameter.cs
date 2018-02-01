using System;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryParameter : IEquatable<IDatabaseQueryParameter>
    {
        string Name { get; }

        ParameterType Type { get; }
    }

    public enum ParameterType
    {
        None = 0,
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 6
    }
}
