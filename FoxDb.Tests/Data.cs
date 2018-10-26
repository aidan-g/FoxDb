#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

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

    public class Test001 : TestData, IEntityConfiguration
    {
        public long Id { get; set; }

        [Index(Name = "Fields", Flags = IndexFlags.Unique)]
        public virtual string Field1 { get; set; }

        [Index(Name = "Fields", Flags = IndexFlags.Unique)]
        public virtual string Field2 { get; set; }

        [Index(Name = "Fields", Flags = IndexFlags.Unique)]
        public virtual string Field3 { get; set; }

        [Column(ColumnFlags.ConcurrencyCheck)]
        public int Version { get; set; }

        public void Configure(IConfig config, ITableConfig table)
        {
            var index = table.GetIndex(IndexConfig.By(new[] { "Field1", "Field2", "Field3" }, Defaults.Index.Flags));
            if (index != null)
            {
                index.Expression = index.Expression.Combine(
                    QueryOperator.AndAlso,
                    index.Columns.Select(column => index.CreateConstraint().With(expression =>
                    {
                        expression.Left = expression.CreateColumn(column);
                        expression.Operator = expression.CreateOperator(QueryOperator.Is);
                        expression.Right = expression.CreateUnary(QueryOperator.Not, expression.CreateOperator(QueryOperator.Null));
                    })).ToArray()
                );
            }
        }
    }

    public class Test002 : TestData
    {
        public Test002()
        {
            this.Test004 = new List<Test004>();
        }

        public long Id { get; set; }

        [Type(IsNullable = true)]
        [Index(Name = "Test003")]
        public long Test003_Id { get; set; }

        [Type(IsNullable = true)]
        [Index(Name = "Test003")]
        public long Test004_Id { get; set; }

        [Index]
        public string Name { get; set; }

        public Test003 Test003 { get; set; }

        public virtual ICollection<Test004> Test004 { get; set; }
    }

    public class Test003 : TestData
    {
        public long Id { get; set; }

        [Index]
        public string Name { get; set; }
    }

    public class Test004 : TestData
    {
        public long Id { get; set; }

        [Index]
        public string Name { get; set; }
    }

    public class Test005 : TestData
    {
        public long Id { get; set; }

        [Index]
        public bool Value { get; set; }
    }
}
