namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQuery Select<T>(params string[] filters) where T : IPersistable;

        IDatabaseQuery First<T>(params string[] filters) where T : IPersistable;

        IDatabaseQuery Count<T>(params string[] filters) where T : IPersistable;

        IDatabaseQuery Count<T>(IDatabaseQuery query) where T : IPersistable;

        IDatabaseQuery Insert<T>() where T : IPersistable;

        IDatabaseQuery Update<T>() where T : IPersistable;

        IDatabaseQuery Delete<T>() where T : IPersistable;
    }
}
