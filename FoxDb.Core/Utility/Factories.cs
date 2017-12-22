using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxDb
{
    public static class Factories
    {
        public static ICollectionFactory CollectionFactory = new ListCollectionFactory();
    }
}
