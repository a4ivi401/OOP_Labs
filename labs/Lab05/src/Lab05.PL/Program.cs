using Lab05.BLL.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Lab05.PL;

public static class Program
{
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();

        // Register BLL and DAL dependencies
        services.AddLab05Services();

        // Register PL presentation menu
        services.AddSingleton<ConsoleMenu>();

        using var serviceProvider = services.BuildServiceProvider();

        var menu = serviceProvider.GetRequiredService<ConsoleMenu>();
        menu.Run();
    }
}
