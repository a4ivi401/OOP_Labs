using System.Collections;
using Lab02.Core.Data;
using Lab02.Core.Models;
using Lab02.Core.Tree;

namespace Lab02.Tests.Tree;

public class BinaryTreeTests
{
    [Fact]
    public void EmptyTree_HasZeroCountAndNullRoot()
    {
        var tree = new BinaryTree<Book>();

        Assert.Equal(0, tree.Count);
        Assert.Null(tree.Root);
    }

    [Fact]
    public void Add_IncreasesCountAndSetsRoot()
    {
        var tree = new BinaryTree<Book>();
        var book = new Book("1", "Кобзар", 1840, 100, 10);

        tree.Add(book);

        Assert.Equal(1, tree.Count);
        Assert.NotNull(tree.Root);
        Assert.Equal(book, tree.Root.Value);
    }

    [Fact]
    public void Contains_ReturnsExpectedResults()
    {
        var books = BookDataSample.CreateInitialBooks();
        var tree = new BinaryTree<Book>(books);

        Assert.True(tree.Contains(books[0]));
        Assert.True(tree.Contains(books[1]));
        Assert.False(tree.Contains(new Book("999", "Невідома", 2000, 100, 10)));
    }

    [Fact]
    public void Clear_ResetsTree()
    {
        var books = BookDataSample.CreateInitialBooks();
        var tree = new BinaryTree<Book>(books);

        tree.Clear();

        Assert.Equal(0, tree.Count);
        Assert.Null(tree.Root);
    }

    [Fact]
    public void PostOrderEnumerator_TraversesTreeInPostOrder()
    {
        var bookRoot = new Book("1", "Кобзар", 1840, 100, 10);
        var bookLeft = new Book("2", "Захар Беркут", 1883, 100, 10);
        var bookRight = new Book("3", "Тіні забутих предків", 1911, 100, 10);
        var bookRightLeft = new Book("4", "Місто", 1928, 100, 10);

        var tree = new BinaryTree<Book>();
        tree.Add(bookRoot);
        tree.Add(bookRight);
        tree.Add(bookRightLeft);
        tree.Add(bookLeft);

        var postOrderList = new List<Book>();
        foreach (Book book in tree)
        {
            postOrderList.Add(book);
        }

        Assert.Equal(4, postOrderList.Count);
        Assert.Equal("Захар Беркут", postOrderList[0].Title);
        Assert.Equal("Місто", postOrderList[1].Title);
        Assert.Equal("Тіні забутих предків", postOrderList[2].Title);
        Assert.Equal("Кобзар", postOrderList[3].Title);
    }

    [Fact]
    public void NonGenericEnumerator_WorksCorrectly()
    {
        var tree = new BinaryTree<Book>(BookDataSample.CreateInitialBooks());
        IEnumerable nonGenericEnumerable = tree;

        int count = 0;
        foreach (object item in nonGenericEnumerable)
        {
            Assert.IsType<Book>(item);
            count++;
        }

        Assert.Equal(4, count);
    }

    [Fact]
    public void Enumerator_ResetAndCurrentBeforeStart_ThrowsOrResets()
    {
        var tree = new BinaryTree<Book>(BookDataSample.CreateInitialBooks());
        using var enumerator = tree.GetEnumerator();

        Assert.Throws<InvalidOperationException>(() => _ = enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.NotNull(enumerator.Current);

        enumerator.Reset();
        Assert.Throws<InvalidOperationException>(() => _ = enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.NotNull(enumerator.Current);
    }

    [Fact]
    public void TraversalArrays_MatchExpectedCounts()
    {
        var tree = new BinaryTree<Book>(BookDataSample.CreateInitialBooks());

        Assert.Equal(4, tree.ToPostOrderArray().Length);
        Assert.Equal(4, tree.ToInOrderArray().Length);
        Assert.Equal(4, tree.ToPreOrderArray().Length);
    }
}
