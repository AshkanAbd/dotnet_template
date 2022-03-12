using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Infrastructure.ImageUrlFormatter
{
    public class ImageUrlFormatterFilter : IAsyncActionFilter
    {
        public ImageUrlFormatterFilter(IOptions<Config> options)
        {
            Config = options.Value;
        }

        public Config Config { get; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var response = await next();

            if (response.Result is JsonResult jsonResponse) {
                var getResponseMethod = jsonResponse.Value.GetType()
                    .GetMethods()
                    .FirstOrDefault(x => x.Name == "DataAsDataStruct");

                if (getResponseMethod == null) return;

                try {
                    var stdResponse = getResponseMethod.Invoke(jsonResponse.Value, null);
                    SearchAttribute(stdResponse?.GetType(), stdResponse);
                }
                catch {
                    // ignored
                }
            }
        }

        public void ReformatImageAttributes(object obj, PropertyInfo propertyInfo)
        {
            if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) &&
                propertyInfo.PropertyType.Name != "String") {
                ReformatEnumerableImageAttribute(obj, propertyInfo);
            }
            else {
                ReformatObjectImageAttribute(obj, propertyInfo);
            }
        }

        public void ReformatEnumerableImageAttribute(object obj, PropertyInfo propertyInfo)
        {
            if (!(propertyInfo.GetValue(obj) is IEnumerable values)) return;
            var strList = new List<string>();

            foreach (var value in values) {
                if (!(value is string str)) continue;
                strList.Add(NormalizePrefixAndImage(Config.ImageUrlFormatter.Prefix, str));
            }

            propertyInfo.SetValue(obj, strList.AsEnumerable());
        }

        public void ReformatObjectImageAttribute(object obj, PropertyInfo propertyInfo)
        {
            var value = propertyInfo.GetValue(obj);
            if (value is not string str) return;
            propertyInfo.SetValue(obj, NormalizePrefixAndImage(Config.ImageUrlFormatter.Prefix, str));
        }

        public static string NormalizePrefixAndImage(string prefix, string image)
        {
            if (prefix.EndsWith("/")) {
                if (image.StartsWith("/")) {
                    return $"{prefix}{image.Substring(1)}";
                }
                else {
                    return $"{prefix}{image}";
                }
            }
            else {
                if (image.StartsWith("/")) {
                    return $"{prefix}{image}";
                }
                else {
                    return $"{prefix}/{image}";
                }
            }
        }

        public void SearchAttribute(Type t, object obj)
        {
            if (obj == null) return;
            if (obj is IEnumerable && obj.GetType().Name != "String") {
                obj.GetType().GetGenericArguments()
                    .ToList()
                    .ForEach(y => {
                        if (!(obj is IEnumerable values)) return;
                        foreach (var value in values) {
                            SearchAttribute(y, value);
                        }
                    });
                return;
            }

            CheckProperties(t, obj);
            CheckNestedProperties(t, obj);
            CheckNestedListProperties(t, obj);
            CheckListProperties(t, obj);
        }

        public void CheckProperties(Type t, object obj)
        {
            t.GetProperties()
                .Where(x =>
                    x.GetCustomAttributes(typeof(ImageUrlFormatterAttribute), false)
                        .FirstOrDefault() != null
                ).Where(x =>
                    typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                ).ToList()
                .ForEach(x => ReformatImageAttributes(obj, x));
        }

        public void CheckNestedProperties(Type t, object obj)
        {
            t.GetProperties()
                .Where(x => x.PropertyType.Namespace != null)
                .Where(x => x.PropertyType.Namespace.StartsWith("Application"))
                .Where(x =>
                    !typeof(IEnumerable).IsAssignableFrom(x.PropertyType) || x.PropertyType.Name == "String"
                ).ToList()
                .ForEach(x => { SearchAttribute(x.PropertyType, x.GetValue(obj)); });
        }

        public void CheckNestedListProperties(Type t, object obj)
        {
            t.GetProperties()
                .Where(x => x.PropertyType.Namespace != null)
                .Where(x => x.PropertyType.Namespace.StartsWith("Application"))
                .Where(x =>
                    typeof(IEnumerable).IsAssignableFrom(x.PropertyType) && x.PropertyType.Name != "String"
                ).ToList()
                .ForEach(x => {
                    x.PropertyType.GetGenericArguments()
                        .ToList()
                        .ForEach(y => {
                            if (!(x.GetValue(obj) is IEnumerable values)) return;
                            foreach (var value in values) {
                                SearchAttribute(y, value);
                            }
                        });
                });
        }

        public void CheckListProperties(Type t, object obj)
        {
            t.GetProperties().Where(x =>
                typeof(IEnumerable).IsAssignableFrom(x.PropertyType) && x.PropertyType.Name != "String"
            ).ToList().ForEach(x => {
                x.PropertyType.GetGenericArguments()
                    .ToList()
                    .ForEach(y => {
                        if (!(x.GetValue(obj) is IEnumerable values)) return;
                        foreach (var value in values) {
                            SearchAttribute(y, value);
                        }
                    });
            });
        }
    }
}