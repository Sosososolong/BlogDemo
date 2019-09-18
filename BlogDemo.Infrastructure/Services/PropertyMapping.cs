using BlogDemo.Core.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Infrastructure.Services
{
    public abstract class PropertyMapping<TSource, TDestination> : IPropertyMapping where TDestination : IEntity
    {
        public IDictionary<string, List<MappedProperty>> MappingDictionary { get; }

        protected PropertyMapping(IDictionary<string, List<MappedProperty>> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            MappingDictionary[nameof(IEntity.Id)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(IEntity.Id), Revert = false}
            };
        }
    }
}
