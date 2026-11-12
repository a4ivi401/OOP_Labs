using Lab02.Core.Data;
using Lab02.Core.Models;

namespace Lab02.Core.Collections;

public class CollectionsComparisonManager
{
    public IBookCollectionOperations ArrayService { get; }
    public IBookCollectionOperations ArrayListService { get; }
    public IBookCollectionOperations GenericListService { get; }

    public CollectionsComparisonManager()
    {
        Book[] initialBooks = BookDataSample.CreateInitialBooks();
        ArrayService = new ArrayBookService(initialBooks);
        ArrayListService = new ArrayListBookService(initialBooks);
        GenericListService = new GenericListBookService(initialBooks);
    }

    public static string[] GetTheoreticalComparisonPoints()
    {
        return
        [
            "1. Безпека типів (Type Safety):",
            "   - Book[]: повна безпека на етапі компіляції та виконання.",
            "   - ArrayList: відсутня на етапі компіляції (зберігає System.Object), ризик InvalidCastException під час виконання.",
            "   - List<Book>: повна статична типізація на етапі компіляції без потреби приведення типів.",
            "",
            "2. Упаковка та розпаковка (Boxing/Unboxing):",
            "   - Book[]: відсутня для посилальних типів; для значущих типів виділення пам'яті в купі не відбувається.",
            "   - ArrayList: значущі типи обов'язково пакуються в об'єкт (boxing), що створює значне навантаження на Garbage Collector.",
            "   - List<Book>: працює без упаковки, оптимізований як для посилальних, так і для значущих типів.",
            "",
            "3. Виділення пам'яті та зміна розміру:",
            "   - Book[]: фіксований розмір, зміна вимагає ручного виділення нового масиву (new Book[n]) та копіювання (Array.Copy).",
            "   - ArrayList: автоматичне динамічне розширення внутрішнього масиву object[] при заповненні (з коефіцієнтом 2).",
            "   - List<Book>: автоматичне розширення строго типізованого масиву Book[] (подвоєння Capacity при вичерпанні)."
        ];
    }
}
