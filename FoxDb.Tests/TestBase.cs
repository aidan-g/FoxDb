using System.IO;

namespace FoxDb
{
    public abstract class TestBase
    {
        public static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
            }
        }
    }
}
