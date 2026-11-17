using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab05.DAL.Providers;

public class StreamDataProvider<T> : IDataProvider<T>
{
    public string FormatName => "Stream (Custom Text)";

    public void SaveToFile(string filePath, IEnumerable<T> items)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(items);

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new StreamWriter(fileStream, Encoding.UTF8);

        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            SerializeSingleEntity(writer, item);
        }
    }

    public List<T> LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не знайдено: '{filePath}'", filePath);
        }

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(fileStream, Encoding.UTF8);

        string content = reader.ReadToEnd();
        var resultList = new List<T>();
        Type itemType = typeof(T);

        // Pattern matches: EntityType Identifier \n { attributes... };
        var blockMatches = Regex.Matches(
            content,
            @"(\w+)\s+([^\r\n{]+)\s*\{([^}]*)\}\s*;",
            RegexOptions.Singleline);

        foreach (Match blockMatch in blockMatches)
        {
            string typeName = blockMatch.Groups[1].Value.Trim();
            string rawBody = blockMatch.Groups[3].Value;

            Type? actualType = ResolveType(typeName, itemType);
            if (actualType is null || !itemType.IsAssignableFrom(actualType))
            {
                continue;
            }

            object? instance = Activator.CreateInstance(actualType);
            if (instance is null)
            {
                continue;
            }

            var propMatches = Regex.Matches(rawBody, @"""([^""]+)""\s*:\s*""([^""]*)""");
            foreach (Match propMatch in propMatches)
            {
                string propName = propMatch.Groups[1].Value;
                string propValue = propMatch.Groups[2].Value;

                PropertyInfo? prop = actualType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop is not null && prop.CanWrite)
                {
                    object? converted = ConvertValue(propValue, prop.PropertyType);
                    prop.SetValue(instance, converted);
                }
            }

            if (instance is T typedInstance)
            {
                resultList.Add(typedInstance);
            }
        }

        return resultList;
    }

    private static void SerializeSingleEntity(StreamWriter writer, object item)
    {
        Type type = item.GetType();
        string typeName = type.Name.Replace("Entity", string.Empty);

        PropertyInfo? idProp = type.GetProperty("Id") ?? type.GetProperty("StudentId");
        string identifier = idProp?.GetValue(item)?.ToString() ?? Guid.NewGuid().ToString("N")[..8];

        writer.WriteLine($"{typeName} {identifier}");
        writer.WriteLine("{");

        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToArray();

        for (int i = 0; i < props.Length; i++)
        {
            var p = props[i];
            object? val = p.GetValue(item);
            string formattedVal = FormatValue(val);
            string comma = (i < props.Length - 1) ? "," : string.Empty;
            writer.WriteLine($"  \"{p.Name}\": \"{formattedVal}\"{comma}");
        }

        writer.WriteLine("};");
        writer.WriteLine();
    }

    private static string FormatValue(object? value)
    {
        if (value is null) return string.Empty;
        if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        if (value is IFormattable formattable) return formattable.ToString(null, CultureInfo.InvariantCulture);
        return value.ToString() ?? string.Empty;
    }

    private static object? ConvertValue(string stringValue, Type targetType)
    {
        Type underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (string.IsNullOrEmpty(stringValue))
        {
            return Nullable.GetUnderlyingType(targetType) is not null ? null : (underlying.IsValueType ? Activator.CreateInstance(underlying) : null);
        }

        if (underlying == typeof(string)) return stringValue;
        if (underlying == typeof(int)) return int.Parse(stringValue, CultureInfo.InvariantCulture);
        if (underlying == typeof(long)) return long.Parse(stringValue, CultureInfo.InvariantCulture);
        if (underlying == typeof(double)) return double.Parse(stringValue, CultureInfo.InvariantCulture);
        if (underlying == typeof(decimal)) return decimal.Parse(stringValue, CultureInfo.InvariantCulture);
        if (underlying == typeof(bool)) return bool.Parse(stringValue);
        if (underlying == typeof(DateTime)) return DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
        if (underlying.IsEnum) return Enum.Parse(underlying, stringValue);

        return Convert.ChangeType(stringValue, underlying, CultureInfo.InvariantCulture);
    }

    private static Type? ResolveType(string typeName, Type fallbackType)
    {
        if (fallbackType.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
            fallbackType.Name.Equals($"{typeName}Entity", StringComparison.OrdinalIgnoreCase))
        {
            return fallbackType;
        }

        var asm = fallbackType.Assembly;
        var found = asm.GetTypes().FirstOrDefault(t =>
            t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
            t.Name.Equals($"{typeName}Entity", StringComparison.OrdinalIgnoreCase));

        return found ?? fallbackType;
    }
}
