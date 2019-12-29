using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    [TestFixture]
    public class ConverterTests
    {
        [Test]
        public void CanConvert()
        {
            var expected = 1;
            var actual = Converter.ChangeType<int>("1");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanConvert_Nullable()
        {
            {
                var expected = (int?)null;
                var actual = Converter.ChangeType<int?>(null);
                Assert.AreEqual(expected, actual);
            }
            {
                var expected = (int?)1;
                var actual = Converter.ChangeType<int?>(1);
                Assert.AreEqual(expected, actual);
            }
            {
                var expected = (int?)1;
                var actual = Converter.ChangeType<int?>(1L);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
