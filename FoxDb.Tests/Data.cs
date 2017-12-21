using System;

namespace FoxDb
{
    public class Test001 : Persistable<long>, IEquatable<Test001>
    {
        public string Field1 { get; set; }

        public string Field2 { get; set; }

        public string Field3 { get; set; }

        public bool Equals(Test001 other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) &&
                string.Equals(this.Field1, other.Field1) &&
                string.Equals(this.Field2, other.Field2) &&
                string.Equals(this.Field3, other.Field3);
        }
    }
}
