using Lab01.Core.IO;
using Lab01.Core.Models;
using Xunit;

namespace Lab01.Tests.IO;

public class CustomFileServiceTests : IDisposable
{
    private readonly string _testFilePath;

    public CustomFileServiceTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_students_{Guid.NewGuid():N}.txt");
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            try
            {
                File.Delete(_testFilePath);
            }
            catch
            {
                // Ігноруємо помилки очищення тимчасового файлу
            }
        }
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void SaveAndLoadStudents_RoundTrip_PreservesAllData()
    {
        Student[] originalStudents = new Student[]
        {
            new("Олександр", "Шевченко", 18, 1, "КВ101010", "Львів", "МЕ101010"),
            new("Марія", "Коваленко", 19, 2, "КВ202020", "Київ", "МЕ202020")
        };

        // Зберігаємо у файл за допомогою FileStream і StreamWriter
        CustomFileService.SaveStudents(_testFilePath, originalStudents);
        Assert.True(File.Exists(_testFilePath));

        // Зчитуємо назад за допомогою FileStream і StreamReader
        Student[] loadedStudents = CustomFileService.LoadStudents(_testFilePath);

        Assert.Equal(originalStudents.Length, loadedStudents.Length);

        for (int i = 0; i < originalStudents.Length; i++)
        {
            Assert.Equal(originalStudents[i].FirstName, loadedStudents[i].FirstName);
            Assert.Equal(originalStudents[i].LastName, loadedStudents[i].LastName);
            Assert.Equal(originalStudents[i].Age, loadedStudents[i].Age);
            Assert.Equal(originalStudents[i].Course, loadedStudents[i].Course);
            Assert.Equal(originalStudents[i].StudentId, loadedStudents[i].StudentId);
            Assert.Equal(originalStudents[i].CityOfOrigin, loadedStudents[i].CityOfOrigin);
            Assert.Equal(originalStudents[i].Passport, loadedStudents[i].Passport);
        }
    }

    [Fact]
    public void LoadStudents_NonExistentFile_ThrowsFileNotFoundException()
    {
        string nonExistentPath = Path.Combine(Path.GetTempPath(), $"non_existent_{Guid.NewGuid():N}.txt");

        Assert.Throws<FileNotFoundException>(() => CustomFileService.LoadStudents(nonExistentPath));
    }
}
