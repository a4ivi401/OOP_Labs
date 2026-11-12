using Lab01.Core.Common;
using Xunit;

namespace Lab01.Tests.Core;

public class ArrayHelperTests
{
    [Fact]
    public void Add_AppendsElementToEmptyArray()
    {
        int[] original = Array.Empty<int>();
        int[] updated = ArrayHelper.Add(original, 42);

        Assert.Single(updated);
        Assert.Equal(42, updated[0]);
    }

    [Fact]
    public void Add_AppendsElementToEndOfExistingArray()
    {
        string[] original = new[] { "A", "B" };
        string[] updated = ArrayHelper.Add(original, "C");

        Assert.Equal(3, updated.Length);
        Assert.Equal("A", updated[0]);
        Assert.Equal("B", updated[1]);
        Assert.Equal("C", updated[2]);
    }

    [Fact]
    public void RemoveAt_RemovesElementFromMiddle()
    {
        string[] original = new[] { "Київ", "Львів", "Одеса" };
        string[] updated = ArrayHelper.RemoveAt(original, 1);

        Assert.Equal(2, updated.Length);
        Assert.Equal("Київ", updated[0]);
        Assert.Equal("Одеса", updated[1]);
    }

    [Fact]
    public void RemoveAt_RemovesFirstAndLastElement()
    {
        int[] original = new[] { 10, 20, 30 };

        int[] withoutFirst = ArrayHelper.RemoveAt(original, 0);
        Assert.Equal(new[] { 20, 30 }, withoutFirst);

        int[] withoutLast = ArrayHelper.RemoveAt(original, 2);
        Assert.Equal(new[] { 10, 20 }, withoutLast);
    }

    [Fact]
    public void RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        int[] original = new[] { 1, 2 };

        Assert.Throws<ArgumentOutOfRangeException>(() => ArrayHelper.RemoveAt(original, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ArrayHelper.RemoveAt(original, 5));
    }

    [Fact]
    public void Filter_ReturnsMatchingElementsOnly()
    {
        int[] numbers = new[] { 1, 2, 3, 4, 5, 6 };
        int[] evens = ArrayHelper.Filter(numbers, n => n % 2 == 0);

        Assert.Equal(new[] { 2, 4, 6 }, evens);
    }

    [Fact]
    public void Filter_WhenNoMatches_ReturnsEmptyArray()
    {
        string[] cities = new[] { "Київ", "Одеса" };
        string[] matches = ArrayHelper.Filter(cities, c => c == "Львів");

        Assert.Empty(matches);
    }

    [Fact]
    public void Count_CountsCorrectNumberOfMatches()
    {
        int[] numbers = new[] { 10, 25, 30, 45 };
        int greaterThan20 = ArrayHelper.Count(numbers, n => n > 20);

        Assert.Equal(3, greaterThan20);
    }

    [Fact]
    public void FindIndex_ReturnsFirstIndexOrMinusOne()
    {
        string[] names = new[] { "Іван", "Марія", "Петро" };

        int found = ArrayHelper.FindIndex(names, n => n == "Марія");
        int notFound = ArrayHelper.FindIndex(names, n => n == "Олена");

        Assert.Equal(1, found);
        Assert.Equal(-1, notFound);
    }
}
