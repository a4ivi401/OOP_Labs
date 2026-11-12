using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Lab03.DAL.Entities;

namespace Lab03.DAL.Providers;

public class CustomDataProvider<T> : IDataProvider<T>
{
    public string FormatName => "Custom (Text Token)";
    public string DefaultExtension => ".txt";

    public void Serialize(string filePath, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        EnsureDirectory(filePath);

        StringBuilder sb = new();
        if (data is Array array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                object? item = array.GetValue(i);
                if (item != null)
                {
                    SerializeSingleEntity(sb, item);
                }
            }
        }
        else if (data is System.Collections.IEnumerable enumerable)
        {
            foreach (object? item in enumerable)
            {
                if (item != null)
                {
                    SerializeSingleEntity(sb, item);
                }
            }
        }
        else if (data != null)
        {
            SerializeSingleEntity(sb, data);
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    public T Deserialize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не знайдено: {filePath}", filePath);
        }

        string content = File.ReadAllText(filePath, Encoding.UTF8);
        Type targetType = typeof(T);

        Type itemType = targetType.IsArray
            ? targetType.GetElementType()!
            : (targetType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(targetType)
                ? targetType.GetGenericArguments()[0]
                : targetType);

        List<object> parsedItems = ParseEntities(content, itemType);

        if (targetType.IsArray)
        {
            Array resultArray = Array.CreateInstance(itemType, parsedItems.Count);
            for (int i = 0; i < parsedItems.Count; i++)
            {
                resultArray.SetValue(parsedItems[i], i);
            }
            return (T)(object)resultArray;
        }

        if (typeof(System.Collections.IList).IsAssignableFrom(targetType))
        {
            System.Collections.IList list = (System.Collections.IList)Activator.CreateInstance(targetType)!;
            foreach (object item in parsedItems)
            {
                list.Add(item);
            }
            return (T)list;
        }

        if (parsedItems.Count > 0)
        {
            return (T)parsedItems[0];
        }

        throw new InvalidDataException($"Файл {filePath} не містить валідних об'єктів для типу {targetType.Name}");
    }

    private static void SerializeSingleEntity(StringBuilder sb, object entity)
    {
        Type type = entity.GetType();
        string typeName = type.Name.Replace("Entity", "");
        string id = GetEntityIdentifier(entity);

        sb.AppendLine($"{typeName} {id}");
        sb.AppendLine("{");

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite).ToArray();

        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo prop = properties[i];
            object? val = prop.GetValue(entity);
            string strVal = FormatValue(val);
            string comma = (i < properties.Length - 1) ? "," : "";
            sb.AppendLine($"  \"{prop.Name}\": \"{strVal}\"{comma}");
        }

        sb.AppendLine("};");
        sb.AppendLine();
    }

    private static string GetEntityIdentifier(object entity)
    {
        if (entity is PersonEntity person && !string.IsNullOrWhiteSpace(person.Id))
        {
            return person.Id;
        }
        if (entity is BookEntity book && !string.IsNullOrWhiteSpace(book.SerialNumber))
        {
            return book.SerialNumber;
        }

        PropertyInfo? idProp = entity.GetType().GetProperty("Id") ?? entity.GetType().GetProperty("SerialNumber") ?? entity.GetType().GetProperty("StudentId");
        if (idProp != null)
        {
            object? val = idProp.GetValue(entity);
            if (val != null) return val.ToString()!;
        }

        return Guid.NewGuid().ToString("N")[..8];
    }

    private static string FormatValue(object? val)
    {
        if (val == null) return string.Empty;
        if (val is decimal dec) return dec.ToString("G", CultureInfo.InvariantCulture);
        if (val is double dbl) return dbl.ToString("G", CultureInfo.InvariantCulture);
        if (val is float flt) return flt.ToString("G", CultureInfo.InvariantCulture);
        return val.ToString() ?? string.Empty;
    }

    private static List<object> ParseEntities(string content, Type itemType)
    {
        List<object> items = new();
        Regex blockRegex = new(@"\s*(?:[A-Za-z0-9_]+)\s+([^\r\n]+)\s*\{([^}]*)\}\s*;", RegexOptions.Singleline);
        Regex propRegex = new(@"""([^""]+)""\s*:\s*""([^""]*)""");

        MatchCollection matches = blockRegex.Matches(content);
        foreach (Match match in matches)
        {
            string body = match.Groups[2].Value;
            object instance = Activator.CreateInstance(itemType)!;
            Dictionary<string, string> kv = new(StringComparer.OrdinalIgnoreCase);

            foreach (Match pMatch in propRegex.Matches(body))
            {
                kv[pMatch.Groups[1].Value] = pMatch.Groups[2].Value;
            }

            PropertyInfo[] props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).ToArray();

            foreach (PropertyInfo prop in props)
            {
                if (kv.TryGetValue(prop.Name, out string? strVal))
                {
                    object? converted = ConvertValue(strVal, prop.PropertyType);
                    prop.SetValue(instance, converted);
                }
            }

            items.Add(instance);
        }

        return items;
    }

    private static object? ConvertValue(string str, Type targetType)
    {
        if (targetType == typeof(string)) return str;
        if (targetType == typeof(int)) return int.TryParse(str, out int i) ? i : 0;
        if (targetType == typeof(decimal)) return decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal d) ? d : 0m;
        if (targetType == typeof(double)) return double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ? d : 0.0;
        if (targetType == typeof(bool)) return bool.TryParse(str, out bool b) && b;
        return Convert.ChangeType(str, targetType, CultureInfo.InvariantCulture);
    }

    private static void EnsureDirectory(string filePath)
    {
        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}
