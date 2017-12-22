namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryCriteria
    {
        string Table { get; set; }

        string Column { get; set; }

        string Operator { get; set; }
    }
}
