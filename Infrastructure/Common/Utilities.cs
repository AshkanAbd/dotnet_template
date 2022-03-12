using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Configs;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Common;

public static class Utilities
{
    public static float? ToFloat(this string s) => float.TryParse(s, out var f1) ? f1 : null;
    public static float ToFloat(this string s, float fallback) => float.TryParse(s, out var f1) ? f1 : fallback;
    public static double? ToDouble(this string s) => double.TryParse(s, out var f1) ? f1 : null;
    public static double ToDouble(this string s, double fallback) => double.TryParse(s, out var f1) ? f1 : fallback;
    public static int? ToInt(this string s) => int.TryParse(s, out var f1) ? f1 : null;
    public static int ToInt(this string s, int fallback) => int.TryParse(s, out var f1) ? f1 : fallback;
    public static int? ToByte(this string s) => byte.TryParse(s, out var f1) ? f1 : null;
    public static byte ToByte(this string s, byte fallback) => byte.TryParse(s, out var f1) ? f1 : fallback;
    public static long? ToLong(this string s) => long.TryParse(s, out var f1) ? f1 : null;
    public static long ToLong(this string s, long fallback) => long.TryParse(s, out var f1) ? f1 : fallback;
    public static decimal? ToDecimal(this string s) => decimal.TryParse(s, out var f1) ? f1 : null;
    public static decimal ToDecimal(this string s, decimal fallback) => decimal.TryParse(s, out var f1) ? f1 : fallback;

    public static bool IsGreaterThan(this IComparable first, IComparable second) => first.CompareTo(second) >= 0;
    public static bool IsLessThan(this IComparable first, IComparable second) => first.CompareTo(second) <= 0;
    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
    public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

    public static bool IsNullOrEmpty(this StringValues value) => StringValues.IsNullOrEmpty(value);
    public static int? ToInt(this long? l) => (int?) l;
    public static int ToInt(this long l) => (int) l;

    public static dynamic ToExpandoObject(this object o)
        => JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(o));

    public static List<dynamic> ToExpandoList(this object o)
        => JsonConvert.DeserializeObject<List<ExpandoObject>>(JsonConvert.SerializeObject(o))!
            .Select(x => (dynamic) x)
            .ToList();

    public static JObject ToJObject(this object o)
        => JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(o));

    public static JArray ToJArray(this object o)
        => JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(o));

    public static object TokenValueCaster(this JToken token, Type type)
    {
        try {
            if (type == null) {
                return null;
            }

            var valueMethod = token.GetType()
                .GetMethods()
                .FirstOrDefault(x => x.Name == "Value")?
                .MakeGenericMethod(type);

            return valueMethod?.Invoke(token, null);
        }
        catch (Exception) {
            return null;
        }
    }

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        RandomNumberGenerator.Create();
        var n = list.Count;
        while (n > 1) {
            byte[] box;
            do box = RandomNumberGenerator.GetBytes(1);
            while (!(box[0] < n * (byte.MaxValue / n)));
            var k = box[0] % n;
            n--;
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    public static long Percent(this long total, float percent)
    {
        return (long) ((decimal) total / 100 * (decimal) percent);
    }

    public static int Percent(this int total, float percent)
    {
        return (int) ((decimal) total / 100 * (decimal) percent);
    }

    public static double Percent(this double total, float percent)
    {
        return (double) ((decimal) total / 100 * (decimal) percent);
    }

    public static bool EqualsList<T>(this IList<T> first, IList<T> second)
    {
        if (first.Count != second.Count) {
            return false;
        }

        return !first.Where((t, i) => !t!.Equals(second[i])).Any();
    }

    public static Dictionary<string, object> ToDictionary(this object obj)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var property in obj.GetType().GetProperties()) {
            dictionary[property.Name] = property.GetValue(obj);
        }

        return dictionary;
    }

    public static object Fill(this object obj, Dictionary<string, object> values, bool withNulls = false)
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

    public static string GenerateJwtToken(IDictionary<string, string> claimDictionary, DateTime expireDate,
        JwtConfig config)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var claims = claimDictionary
            .Select(x => new Claim(x.Key, x.Value))
            .ToArray();
        var token = new JwtSecurityToken(
            issuer: config.Issuer,
            audience: config.Audience,
            claims: claims,
            expires: expireDate,
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateCode(int length = 5, bool useLetter = false)
    {
        var random = new Random((int) DateTime.Now.ToFileTime());
        string data;
        data = useLetter ? "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "0123456789";

        var chars = Enumerable.Range(0, length)
            .Select(_ => data[random.Next(0, data.Length)]);
        return new string(chars.ToArray());
    }
}