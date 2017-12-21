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

        public void AssertSet<T>(IDatabaseSet<T> set, IList<T> expected) where T : IPersistable
        {
            Assert.AreEqual(expected.Count, set.Count);
            var actual = set.ToList();
            for (var a = 0; a < expected.Count; a++)
            {
                Assert.AreEqual(expected[a], actual[a]);
            }
        }
    }
}
