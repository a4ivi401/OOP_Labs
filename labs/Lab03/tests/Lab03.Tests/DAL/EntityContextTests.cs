using Lab03.DAL.Context;
using Lab03.DAL.Entities;
using Lab03.DAL.Providers;

namespace Lab03.Tests.DAL;

public class EntityContextTests : IDisposable
{
    private readonly string _tempDir;

    public EntityContextTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "ContextTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Fact]
    public void CreateOrOpenFile_ShouldCreateFile_WhenDoesNotExist()
    {
        EntityContext<StudentEntity[]> context = new(new JsonDataProvider<StudentEntity[]>());
        string path = Path.Combine(_tempDir, "new_file.json");

        Assert.False(context.FileExists(path));
        context.CreateOrOpenFile(path);

        Assert.True(context.FileExists(path));
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void SaveAndLoad_ShouldWorkProperly()
    {
        EntityContext<StudentEntity[]> context = new(new JsonDataProvider<StudentEntity[]>());
        string path = Path.Combine(_tempDir, "students_ctx.json");

        StudentEntity[] sample = new StudentEntity[]
        {
            new("1", "Іван", "Франко", 1, "KB123456", "Львів", "СН123456")
        };

        context.Save(path, sample);
        Assert.True(context.GetFileSize(path) > 0);

        StudentEntity[] loaded = context.Load(path);
        Assert.Single(loaded);
        Assert.Equal("KB123456", loaded[0].StudentId);
    }

    [Fact]
    public void DeleteFile_ShouldDeleteExistingFile()
    {
        EntityContext<StudentEntity[]> context = new(new JsonDataProvider<StudentEntity[]>());
        string path = Path.Combine(_tempDir, "to_delete.json");

        context.CreateOrOpenFile(path);
        Assert.True(context.FileExists(path));

        bool deleted = context.DeleteFile(path);
        Assert.True(deleted);
        Assert.False(context.FileExists(path));
    }
}
