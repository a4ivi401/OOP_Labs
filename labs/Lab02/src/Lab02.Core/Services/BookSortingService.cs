using Lab02.Core.Comparers;
using Lab02.Core.Models;
using Lab02.Core.Tree;

namespace Lab02.Core.Services;

public static class BookSortingService
{
    public static Book[] SortByNaturalOrder(Book[] books)
    {
        var copy = (Book[])books.Clone();
        Array.Sort(copy);
        return copy;
    }

    public static Book[] SortByPrice(Book[] books)
    {
        var copy = (Book[])books.Clone();
        Array.Sort(copy, new BookPriceComparer());
        return copy;
    }

    public static Book[] SortByTotalCost(Book[] books)
    {
        var copy = (Book[])books.Clone();
        Array.Sort(copy, new BookTotalCostComparer());
        return copy;
    }

    public static Book[] SortByYear(Book[] books)
    {
        var copy = (Book[])books.Clone();
        Array.Sort(copy, new BookYearComparer());
        return copy;
    }

    public static Book[] SortUsingTree(Book[] books, IComparer<Book>? comparer = null)
    {
        var tree = new BinaryTree<Book>(books, comparer);
        return tree.ToInOrderArray();
    }
}
