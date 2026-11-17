using System;
using Lab04.Core.Analysis;
using Lab04.Core.Delegates;
using Xunit;

namespace Lab04.Tests.Delegates;

public class CharacterCounterTests
{
    [Theory]
    [InlineData("Hello, World!", 'o', 2)]
    [InlineData("Hello, World!", 'l', 3)]
    [InlineData("Hello, World!", 'z', 0)]
    [InlineData("Програмування мовою C#", 'о', 3)]
    [InlineData("Програмування мовою C#", 'а', 2)]
    [InlineData("aaaaa", 'a', 5)]
    [InlineData("", 'a', 0)]
    [InlineData(null, 'a', 0)]
    public void CountOccurrencesNamed_ReturnsCorrectCount(string? source, char targetChar, int expected)
    {
        // Act
        int actual = CharacterCounter.CountOccurrencesNamed(source, targetChar);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("AbraCadabrA", 'a', true, 5)]
    [InlineData("AbraCadabrA", 'a', false, 3)]
    [InlineData("Київ - столиця України, Київ прекрасний", 'к', true, 4)]
    [InlineData("Київ - столиця України, Київ прекрасний", 'к', false, 2)]
    [InlineData("Тест ТЕСТ тест", 'т', true, 6)]
    [InlineData("Тест ТЕСТ тест", 'т', false, 3)]
    [InlineData("", 'x', true, 0)]
    [InlineData(null, 'x', true, 0)]
    public void CountOccurrencesNamedWithOptions_HandlesCaseSensitivity(string? source, char targetChar, bool ignoreCase, int expected)
    {
        // Act
        int actual = CharacterCounter.CountOccurrencesNamedWithOptions(source, targetChar, ignoreCase);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AnonymousMethod_ReturnsIdenticalResultToNamedMethod()
    {
        // Arrange
        const string text = "Делегати, анонімні методи та лямбда-вирази у C#";
        const char target = 'а';
        CharacterCountDelegate anonymousCounter = CharacterCounter.CreateAnonymousMethod();

        // Act
        int namedResult = CharacterCounter.CountOccurrencesNamed(text, target);
        int anonResult = anonymousCounter(text, target);

        // Assert
        Assert.Equal(namedResult, anonResult);
        Assert.Equal(5, anonResult);
    }

    [Fact]
    public void AnonymousMethodWithOptions_CorrectlyIgnoresCase()
    {
        // Arrange
        const string text = "Делегати ДЕЛЕГАТИ делегати";
        const char target = 'д';
        CharacterCountWithOptionsDelegate anonCounter = CharacterCounter.CreateAnonymousMethodWithOptions();

        // Act
        int caseInsensitive = anonCounter(text, target, true);
        int caseSensitive = anonCounter(text, target, false);

        // Assert
        Assert.Equal(3, caseInsensitive);
        Assert.Equal(1, caseSensitive);
    }

    [Fact]
    public void LambdaExpression_ReturnsIdenticalResultToNamedMethod()
    {
        // Arrange
        const string text = "Object-Oriented Programming with C# Events and Delegates";
        const char target = 'e';
        CharacterCountDelegate lambdaCounter = CharacterCounter.CreateLambdaExpression();
        CharacterCountDelegate exprBodied = CharacterCounter.CreateExpressionBodiedLambda();

        // Act
        int namedResult = CharacterCounter.CountOccurrencesNamed(text, target);
        int lambdaResult = lambdaCounter(text, target);
        int exprResult = exprBodied(text, target);

        // Assert
        Assert.Equal(namedResult, lambdaResult);
        Assert.Equal(namedResult, exprResult);
        Assert.Equal(7, lambdaResult);
    }

    [Fact]
    public void FuncDelegate_ExecutesProperly()
    {
        // Arrange
        Func<string?, char, int> func = CharacterCounter.CreateFuncDelegate();
        const string text = "123-456-789-000";

        // Act
        int dashes = func(text, '-');

        // Assert
        Assert.Equal(3, dashes);
    }

    [Fact]
    public void ExecuteCount_ThrowsArgumentNullException_WhenDelegateIsNull()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            CharacterCounter.ExecuteCount("test", 't', null!));
    }

    [Fact]
    public void ExecuteCount_CallsPassedDelegateSuccessfully()
    {
        // Arrange
        CharacterCountDelegate myDel = (src, ch) => src != null && src.StartsWith(ch) ? 100 : 0;

        // Act
        int result = CharacterCounter.ExecuteCount("apple", 'a', myDel);

        // Assert
        Assert.Equal(100, result);
    }

    [Theory]
    [InlineData("Обробка подій та делегати", 'о', false)]
    [InlineData("Обробка подій та делегати", 'о', true)]
    [InlineData("MulticastDelegate and Events in .NET 8", 'e', true)]
    [InlineData("AAAAA aaaaa", 'a', true)]
    [InlineData("AAAAA aaaaa", 'a', false)]
    [InlineData("", 'x', true)]
    public void CompareAllApproaches_ConfirmsAllMechanismsMatch(string source, char targetChar, bool ignoreCase)
    {
        // Act
        StringAnalysisResult comparison = CharacterCounter.CompareAllApproaches(source, targetChar, ignoreCase);

        // Assert
        Assert.True(comparison.AllResultsMatch, "Всі 4 механізми виклику делегатів мають повертати однаковий результат!");
        Assert.Equal(comparison.NamedMethodCount, comparison.AnonymousMethodCount);
        Assert.Equal(comparison.AnonymousMethodCount, comparison.LambdaExpressionCount);
        Assert.Equal(comparison.LambdaExpressionCount, comparison.FuncDelegateCount);
    }

    [Fact]
    public void GenericSearchDelegate_DemonstratesCustomGenericDelegate()
    {
        // Arrange
        GenericSearchDelegate<string, char, int> genericSearch = (str, target) =>
            CharacterCounter.CountOccurrencesNamed(str, target);

        // Act
        int count = genericSearch("generic delegate test", 'e');

        // Assert
        Assert.Equal(6, count);
    }
}
