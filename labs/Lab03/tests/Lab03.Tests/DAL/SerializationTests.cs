using Lab03.DAL.Entities;
using Lab03.DAL.Providers;

namespace Lab03.Tests.DAL;

public class SerializationTests : IDisposable
{
    private readonly string _tempDir;

    public SerializationTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "Lab03_Test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    private static BookEntity[] CreateSampleBooks()
    {
        return new BookEntity[]
        {
            new("ISBN-001", "Clean Code", 2008, 450.00m, 1500),
            new("ISBN-002", "Design Patterns", 1994, 620.00m, 1200),
            new("ISBN-003", "Refactoring", 1999, 500.00m, 900),
            new("ISBN-004", "The Pragmatic Programmer", 1999, 550.00m, 1100)
        };
    }

    [Theory]
    [InlineData("json")]
    [InlineData("xml")]
    [InlineData("bin")]
    [InlineData("custom")]
    public void ArraySerialization_ShouldRestoreExactData_ForAllProviders(string providerType)
    {
        BookEntity[] original = CreateSampleBooks();
        IDataProvider<BookEntity[]> provider = GetBookArrayProvider(providerType);
        string filePath = Path.Combine(_tempDir, $"test_books.{provider.DefaultExtension}");

        provider.Serialize(filePath, original);
        Assert.True(File.Exists(filePath));
        Assert.True(new FileInfo(filePath).Length > 0);

        BookEntity[] restored = provider.Deserialize(filePath);
        Assert.NotNull(restored);
        Assert.Equal(original.Length, restored.Length);

        for (int i = 0; i < original.Length; i++)
        {
            Assert.Equal(original[i].SerialNumber, restored[i].SerialNumber);
            Assert.Equal(original[i].Title, restored[i].Title);
            Assert.Equal(original[i].PublicationYear, restored[i].PublicationYear);
            Assert.Equal(original[i].Price, restored[i].Price);
            Assert.Equal(original[i].CopiesCount, restored[i].CopiesCount);
        }
    }

    [Theory]
    [InlineData("json")]
    [InlineData("xml")]
    [InlineData("bin")]
    [InlineData("custom")]
    public void ListSerialization_ShouldRestoreExactData_ForAllProviders(string providerType)
    {
        List<BookEntity> original = CreateSampleBooks().ToList();
        IDataProvider<List<BookEntity>> provider = GetBookListProvider(providerType);
        string filePath = Path.Combine(_tempDir, $"test_list.{provider.DefaultExtension}");

        provider.Serialize(filePath, original);
        Assert.True(File.Exists(filePath));

        List<BookEntity> restored = provider.Deserialize(filePath);
        Assert.NotNull(restored);
        Assert.Equal(original.Count, restored.Count);

        for (int i = 0; i < original.Count; i++)
        {
            Assert.Equal(original[i].SerialNumber, restored[i].SerialNumber);
            Assert.Equal(original[i].Title, restored[i].Title);
            Assert.Equal(original[i].Price, restored[i].Price);
        }
    }

    [Fact]
    public void StudentEntityArray_ShouldRoundtrip_AcrossAllProviders()
    {
        StudentEntity[] students = new StudentEntity[]
        {
            new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456"),
            new("2", "Леся", "Українка", 1, "KB223344", "Новоград", "МЕ654321")
        };

        IDataProvider<StudentEntity[]>[] providers = new IDataProvider<StudentEntity[]>[]
        {
            new JsonDataProvider<StudentEntity[]>(),
            new XmlDataProvider<StudentEntity[]>(),
            new BinaryDataProvider<StudentEntity[]>(),
            new CustomDataProvider<StudentEntity[]>()
        };

        foreach (IDataProvider<StudentEntity[]> provider in providers)
        {
            string path = Path.Combine(_tempDir, $"students_{provider.FormatName}{provider.DefaultExtension}");
            provider.Serialize(path, students);
            StudentEntity[] loaded = provider.Deserialize(path);

            Assert.Equal(students.Length, loaded.Length);
            Assert.Equal(students[0].StudentId, loaded[0].StudentId);
            Assert.Equal(students[0].OriginCity, loaded[0].OriginCity);
        }
    }

    [Fact]
    public void Deserialize_NonExistentFile_ShouldThrowFileNotFoundException()
    {
        JsonDataProvider<BookEntity[]> provider = new();
        string nonExistent = Path.Combine(_tempDir, "absent.json");

        Assert.Throws<FileNotFoundException>(() => provider.Deserialize(nonExistent));
    }

    private static IDataProvider<BookEntity[]> GetBookArrayProvider(string type) => type switch
    {
        "json" => new JsonDataProvider<BookEntity[]>(),
        "xml" => new XmlDataProvider<BookEntity[]>(),
        "bin" => new BinaryDataProvider<BookEntity[]>(),
        "custom" => new CustomDataProvider<BookEntity[]>(),
        _ => throw new ArgumentException("Unknown provider")
    };

    private static IDataProvider<List<BookEntity>> GetBookListProvider(string type) => type switch
    {
        "json" => new JsonDataProvider<List<BookEntity>>(),
        "xml" => new XmlDataProvider<List<BookEntity>>(),
        "bin" => new BinaryDataProvider<List<BookEntity>>(),
        "custom" => new CustomDataProvider<List<BookEntity>>(),
        _ => throw new ArgumentException("Unknown provider")
    };
}
