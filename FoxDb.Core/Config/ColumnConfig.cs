using FoxDb.Interfaces;

namespace FoxDb
{
    public class ColumnConfig : IColumnConfig
    {
        public ColumnConfig(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string Property { get; set; }

        public bool IsKey { get; set; }
    }
}
