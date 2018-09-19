﻿using NUnit.Framework;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SQLite)]
    public class SchemaTests : TestBase
    {
        public SchemaTests(ProviderType providerType)
            : base(providerType)
        {

        }
    }
}