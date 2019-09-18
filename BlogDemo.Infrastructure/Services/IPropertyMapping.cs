using System.Collections.Generic;

namespace BlogDemo.Infrastructure.Services
{
    public interface IPropertyMapping
    {
        IDictionary<string, List<MappedProperty>> MappingDictionary { get; }
    }
}