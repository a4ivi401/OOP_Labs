using System.Text;
using Lab02.Core.Collections;
using Lab02.Core.Data;
using Lab02.Core.Models;
using Lab02.Core.Services;
using Lab02.Core.Tree;

namespace Lab02.App;

public class ConsoleMenu
{
    private readonly CollectionsComparisonManager _comparisonManager;
    private readonly BinaryTree<Book> _tree;
    private Book[] _books;

    public ConsoleMenu()
    {
        _books = BookDataSample.CreateInitialBooks();
        _comparisonManager = new CollectionsComparisonManager();
        _tree = new BinaryTree<Book>(_books);
    }

    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        bool running = true;
        while (running)
        {
            PrintHeader();
            Console.WriteLine("1. Показати список книг (Варіант 2, 4 об'єкти)");
            Console.WriteLine("2. Збільшити вартість книги на відсоток");
            Console.WriteLine("3. Порівняти операції: Book[], ArrayList, List<Book>");
            Console.WriteLine("4. Порівняльний аналіз колекцій (теорія та відмінності)");
            Console.WriteLine("5. Бінарне дерево: зворотній обхід (Postorder - власний ітератор)");
            Console.WriteLine("6. Демонстрація сортування (IComparable та IComparer)");
            Console.WriteLine("7. Додати нову книгу до системи");
            Console.WriteLine("0. Вихід");
            Console.WriteLine("====================================================================");
            Console.Write("Оберіть пункт меню (0-7): ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    DisplayBooks(_books, "Поточний список книг");
                    break;
                case "2":
                    IncreaseBookPrice();
                    break;
                case "3":
                    DemonstrateCollectionsOperations();
                    break;
                case "4":
                    DisplayTheoreticalComparison();
                    break;
                case "5":
                    DemonstrateBinaryTreePostOrder();
                    break;
                case "6":
                    DemonstrateSorting();
                    break;
                case "7":
                    AddNewBook();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("Завершення роботи застосунку.");
                    break;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }

            if (running)
            {
                Console.WriteLine();
                Console.Write("Натисніть Enter, щоб продовжити...");
                Console.ReadLine();
                Console.WriteLine();
            }
        }
    }

    private static void PrintHeader()
    {
        Console.WriteLine("====================================================================");
        Console.WriteLine("           Лабораторна робота 2 (Варіант 2: Клас Книга)           ");
        Console.WriteLine("      Дослідження колекцій, узагальнень та бінарного дерева         ");
        Console.WriteLine("====================================================================");
    }

    private static void DisplayBooks(IEnumerable<Book> books, string title)
    {
        Console.WriteLine($"--- {title} ---");
        Console.WriteLine($"{"Серійний номер",-20} | {"Назва",-24} | {"Рік",-5} | {"Ціна (грн)",-12} | {"Тираж",-8} | {"Вартість тиражу",-16}");
        Console.WriteLine(new string('-', 95));

        foreach (Book book in books)
        {
            Console.WriteLine($"{book.SerialNumber,-20} | {book.Title,-24} | {book.PublicationYear,-5} | {book.Price,-12:F2} | {book.CopiesCount,-8} | {book.TotalCirculationCost,-16:F2}");
        }
    }

    private void IncreaseBookPrice()
    {
        DisplayBooks(_books, "Оберіть книгу для збільшення вартості");
        Console.Write("Введіть серійний номер книги: ");
        string? serial = Console.ReadLine()?.Trim();

        Book? book = Array.Find(_books, b => string.Equals(b.SerialNumber, serial, StringComparison.OrdinalIgnoreCase));
        if (book is null)
        {
            Console.WriteLine("Книгу з таким номером не знайдено.");
            return;
        }

        Console.Write("Введіть відсоток збільшення ціни (наприклад, 15): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal percent) || percent <= 0)
        {
            Console.WriteLine("Некоректний відсоток. Значення має бути більшим за 0.");
            return;
        }

        decimal oldPrice = book.Price;
        decimal oldTotal = book.TotalCirculationCost;
        book.IncreasePrice(percent);

        Console.WriteLine($"Ціну успішно збільшено на {percent}%!");
        Console.WriteLine($"Попередня ціна: {oldPrice:F2} грн -> Нова ціна: {book.Price:F2} грн");
        Console.WriteLine($"Попередня вартість тиражу: {oldTotal:F2} грн -> Нова: {book.TotalCirculationCost:F2} грн");
    }

    private void DemonstrateCollectionsOperations()
    {
        Console.WriteLine("--- Демонстрація операцій над трьома типами колекцій ---");
        var testBook = new Book("978-966-01-0001-1", "Енеїда", 1798, 260.00m, 4500);

        IBookCollectionOperations[] services =
        [
            _comparisonManager.ArrayService,
            _comparisonManager.ArrayListService,
            _comparisonManager.GenericListService
        ];

        foreach (var service in services)
        {
            Console.WriteLine($"\nКолекція: {service.CollectionTypeName}");
            Console.WriteLine($"  Початкова кількість: {service.Count}");

            service.Add(testBook);
            Console.WriteLine($"  Додано книгу '{testBook.Title}'. Нова кількість: {service.Count}");

            Book? found = service.Find(testBook.SerialNumber);
            Console.WriteLine($"  Пошук за номером {testBook.SerialNumber}: {(found != null ? "Знайдено (" + found.Title + ")" : "Не знайдено")}");

            bool updated = service.Update(testBook.SerialNumber, "Енеїда (Нове видання)", 290.00m, 5000);
            Console.WriteLine($"  Оновлення даних книги: {(updated ? "Успішно" : "Помилка")}");

            bool removed = service.Remove(testBook.SerialNumber);
            Console.WriteLine($"  Видалення книги: {(removed ? "Успішно" : "Помилка")}. Залишок: {service.Count}");
        }
    }

    private static void DisplayTheoreticalComparison()
    {
        Console.WriteLine("--- Порівняльний аналіз масивів та колекцій C# ---");
        foreach (string line in CollectionsComparisonManager.GetTheoreticalComparisonPoints())
        {
            Console.WriteLine(line);
        }
    }

    private void DemonstrateBinaryTreePostOrder()
    {
        Console.WriteLine("--- Бінарне дерево пошуку (BinaryTree<Book>) ---");
        Console.WriteLine($"Кількість елементів у дереві: {_tree.Count}");
        Console.WriteLine("Порядок обходу за варіантом 2: Зворотній порядок (Postorder: Left -> Right -> Root)");
        Console.WriteLine("Обхід виконується через власний ітератор (PostOrderTreeEnumerator):");
        Console.WriteLine();

        int step = 1;
        foreach (Book book in _tree)
        {
            Console.WriteLine($"  {step++}. {book}");
        }
    }

    private void DemonstrateSorting()
    {
        Console.WriteLine("--- Демонстрація сортування за допомогою IComparable та IComparer ---");

        Console.WriteLine("\n1. За замовчуванням (IComparable<Book> - за назвою книги):");
        DisplayBooks(BookSortingService.SortByNaturalOrder(_books), "Сортування за назвою");

        Console.WriteLine("\n2. Через IComparer<Book> (BookPriceComparer - за зростанням вартості примірника):");
        DisplayBooks(BookSortingService.SortByPrice(_books), "Сортування за ціною");

        Console.WriteLine("\n3. Через IComparer<Book> (BookYearComparer - за роком видання):");
        DisplayBooks(BookSortingService.SortByYear(_books), "Сортування за роком видання");

        Console.WriteLine("\n4. Через IComparer<Book> (BookTotalCostComparer - за загальною вартістю тиражу):");
        DisplayBooks(BookSortingService.SortByTotalCost(_books), "Сортування за вартістю тиражу");

        Console.WriteLine("\n5. Сортування за допомогою бінарного дерева (In-Order Tree Sort):");
        DisplayBooks(BookSortingService.SortUsingTree(_books), "In-Order Tree Sort");
    }

    private void AddNewBook()
    {
        Console.WriteLine("--- Додавання нової книги ---");

        Console.Write("Введіть серійний номер (ISBN): ");
        string serialNumber = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Введіть назву книги: ");
        string title = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Введіть рік видання: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Некоректний рік.");
            return;
        }

        Console.Write("Введіть вартість примірника (грн): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Некоректна вартість.");
            return;
        }

        Console.Write("Введіть кількість примірників: ");
        if (!int.TryParse(Console.ReadLine(), out int copies))
        {
            Console.WriteLine("Некоректна кількість.");
            return;
        }

        try
        {
            var newBook = new Book(serialNumber, title, year, price, copies);

            var updatedBooks = new Book[_books.Length + 1];
            Array.Copy(_books, updatedBooks, _books.Length);
            updatedBooks[^1] = newBook;
            _books = updatedBooks;

            _tree.Add(newBook);
            _comparisonManager.ArrayService.Add(newBook);
            _comparisonManager.ArrayListService.Add(newBook);
            _comparisonManager.GenericListService.Add(newBook);

            Console.WriteLine("Книгу успішно додано до масиву, колекцій та дерева!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка створення книги: {ex.Message}");
        }
    }
}
