namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQueryCriteria Criteria<T>(string column) where T : IPersistable;

        IDatabaseQueryCriteria Criteria<T1, T2>(string column) where T1 : IPersistable where T2 : IPersistable;

        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQuery Select<T>(params IDatabaseQueryCriteria[] criteria) where T : IPersistable;

        IDatabaseQuery Select<T1, T2>(params IDatabaseQueryCriteria[] criteria) where T1 : IPersistable where T2 : IPersistable;

        IDatabaseQuery First<T>(params IDatabaseQueryCriteria[] criteria) where T : IPersistable;

        IDatabaseQuery Count<T>(params IDatabaseQueryCriteria[] criteria) where T : IPersistable;

        IDatabaseQuery Count<T>(IDatabaseQuery query) where T : IPersistable;

        IDatabaseQuery Insert<T>() where T : IPersistable;

        IDatabaseQuery Insert<T1, T2>() where T1 : IPersistable where T2 : IPersistable;

        IDatabaseQuery Update<T>() where T : IPersistable;

        IDatabaseQuery Delete<T>() where T : IPersistable;

        IDatabaseQuery Delete<T1, T2>() where T1 : IPersistable where T2 : IPersistable;
    }
}
