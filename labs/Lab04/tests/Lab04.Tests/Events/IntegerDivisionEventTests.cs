using System;
using Lab04.Core.Math;
using Xunit;

namespace Lab04.Tests.Events;

public class IntegerDivisionEventTests
{
    private class SubclassedCalculator : MathCalculator
    {
        public bool CustomHookCalled { get; private set; }

        public SubclassedCalculator(string name) : base(name) { }

        protected override void OnIntegerDivision(IntegerDivisionEventArgs e)
        {
            CustomHookCalled = true;
            e.Note = "Modified by subclass hook";
            base.OnIntegerDivision(e);
        }
    }

    [Fact]
    public void IntegerDivide_FiresIntegerDivisionEvent_WithCorrectArgs()
    {
        // Arrange
        var calculator = new MathCalculator("EventTestCalc");
        bool eventFired = false;
        object? capturedSender = null;
        IntegerDivisionEventArgs? capturedArgs = null;

        calculator.IntegerDivision += (sender, args) =>
        {
            eventFired = true;
            capturedSender = sender;
            capturedArgs = args;
        };

        // Act
        long quotient = calculator.IntegerDivide(25, 4, note: "Unit testing");

        // Assert
        Assert.Equal(6, quotient);
        Assert.True(eventFired, "Подія IntegerDivision має бути згенерована!");
        Assert.Same(calculator, capturedSender);
        Assert.NotNull(capturedArgs);
        Assert.Equal(25, capturedArgs.Dividend);
        Assert.Equal(4, capturedArgs.Divisor);
        Assert.Equal(6, capturedArgs.Quotient);
        Assert.Equal(1, capturedArgs.Remainder);
        Assert.False(capturedArgs.IsExact);
        Assert.Equal("Unit testing", capturedArgs.Note);
        Assert.NotEqual(Guid.Empty, capturedArgs.OperationId);
        Assert.True(capturedArgs.TimestampUtc <= DateTime.UtcNow);
        Assert.Equal(1, calculator.IntegerDivisionsCount);
    }

    [Fact]
    public void IntegerDivideWithRemainder_ExactDivision_SetsIsExactToTrue()
    {
        // Arrange
        var calculator = new MathCalculator();
        bool isExactReported = false;

        calculator.IntegerDivision += (_, args) =>
        {
            isExactReported = args.IsExact;
        };

        // Act
        var (quotient, remainder) = calculator.IntegerDivideWithRemainder(40, 8);

        // Assert
        Assert.Equal(5, quotient);
        Assert.Equal(0, remainder);
        Assert.True(isExactReported);
    }

    [Fact]
    public void IntegerDivide_ThrowsDivideByZeroException_AndDoesNotFireEvent()
    {
        // Arrange
        var calculator = new MathCalculator();
        bool eventFired = false;

        calculator.IntegerDivision += (_, _) =>
        {
            eventFired = true;
        };

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => calculator.IntegerDivide(10, 0));
        Assert.False(eventFired, "При діленні на нуль подія не повинна генеруватися!");
        Assert.Equal(0, calculator.IntegerDivisionsCount);
    }

    [Fact]
    public void IntegerDivisionCustom_FiresCustomDelegateEvent()
    {
        // Arrange
        var calculator = new MathCalculator();
        bool customEventFired = false;
        IntegerDivisionEventArgs? receivedArgs = null;

        calculator.IntegerDivisionCustom += (sender, args) =>
        {
            customEventFired = true;
            receivedArgs = args;
        };

        // Act
        calculator.IntegerDivide(100, 7);

        // Assert
        Assert.True(customEventFired);
        Assert.NotNull(receivedArgs);
        Assert.Equal(14, receivedArgs.Quotient);
        Assert.Equal(2, receivedArgs.Remainder);
    }

    [Fact]
    public void SubclassedCalculator_CanOverrideOnIntegerDivision()
    {
        // Arrange
        var customCalc = new SubclassedCalculator("SubCalc");
        string noteReported = string.Empty;

        customCalc.IntegerDivision += (_, args) =>
        {
            noteReported = args.Note;
        };

        // Act
        customCalc.IntegerDivide(50, 5);

        // Assert
        Assert.True(customCalc.CustomHookCalled);
        Assert.Equal("Modified by subclass hook", noteReported);
    }

    [Fact]
    public void ToString_OnEventArgs_ProducesFormattedSummary()
    {
        var args = new IntegerDivisionEventArgs(10, 3, 3, 1, "test");
        string summary = args.ToString();

        Assert.Contains("10 / 3 = 3", summary);
        Assert.Contains("з остачею 1", summary);
    }
}
