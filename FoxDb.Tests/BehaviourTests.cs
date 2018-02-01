﻿using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SQLite)]
    public class BehaviourTests : TestBase
    {
        public BehaviourTests(ProviderType providerType) : base(providerType)
        {

        }

        [Test]
        public void NullableColumns()
        {
            var set = this.Database.Set<Orange>(this.Transaction);
            var data = new List<Orange>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Orange() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3", Field4 = 1, Field5 = 1 },
                new Orange() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3", Field4 = 2, Field5 = 2 },
                new Orange() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3", Field4 = 3, Field5 = 3 }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Field4 = null;
            data[1].Field5 = null;
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }

        [Test]
        public void Int32Columns()
        {
            var set = this.Database.Set<Pear>(this.Transaction);
            var data = new List<Pear>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Pear() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3", Field4 = 1 },
                new Pear() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3", Field4 = 2 },
                new Pear() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3", Field4 = 3 }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Field4 = 4;
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }

        [Test]
        public void EnumColumns()
        {
            var set = this.Database.Set<Apple>(this.Transaction);
            var data = new List<Apple>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Apple() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3", Field4 = AppleType.Type1 },
                new Apple() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3", Field4 = AppleType.Type2 },
                new Apple() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3", Field4 = AppleType.Type3 }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Field4 = AppleType.None;
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }

        [Table(Name = "Test001")]
        public class Orange : Test001
        {
            public virtual int? Field4 { get; set; }

            public virtual double? Field5 { get; set; }
        }

        [Table(Name = "Test001")]
        public class Pear : Test001
        {
            public virtual int Field4 { get; set; }
        }

        [Table(Name = "Test001")]
        public class Apple : Test001
        {
            public virtual AppleType Field4 { get; set; }
        }

        public enum AppleType : byte
        {
            None,
            Type1,
            Type2,
            Type3
        }
    }
}
