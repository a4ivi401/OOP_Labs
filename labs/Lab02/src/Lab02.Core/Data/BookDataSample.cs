using Lab02.Core.Models;

namespace Lab02.Core.Data;

public static class BookDataSample
{
    public static Book[] CreateInitialBooks()
    {
        return
        [
            new Book("978-966-03-8201-5", "Кобзар", 1840, 350.00m, 5000),
            new Book("978-617-7654-12-3", "Тіні забутих предків", 1911, 220.00m, 3000),
            new Book("978-966-14-9123-4", "Місто", 1928, 280.00m, 4000),
            new Book("978-617-09-5678-9", "Захар Беркут", 1883, 195.00m, 6000)
        ];
    }
}
