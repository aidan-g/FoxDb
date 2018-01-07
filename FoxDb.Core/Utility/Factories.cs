using FoxDb.Interfaces;

namespace FoxDb
{
    public static class Factories
    {
        public static ICollectionFactory CollectionFactory = new ListCollectionFactory();
    }
}
