using Lab05.BLL.Interfaces;
using Lab05.BLL.Services;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lab05.BLL.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLab05Services(this IServiceCollection services)
    {
        // 1. Data Providers
        services.AddSingleton<IDataProvider<StudentEntity>, JsonDataProvider<StudentEntity>>();
        services.AddSingleton<IDataProvider<StudentEntity>, StreamDataProvider<StudentEntity>>();
        services.AddSingleton<IDataProvider<StudentEntity>, XmlDataProvider<StudentEntity>>();

        services.AddSingleton<IDataProvider<TaxiDriverEntity>, JsonDataProvider<TaxiDriverEntity>>();
        services.AddSingleton<IDataProvider<TaxiDriverEntity>, StreamDataProvider<TaxiDriverEntity>>();
        services.AddSingleton<IDataProvider<TaxiDriverEntity>, XmlDataProvider<TaxiDriverEntity>>();

        services.AddSingleton<IDataProvider<AcrobatEntity>, JsonDataProvider<AcrobatEntity>>();
        services.AddSingleton<IDataProvider<AcrobatEntity>, StreamDataProvider<AcrobatEntity>>();
        services.AddSingleton<IDataProvider<AcrobatEntity>, XmlDataProvider<AcrobatEntity>>();

        // 2. Data Repositories
        services.AddSingleton<IStudentRepository, StudentRepository>();
        services.AddSingleton<ITaxiDriverRepository, TaxiDriverRepository>();
        services.AddSingleton<IAcrobatRepository, AcrobatRepository>();

        // 3. Validation
        services.AddSingleton<IEntityValidator, EntityValidator>();

        // 4. Business Logic Services
        services.AddTransient<IDriverLicenseService, DriverLicenseService>();
        services.AddTransient<IDormitoryService, DormitoryService>();
        services.AddTransient<IStudentService, StudentService>();
        services.AddTransient<ITaxiDriverService, TaxiDriverService>();
        services.AddTransient<IAcrobatService, AcrobatService>();

        return services;
    }
}
