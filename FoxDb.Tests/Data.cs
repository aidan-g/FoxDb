using System;
using System.Collections.Generic;
using System.Linq;

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
                this.Test003Equals(other.Test003) &&
                this.Test004Equals(other.Test004);
        }

        private bool Test003Equals(Test003 other)
        {
            if (this.Test003 == null)
            {
                return other == null;
            }
            return this.Test003.Equals(other);
        }

        private bool Test004Equals(ICollection<Test004> other)
        {
            if (this.Test004 == null)
            {
                return other == null || other.Count == 0;
            }
            if (this.Test004.Count != other.Count)
            {
                return false;
            }
            var a = this.Test004.ToArray();
            var b = other.ToArray();
            for (var c = 0; c < a.Length; c++)
            {
                if (!a[c].Equals(b[c]))
                {
                    return false;
                }
            }
            return true;
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
