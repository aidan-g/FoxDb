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
    }
}
