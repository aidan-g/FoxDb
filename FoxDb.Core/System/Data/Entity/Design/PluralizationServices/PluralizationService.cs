//---------------------------------------------------------------------
// <copyright file="PluralizationService.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       venkatja
// @backupOwner willa
//---------------------------------------------------------------------
using System.Globalization;

namespace System.Data.Entity.Design.PluralizationServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pluralization")]
    public abstract class PluralizationService
    {
        public CultureInfo Culture { get; protected set; }

        public abstract bool IsPlural(string word);
        public abstract bool IsSingular(string word);
        public abstract string Pluralize(string word);
        public abstract string Singularize(string word);

        /// <summary>
        /// Factory method for PluralizationService. Only support english pluralization.
        /// Please set the PluralizationService on the System.Data.Entity.Design.EntityModelSchemaGenerator
        /// to extend the service to other locales.
        /// </summary>
        /// <param name="culture">CultureInfo</param>
        /// <returns>PluralizationService</returns>
        public static PluralizationService CreateService(CultureInfo culture)
        {
            return new EnglishPluralizationService();
        }
    }
}
