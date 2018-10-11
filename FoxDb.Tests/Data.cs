using System.Collections.Generic;

namespace FoxDb
{
    public abstract class TestData
    {
        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            return Utility.Equals(this, obj);
        }
    }

    public class Test001 : TestData
    {
        public long Id { get; set; }

        public virtual string Field1 { get; set; }

        public virtual string Field2 { get; set; }

        public virtual string Field3 { get; set; }
    }

    public class Test002 : TestData
    {
        public Test002()
        {
            this.Test004 = new List<Test004>();
        }

        public long Id { get; set; }

        [Type(IsNullable = true)]
        public long Test003_Id { get; set; }

        [Type(IsNullable = true)]
        public long Test004_Id { get; set; }

        public string Name { get; set; }

        public Test003 Test003 { get; set; }

        public virtual ICollection<Test004> Test004 { get; set; }
    }

    public class Test003 : TestData
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class Test004 : TestData
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class Test005 : TestData
    {
        public long Id { get; set; }

        public bool Value { get; set; }
    }
}
