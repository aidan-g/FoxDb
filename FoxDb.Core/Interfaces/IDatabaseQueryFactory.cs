namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQuery Select<T>(params string[] filters);

        IDatabaseQuery First<T>(params string[] filters);

        IDatabaseQuery Count<T>(params string[] filters);

        IDatabaseQuery Insert<T>();

        IDatabaseQuery Update<T>();

        IDatabaseQuery Delete<T>();
    }
}
