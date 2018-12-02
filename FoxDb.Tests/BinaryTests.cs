using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class BinaryTests : TestBase
    {
        public BinaryTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void CanAddUpdateDelete_Binary()
        {
            var set = this.Database.Set<Test007>(this.Transaction);
            var data = new List<Test007>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Test007() { Name = "Name1", Value = Guid.NewGuid().ToByteArray() },
                new Test007() { Name = "Name2", Value = Guid.NewGuid().ToByteArray() },
                new Test007() { Name = "Name3", Value = Guid.NewGuid().ToByteArray() }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Value = Guid.NewGuid().ToByteArray();
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [Test]
        public void CannotAdd_InvalidLength()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SQLite:
                    Assert.Ignore("The provider does not support length limits.");
                    return;
            }

            var set = this.Database.Set<Test007>(this.Transaction);
            try
            {
                set.Add(new Test007()
                {
                    Name = "Name",
                    Value = Encoding.Default.GetBytes("07656178-A362-47E5-9428-AA33EF1C5C49")
                });
                Assert.Fail("Expected exception.");
            }
            catch
            {

            }
        }
    }
}
