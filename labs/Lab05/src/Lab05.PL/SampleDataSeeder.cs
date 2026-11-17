using Lab05.BLL.Interfaces;
using Lab05.BLL.Models;

namespace Lab05.PL;

public static class SampleDataSeeder
{
    public static void SeedDemoData(
        IStudentService studentService,
        ITaxiDriverService taxiService,
        IAcrobatService acrobatService,
        IDormitoryService dormitoryService)
    {
        studentService.Clear();

        // 1. Students: 4 first-year students (3 from other cities, 1 from Kyiv -> 75% non-local)
        var s1 = new StudentModel
        {
            FirstName = "Олександр",
            LastName = "Шевченко",
            Course = 1,
            StudentId = "KB102030",
            OriginCity = "Львів",
            Passport = "СН123456"
        };
        studentService.Add(s1);

        var s2 = new StudentModel
        {
            FirstName = "Марія",
            LastName = "Коваленко",
            Course = 1,
            StudentId = "KB102031",
            OriginCity = "Одеса",
            Passport = "123456789"
        };
        studentService.Add(s2);

        var s3 = new StudentModel
        {
            FirstName = "Дмитро",
            LastName = "Бондаренко",
            Course = 1,
            StudentId = "KB102032",
            OriginCity = "Київ", // Local student
            Passport = "АА654321"
        };
        studentService.Add(s3);

        var s4 = new StudentModel
        {
            FirstName = "Анна",
            LastName = "Мельник",
            Course = 1,
            StudentId = "KB102033",
            OriginCity = "Полтава",
            Passport = "987654321"
        };
        studentService.Add(s4);

        // 2nd and 3rd course students
        var s5 = new StudentModel
        {
            FirstName = "Іван",
            LastName = "Кравченко",
            Course = 2,
            StudentId = "KB203040",
            OriginCity = "Харків",
            Passport = "ВВ789012"
        };
        studentService.Add(s5);

        var s6 = new StudentModel
        {
            FirstName = "Олена",
            LastName = "Ткаченко",
            Course = 3,
            StudentId = "KB304050",
            OriginCity = "Чернігів",
            Passport = "СС345678"
        };
        studentService.Add(s6);

        // Seed 1 dormitory settlement for demo
        try
        {
            dormitoryService.SettleInDormitory("KB102030", 4, 312, "Київ");
        }
        catch
        {
            // Ignore if already settled
        }

        // 2. Taxi Drivers
        if (taxiService.GetAll().Count == 0)
        {
            taxiService.Add(new TaxiDriverModel
            {
                FirstName = "Василь",
                LastName = "Петренко",
                CarModel = "Skoda Octavia",
                LicenseNumber = "BTA543210",
                ExperienceYears = 7,
                Age = 32,
                HasMedicalCertificate = true,
                HasActiveLicense = true
            });

            taxiService.Add(new TaxiDriverModel
            {
                FirstName = "Сергій",
                LastName = "Григоренко",
                CarModel = "Toyota Prius",
                LicenseNumber = "CXR987654",
                ExperienceYears = 3,
                Age = 24,
                HasMedicalCertificate = true,
                HasActiveLicense = true
            });
        }

        // 3. Acrobats
        if (acrobatService.GetAll().Count == 0)
        {
            acrobatService.Add(new AcrobatModel
            {
                FirstName = "Вікторія",
                LastName = "Лисенко",
                CircusOrTroupe = "Національний цирк України",
                Specialization = "Повітряна гімнастика",
                SafetyCertificationValid = true,
                PerformancesCount = 42
            });

            acrobatService.Add(new AcrobatModel
            {
                FirstName = "Максим",
                LastName = "Дорошенко",
                CircusOrTroupe = "Кобзов",
                Specialization = "Еквілібристика на канатах",
                SafetyCertificationValid = true,
                PerformancesCount = 28
            });
        }
    }
}
