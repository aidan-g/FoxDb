using FoxDb.Interfaces;
using System;
using System.Reflection;
using System.Web.UI.WebControls;

namespace FoxDb
{
    public class TypeFactory : ITypeFactory
    {
        public ITypeConfig Create(ITypeSelector selector)
        {
            switch (selector.Type)
            {
                case TypeSelectorType.Property:
                    return this.Create(selector.Property);
                default:
                    throw new NotImplementedException();
            }
        }

        public ITypeConfig Create(PropertyInfo property)
        {
            return new TypeConfig(
                Parameter.ConvertTypeCodeToDbType(Type.GetTypeCode(property.PropertyType))
            );
        }
    }
}
