namespace WBBWeb.Attributes
{
    using SimpleInjector.Advanced;
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    public class ImportPropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type type, PropertyInfo prop)
        {
            return prop.GetCustomAttributes(typeof(ImportAttribute)).Any();
        }
    }
}