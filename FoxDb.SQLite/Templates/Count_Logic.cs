namespace FoxDb.Templates
{
    public partial class Count
    {
        public Count(string commandText)
        {
            this.CommandText = commandText;
        }

        public string CommandText { get; private set; }
    }
}
