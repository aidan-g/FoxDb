using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FoxDb
{
    public abstract class TestBase
    {
        static TestBase()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
        }

        public static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
            }
        }

        public static string FileName
        {
            get
            {
                return Path.Combine(CurrentDirectory, "test.db");
            }
        }

        public static string CreateSchema
        {
            get
            {
                return Resources.CreateSchema;
            }
        }

        public void AssertSequence<T>(IEnumerable<T> expected, IEnumerable<T> actual, bool equal = true)
        {
            var a = expected.ToArray();
            var b = actual.ToArray();
            Assert.AreEqual(a.Length, b.Length);
            for (var c = 0; c < a.Length; c++)
            {
                if (equal)
                {
                    Assert.AreEqual(a[c], b[c]);
                }
                else
                {
                    Assert.AreNotEqual(a[c], b[c]);
                }
            }
        }
    }
}
