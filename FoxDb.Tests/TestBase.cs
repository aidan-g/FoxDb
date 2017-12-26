using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoxDb
{
    public abstract class TestBase
    {
        public static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
            }
        }

        public static string CreateSchema
        {
            get
            {
                return Resources.CreateSchema;
            }
        }

        public void AssertSequence<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var a = expected.ToArray();
            var b = actual.ToArray();
            Assert.AreEqual(a.Length, b.Length);
            for (var c = 0; c < a.Length; c++)
            {
                Assert.AreEqual(a[c], b[c]);
            }
        }
    }
}
