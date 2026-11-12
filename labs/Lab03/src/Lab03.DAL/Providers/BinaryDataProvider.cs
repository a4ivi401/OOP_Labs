using System.Reflection;
using Lab03.DAL.Entities;

namespace Lab03.DAL.Providers;

public class BinaryDataProvider<T> : IDataProvider<T>
{
    private const uint MagicNumber = 0x4F4F5033; // "OOP3"

    public string FormatName => "Binary";
    public string DefaultExtension => ".bin";

    public void Serialize(string filePath, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        EnsureDirectory(filePath);

        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using BinaryWriter writer = new(stream, System.Text.Encoding.UTF8);

        writer.Write(MagicNumber);
        writer.Write(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName ?? typeof(T).Name);

        WriteValue(writer, data, typeof(T));
    }

    public T Deserialize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не знайдено: {filePath}", filePath);
        }

        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BinaryReader reader = new(stream, System.Text.Encoding.UTF8);

        uint magic = reader.ReadUInt32();
        if (magic != MagicNumber)
        {
            throw new InvalidDataException("Недійсний бінарний формат або сигнатура файлу.");
        }

        string typeName = reader.ReadString();
        object? result = ReadValue(reader, typeof(T));

        return result is T typed ? typed : throw new InvalidDataException($"Помилка десеріалізації бінарного об'єкта {typeName}");
    }

    private static void WriteValue(BinaryWriter writer, object? value, Type type)
    {
        if (value == null)
        {
            writer.Write(false);
            return;
        }

        writer.Write(true);

        if (type == typeof(string))
        {
            writer.Write((string)value);
        }
        else if (type == typeof(int))
        {
            writer.Write((int)value);
        }
        else if (type == typeof(decimal))
        {
            writer.Write((decimal)value);
        }
        else if (type == typeof(double))
        {
            writer.Write((double)value);
        }
        else if (type == typeof(bool))
        {
            writer.Write((bool)value);
        }
        else if (type.IsArray)
        {
            Array array = (Array)value;
            Type elemType = type.GetElementType()!;
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteValue(writer, array.GetValue(i), elemType);
            }
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(type))
        {
            System.Collections.IList list = (System.Collections.IList)value;
            Type elemType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            writer.Write(list.Count);
            foreach (object? item in list)
            {
                WriteValue(writer, item, elemType);
            }
        }
        else
        {
            // Direct entity writers for optimal performance & clean layout
            if (value is BookEntity book)
            {
                writer.Write(book.SerialNumber);
                writer.Write(book.Title);
                writer.Write(book.PublicationYear);
                writer.Write(book.Price);
                writer.Write(book.CopiesCount);
            }
            else if (value is StudentEntity student)
            {
                writer.Write(student.Id);
                writer.Write(student.FirstName);
                writer.Write(student.LastName);
                writer.Write(student.Course);
                writer.Write(student.StudentId);
                writer.Write(student.OriginCity);
                writer.Write(student.Passport);
            }
            else if (value is TaxiDriverEntity driver)
            {
                writer.Write(driver.Id);
                writer.Write(driver.FirstName);
                writer.Write(driver.LastName);
                writer.Write(driver.CarModel);
                writer.Write(driver.LicenseNumber);
                writer.Write(driver.DrivingExperienceYears);
            }
            else if (value is AcrobatEntity acrobat)
            {
                writer.Write(acrobat.Id);
                writer.Write(acrobat.FirstName);
                writer.Write(acrobat.LastName);
                writer.Write(acrobat.CircusOrTroupeName);
                writer.Write(acrobat.Specialization);
            }
            else
            {
                // Fallback for any other custom POCO
                PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite).ToArray();
                writer.Write(props.Length);
                foreach (PropertyInfo prop in props)
                {
                    writer.Write(prop.Name);
                    WriteValue(writer, prop.GetValue(value), prop.PropertyType);
                }
            }
        }
    }

    private static object? ReadValue(BinaryReader reader, Type type)
    {
        bool hasValue = reader.ReadBoolean();
        if (!hasValue)
        {
            return null;
        }

        if (type == typeof(string))
        {
            return reader.ReadString();
        }
        if (type == typeof(int))
        {
            return reader.ReadInt32();
        }
        if (type == typeof(decimal))
        {
            return reader.ReadDecimal();
        }
        if (type == typeof(double))
        {
            return reader.ReadDouble();
        }
        if (type == typeof(bool))
        {
            return reader.ReadBoolean();
        }
        if (type.IsArray)
        {
            Type elemType = type.GetElementType()!;
            int length = reader.ReadInt32();
            Array array = Array.CreateInstance(elemType, length);
            for (int i = 0; i < length; i++)
            {
                array.SetValue(ReadValue(reader, elemType), i);
            }
            return array;
        }
        if (typeof(System.Collections.IList).IsAssignableFrom(type))
        {
            Type elemType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            int count = reader.ReadInt32();
            System.Collections.IList list = (System.Collections.IList)Activator.CreateInstance(type)!;
            for (int i = 0; i < count; i++)
            {
                list.Add(ReadValue(reader, elemType));
            }
            return list;
        }

        if (type == typeof(BookEntity))
        {
            return new BookEntity
            {
                SerialNumber = reader.ReadString(),
                Title = reader.ReadString(),
                PublicationYear = reader.ReadInt32(),
                Price = reader.ReadDecimal(),
                CopiesCount = reader.ReadInt32()
            };
        }
        if (type == typeof(StudentEntity))
        {
            return new StudentEntity
            {
                Id = reader.ReadString(),
                FirstName = reader.ReadString(),
                LastName = reader.ReadString(),
                Course = reader.ReadInt32(),
                StudentId = reader.ReadString(),
                OriginCity = reader.ReadString(),
                Passport = reader.ReadString()
            };
        }
        if (type == typeof(TaxiDriverEntity))
        {
            return new TaxiDriverEntity
            {
                Id = reader.ReadString(),
                FirstName = reader.ReadString(),
                LastName = reader.ReadString(),
                CarModel = reader.ReadString(),
                LicenseNumber = reader.ReadString(),
                DrivingExperienceYears = reader.ReadInt32()
            };
        }
        if (type == typeof(AcrobatEntity))
        {
            return new AcrobatEntity
            {
                Id = reader.ReadString(),
                FirstName = reader.ReadString(),
                LastName = reader.ReadString(),
                CircusOrTroupeName = reader.ReadString(),
                Specialization = reader.ReadString()
            };
        }

        object instance = Activator.CreateInstance(type)!;
        int propCount = reader.ReadInt32();
        Dictionary<string, PropertyInfo> propMap = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);

        for (int i = 0; i < propCount; i++)
        {
            string name = reader.ReadString();
            if (propMap.TryGetValue(name, out PropertyInfo? prop))
            {
                object? val = ReadValue(reader, prop.PropertyType);
                prop.SetValue(instance, val);
            }
        }
        return instance;
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
