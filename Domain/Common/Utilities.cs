namespace Domain.Common;

public static class Utilities
{
    public static T Fill<T>(this T obj, Dictionary<string, object> values, bool withNulls = false)
    {
        foreach (var property in obj.GetType().GetProperties()) {
            if (!values.ContainsKey(property.Name)) continue;

            if (values[property.Name] == null && withNulls) {
                property.SetValue(obj, null);
            }

            if (values[property.Name] != null && values[property.Name]?.GetType() == property.PropertyType) {
                property.SetValue(obj, values[property.Name]);
            }
        }

        return obj;
    }
}