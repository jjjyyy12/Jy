using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Jy.IRepositories
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ToDynamicIEnumerable<TSource>(this IEnumerable<TSource> source, string fields = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObjectList = new List<ExpandoObject>();
            var propertyInfoList = new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',').ToList();
                if (!fieldsAfterSplit.Contains("Id", StringComparer.InvariantCultureIgnoreCase))
                {
                    fieldsAfterSplit.Add("Id");
                }
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }
                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
    /*
     [HttpGet(Name = "GetAllCustomers")]
        public async Task<IActionResult> GetAll(string fields)
        {
            var items = await _customerRepository.ListAllAsync();
            var results = Mapper.Map<IEnumerable<CustomerViewModel>>(items);
            var dynamicList = results.ToDynamicIEnumerable(fields);
            var links = CreateLinksForCustomers(fields);
            var dynamicListWithLinks = dynamicList.Select(customer =>
            {
                var customerDictionary = customer as IDictionary<string, object>;
                var customerLinks = CreateLinksForCustomer(
                    (int)customerDictionary["Id"], fields);
                customerDictionary.Add("links", customerLinks);
                return customerDictionary;
            });
            var resultWithLink = new
            {
                Value = dynamicListWithLinks,
                Links = links
            };
            return Ok(resultWithLink);
        }
     */
}
