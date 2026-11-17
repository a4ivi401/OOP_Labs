using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;

namespace Lab05.Tests.DAL;

public class DataProvidersAndRepositoryTests : IDisposable
{
    private readonly string _tempDir;

    public DataProvidersAndRepositoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "Lab05Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try
            {
                Directory.Delete(_tempDir, true);
            }
            catch
            {
                // Ignore cleanup issues in temp dir
            }
        }
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void JsonDataProvider_Roundtrip_SavesAndLoadsEntities()
    {
        // Arrange
        var provider = new JsonDataProvider<StudentEntity>();
        string path = Path.Combine(_tempDir, "students.json");
        var students = new List<StudentEntity>
        {
            new() { Id = 1, FirstName = "Олександр", LastName = "Шевченко", Course = 1, StudentId = "KB102030", OriginCity = "Львів" },
            new() { Id = 2, FirstName = "Марія", LastName = "Франко", Course = 2, StudentId = "KB102031", OriginCity = "Київ" }
        };

        // Act
        provider.SaveToFile(path, students);
        var loaded = provider.LoadFromFile(path);

        // Assert
        Assert.Equal(2, loaded.Count);
        Assert.Equal("Шевченко", loaded[0].LastName);
        Assert.Equal("KB102030", loaded[0].StudentId);
    }

    [Fact]
    public void StreamDataProvider_Roundtrip_SavesAndLoadsEntities()
    {
        // Arrange
        var provider = new StreamDataProvider<StudentEntity>();
        string path = Path.Combine(_tempDir, "students.txt");
        var students = new List<StudentEntity>
        {
            new() { Id = 1, FirstName = "Тарас", LastName = "Бульба", Course = 1, StudentId = "KB555555", OriginCity = "Полтава" }
        };

        // Act
        provider.SaveToFile(path, students);
        var loaded = provider.LoadFromFile(path);

        // Assert
        Assert.Single(loaded);
        Assert.Equal("Бульба", loaded[0].LastName);
        Assert.Equal("KB555555", loaded[0].StudentId);
    }

    [Fact]
    public void XmlDataProvider_Roundtrip_SavesAndLoadsEntities()
    {
        // Arrange
        var provider = new XmlDataProvider<StudentEntity>();
        string path = Path.Combine(_tempDir, "students.xml");
        var students = new List<StudentEntity>
        {
            new() { Id = 1, FirstName = "Григорій", LastName = "Сковорода", Course = 3, StudentId = "KB777777", OriginCity = "Харків" }
        };

        // Act
        provider.SaveToFile(path, students);
        var loaded = provider.LoadFromFile(path);

        // Assert
        Assert.Single(loaded);
        Assert.Equal("Сковорода", loaded[0].LastName);
        Assert.Equal("KB777777", loaded[0].StudentId);
    }

    [Fact]
    public void EntityRepository_CrudOperations_WorkCorrectly()
    {
        // Arrange
        var provider = new JsonDataProvider<StudentEntity>();
        var repo = new EntityRepository<StudentEntity>(provider);

        var s1 = new StudentEntity { Id = 1, FirstName = "Тест", LastName = "Студент", StudentId = "KB123456" };
        var s2 = new StudentEntity { Id = 2, FirstName = "Другий", LastName = "Студент", StudentId = "KB654321" };

        // Act: Add
        repo.Add(s1);
        repo.Add(s2);

        // Assert: Count and GetById
        Assert.Equal(2, repo.Count);
        Assert.Equal("Тест", repo.GetById(1)?.FirstName);

        // Act: Update
        s1.FirstName = "Оновлений";
        bool updated = repo.Update(s1);
        Assert.True(updated);
        Assert.Equal("Оновлений", repo.GetById(1)?.FirstName);

        // Act: Delete
        bool deleted = repo.Delete(2);
        Assert.True(deleted);
        Assert.Equal(1, repo.Count);
        Assert.Null(repo.GetById(2));

        // Act: Clear
        repo.Clear();
        Assert.Equal(0, repo.Count);
    }

    [Fact]
    public void StudentRepository_GetByStudentId_FindsCorrectStudent()
    {
        // Arrange
        var provider = new JsonDataProvider<StudentEntity>();
        var repo = new StudentRepository(provider);
        repo.Add(new StudentEntity { FirstName = "Тарас", LastName = "Шевченко", StudentId = "KB102030" });
        repo.Add(new StudentEntity { FirstName = "Леся", LastName = "Українка", StudentId = "KB102031" });

        // Act
        var found = repo.GetByStudentId("kb102030");
        var notFound = repo.GetByStudentId("KB999999");

        // Assert
        Assert.NotNull(found);
        Assert.Equal("Шевченко", found.LastName);
        Assert.Null(notFound);
    }

    [Fact]
    public void TaxiDriverRepository_GetByLicenseNumber_FindsCorrectDriver()
    {
        // Arrange
        var provider = new JsonDataProvider<TaxiDriverEntity>();
        var repo = new TaxiDriverRepository(provider);
        repo.Add(new TaxiDriverEntity { FirstName = "Іван", LastName = "Сірко", LicenseNumber = "BTA123456" });

        // Act
        var found = repo.GetByLicenseNumber("bta123456");
        var notFound = repo.GetByLicenseNumber("BTA999999");

        // Assert
        Assert.NotNull(found);
        Assert.Equal("Сірко", found.LastName);
        Assert.Null(notFound);
    }

    [Fact]
    public void Providers_FileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var json = new JsonDataProvider<StudentEntity>();
        var stream = new StreamDataProvider<StudentEntity>();
        var xml = new XmlDataProvider<StudentEntity>();
        string nonExistent = Path.Combine(_tempDir, "missing.file");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => json.LoadFromFile(nonExistent));
        Assert.Throws<FileNotFoundException>(() => stream.LoadFromFile(nonExistent));
        Assert.Throws<FileNotFoundException>(() => xml.LoadFromFile(nonExistent));
    }
}
