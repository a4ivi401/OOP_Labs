using System;
using Lab04.Core.Math;
using Xunit;

namespace Lab04.Tests.Math;

public class MathCalculatorTests
{
    private readonly MathCalculator _calculator = new("TestCalculator");

    [Fact]
    public void Constructor_SetsDefaultOrCustomName()
    {
        var defaultCalc = new MathCalculator();
        var namedCalc = new MathCalculator("EnterpriseEngine");

        Assert.Equal("DefaultCalculator", defaultCalc.Name);
        Assert.Equal("EnterpriseEngine", namedCalc.Name);
    }

    [Theory]
    [InlineData(10, 25, 35)]
    [InlineData(-5, 15, 10)]
    [InlineData(-10, -20, -30)]
    [InlineData(0, 42, 42)]
    public void Add_ReturnsCorrectSum_AndIncrementsOperationsCount(long a, long b, long expected)
    {
        long initialCount = _calculator.TotalOperationsCount;

        long result = _calculator.Add(a, b);

        Assert.Equal(expected, result);
        Assert.Equal(initialCount + 1, _calculator.TotalOperationsCount);
    }

    [Fact]
    public void Add_ThrowsOverflowException_OnCheckedOverflow()
    {
        Assert.Throws<OverflowException>(() => _calculator.Add(long.MaxValue, 1));
    }

    [Theory]
    [InlineData(100, 35, 65)]
    [InlineData(10, 20, -10)]
    [InlineData(-5, -5, 0)]
    public void Subtract_ReturnsCorrectDifference(long a, long b, long expected)
    {
        long result = _calculator.Subtract(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(6, 7, 42)]
    [InlineData(-5, 8, -40)]
    [InlineData(-4, -5, 20)]
    [InlineData(100, 0, 0)]
    public void Multiply_ReturnsCorrectProduct(long a, long b, long expected)
    {
        long result = _calculator.Multiply(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10.0, 2.0, 5.0)]
    [InlineData(7.0, 2.0, 3.5)]
    [InlineData(-15.0, 3.0, -5.0)]
    public void Divide_Double_ReturnsCorrectFloatingPointQuotient(double a, double b, double expected)
    {
        double result = _calculator.Divide(a, b);
        Assert.Equal(expected, result, precision: 5);
    }

    [Fact]
    public void Divide_Double_ThrowsDivideByZeroException_WhenDivisorIsZero()
    {
        Assert.Throws<DivideByZeroException>(() => _calculator.Divide(10.0, 0.0));
    }

    [Theory]
    [InlineData(10, 3, 1)]
    [InlineData(20, 5, 0)]
    [InlineData(7, 4, 3)]
    public void Modulo_ReturnsCorrectRemainder(long a, long b, long expected)
    {
        long result = _calculator.Modulo(a, b);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Modulo_ThrowsDivideByZeroException_WhenDivisorIsZero()
    {
        Assert.Throws<DivideByZeroException>(() => _calculator.Modulo(10, 0));
    }

    [Theory]
    [InlineData(2.0, 3.0, 8.0)]
    [InlineData(5.0, 0.0, 1.0)]
    [InlineData(4.0, 0.5, 2.0)]
    [InlineData(2.0, -2.0, 0.25)]
    public void Power_ReturnsCorrectValue(double baseValue, double exponent, double expected)
    {
        double result = _calculator.Power(baseValue, exponent);
        Assert.Equal(expected, result, precision: 5);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(5, 120)]
    [InlineData(10, 3628800)]
    [InlineData(20, 2432902008176640000)]
    public void Factorial_CalculatesCorrectly(int n, long expected)
    {
        long result = _calculator.Factorial(n);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Factorial_ThrowsArgumentOutOfRangeException_ForNegativeInput()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Factorial(-1));
    }

    [Fact]
    public void Factorial_ThrowsOverflowException_ForNumbersGreaterThan20()
    {
        Assert.Throws<OverflowException>(() => _calculator.Factorial(21));
    }

    [Theory]
    [InlineData(-42, 42)]
    [InlineData(42, 42)]
    [InlineData(0, 0)]
    public void Abs_ReturnsAbsoluteValue(long input, long expected)
    {
        long result = _calculator.Abs(input);
        Assert.Equal(expected, result);
    }
}
