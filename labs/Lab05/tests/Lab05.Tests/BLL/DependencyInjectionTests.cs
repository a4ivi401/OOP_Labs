using Lab05.BLL.DependencyInjection;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lab05.Tests.BLL;

public class DependencyInjectionTests
{
    [Fact]
    public void AddLab05Services_ResolvesAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLab05Services();
        var provider = services.BuildServiceProvider();

        // Act & Assert
        Assert.NotNull(provider.GetService<IStudentService>());
        Assert.NotNull(provider.GetService<IDormitoryService>());
        Assert.NotNull(provider.GetService<IDriverLicenseService>());
        Assert.NotNull(provider.GetService<ITaxiDriverService>());
        Assert.NotNull(provider.GetService<IAcrobatService>());

        Assert.NotNull(provider.GetService<IStudentRepository>());
        Assert.NotNull(provider.GetService<ITaxiDriverRepository>());
        Assert.NotNull(provider.GetService<IAcrobatRepository>());

        Assert.NotNull(provider.GetService<IEntityValidator>());

        var studentProviders = provider.GetServices<IDataProvider<StudentEntity>>().ToList();
        Assert.Equal(3, studentProviders.Count); // JSON, Stream, XML
    }
}
