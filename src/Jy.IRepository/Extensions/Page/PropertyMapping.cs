using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IRepositories
{
    public abstract class PropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; }

        protected PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            MappingDictionary[nameof(Entity.Id)] = new PropertyMappingValue(new List<string> { nameof(Entity.Id) });
        }

        public bool ValidMappingExistsFor(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);
                if (!MappingDictionary.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
