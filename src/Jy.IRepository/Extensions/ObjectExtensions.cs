using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Jy.IRepositories
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ToDynamic<TSource>(this TSource source, string fields = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dataShapedObject = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                return dataShapedObject;
            }
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
                    throw new Exception($"没有在‘{typeof(TSource)}’上找到‘{propertyName}’这个Property");
                }
                var propertyValue = propertyInfo.GetValue(source);
                ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            return dataShapedObject;
        }
    }
    /*
     [HttpGet]
        [Route("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get(int id, string fields)
        {
            var item = await _customerRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var customerVm = Mapper.Map<CustomerViewModel>(item);
            var links = CreateLinksForCustomer(id, fields);
            var dynamicObject = customerVm.ToDynamic(fields) as IDictionary<string, object>;
            dynamicObject.Add("links", links);
            return Ok(dynamicObject);
        }
     */
}
