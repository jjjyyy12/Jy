using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Jy.IRepositories
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, PropertyMapping propertyMapping)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var mappingDictionary = propertyMapping.MappingDictionary;
            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }
            var orderByAfterSplit = orderBy.Split(',');
            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderByClause = orderByClause.Trim();
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }
                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException(propertyName);
                }
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }
            }
            return source;
        }

        public static IQueryable<object> ToDynamicQueryable<TSource>
            (this IQueryable<TSource> source, string fields, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(fields))
            {
                return (IQueryable<object>)source;
            }

            fields = fields.ToLower();
            var fieldsAfterSplit = fields.Split(',').ToList();
            if (!fieldsAfterSplit.Contains("id", StringComparer.InvariantCultureIgnoreCase))
            {
                fieldsAfterSplit.Add("id");
            }
            var selectClause = "new (";

            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }
                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException(propertyName);
                }
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    selectClause += $" {destinationProperty},";
                }
            }

            selectClause = selectClause.Substring(0, selectClause.Length - 1) + ")";
            return (IQueryable<object>)source.Select(selectClause);
        }

    }
}
