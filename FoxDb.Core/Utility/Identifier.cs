using System;

namespace FoxDb
{
    public static class Unique
    {
        public static string New
        {
            get
            {
                return Guid.NewGuid().ToString("d").Split('-')[0].ToLower();
            }
        }
    }
}
