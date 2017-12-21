using System;
using System.Collections.Generic;

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

    public class Test002 : Persistable<long>, IEquatable<Test002>
    {
        public string Name { get; set; }

        public Test003 Test003 { get; set; }

        public ICollection<Test004> Test004 { get; set; }

        public bool Equals(Test002 other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) &&
                string.Equals(this.Name, other.Name) &&
                (this.Test003 != null ? this.Test003.Equals(other.Test003) : other.Test003 == null);
        }
    }

    public class Test003 : Persistable<long>, IEquatable<Test003>
    {
        public string Name { get; set; }

        public bool Equals(Test003 other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) && string.Equals(this.Name, other.Name);
        }
    }

    public class Test004 : Persistable<long>, IEquatable<Test004>
    {
        public string Name { get; set; }

        public bool Equals(Test004 other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) && string.Equals(this.Name, other.Name);
        }
    }
}
