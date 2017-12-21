using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace FoxDb
{
    public static class Pluralization
    {
        static Pluralization()
        {
            Service = PluralizationService.CreateService(CultureInfo.CurrentCulture);
        }

        private static PluralizationService Service { get; set; }

        public static bool IsPlural(string word)
        {
            return Service.IsPlural(word);
        }

        public static bool IsSingular(string word)
        {
            return Service.IsSingular(word);
        }

        public static string Pluralize(string word)
        {
            return Service.Pluralize(word);
        }

        public static string Singularize(string word)
        {
            return Service.Singularize(word);
        }
    }
}
