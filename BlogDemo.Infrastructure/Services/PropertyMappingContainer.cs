using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlogDemo.Core.interfaces;

namespace BlogDemo.Infrastructure.Services
{
    public class PropertyMappingContainer : IPropertyMappingContainer
    {
        protected internal readonly IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();
        public void Register<T>() where T : IPropertyMapping, new()
        {
            if (propertyMappings.All(x => x.GetType() != typeof(T)))
            {
                propertyMappings.Add(new T());
            }
        }

        public IPropertyMapping Resolve<TSource, TDestination>() where TDestination : IEntity
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>().ToList();
            if (matchingMapping.Count == 1)
            {
                return matchingMapping.First();
            }

            throw new Exception($"Cannot find property mapping instance for <{typeof(TSource)},{typeof(TDestination)}>");
        }

        public bool ValidateMappingExistsFor<TSource, TDestination>(string fields) where TDestination : IEntity
        {
            var propertyMapping = Resolve<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimedField = field.Trim();
                var indexOfFirstSpace = trimedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ? trimedField : trimedField.Remove(indexOfFirstSpace);
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    continue;
                }
                if (!propertyMapping.MappingDictionary.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
