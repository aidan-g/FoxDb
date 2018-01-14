using Newtonsoft.Json;
using System;

namespace FoxDb
{
    public static class Utility
    {
        private static bool IsComparing { get; set; }

        new public static bool Equals(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (IsComparing)
            {
                return false;
            }
            IsComparing = true;
            try
            {
                var data1 = JsonConvert.SerializeObject(a);
                var data2 = JsonConvert.SerializeObject(b);
                return string.Equals(data1, data2, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                IsComparing = false;
            }
        }
    }
}
