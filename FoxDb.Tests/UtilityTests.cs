using NUnit.Framework;
using System;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void CanSplitString_OrdinalIgnoreCase_RemoveEmptyEntries()
        {
            var subject = " AA Value1 aa Value2 AA Value3 AA aa AA Value4 AA ";
            var sequence = subject
                .Split("aa", StringComparison.OrdinalIgnoreCase, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            Assert.AreEqual(4, sequence.Length);
            Assert.AreEqual("Value1", sequence[0]);
            Assert.AreEqual("Value2", sequence[1]);
            Assert.AreEqual("Value3", sequence[2]);
            Assert.AreEqual("Value4", sequence[3]);
        }

        [Test]
        public void CanIncrement_Numeric()
        {
            var subject = new Test001();
            var incrementor = new LambdaPropertyIncrementorStrategy().CreateNumericIncrementor<Test001>(
                typeof(Test001).GetProperty("Version")
            );
            for (var a = 0; a < 1024; a++)
            {
                Assert.AreEqual(a, subject.Version);
                incrementor(subject);
            }
        }

        [Test]
        public void CanIncrement_Binary()
        {
            var subject = new Whisker()
            {
                Version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            var incrementor = new LambdaPropertyIncrementorStrategy().CreateBinaryIncrementor<Whisker>(
                typeof(Whisker).GetProperty("Version"),
                8
            );
            for (var a = 0; a < 1024; a++)
            {
                var version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(subject.Version, version, version.Length);
                Array.Reverse(version);
                Assert.AreEqual(a, BitConverter.ToInt64(version, 0));
                incrementor(subject);
            }
        }

        [Table(Name = "Test002")]
        public class Whisker : Test002
        {
            [Type(Size = 8)]
            [Column(Flags = ColumnFlags.Generated | ColumnFlags.ConcurrencyCheck)]
            public byte[] Version { get; set; }
        }
    }
}
