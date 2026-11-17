using System;
using System.Collections.Generic;
using Lab04.Core.Delegates;
using Lab04.Core.Handlers;
using Lab04.Core.Math;
using Xunit;

namespace Lab04.Tests.Handlers;

public class AdvancedMulticastTests
{
    [Fact]
    public void MulticastDelegate_WithCharacterCountDelegate_CombinesInvocations()
    {
        // Arrange
        int invocationCount1 = 0;
        int invocationCount2 = 0;

        CharacterCountDelegate del1 = (src, ch) =>
        {
            invocationCount1++;
            return CharacterCounter.CountOccurrencesNamed(src, ch);
        };

        CharacterCountDelegate del2 = (src, ch) =>
        {
            invocationCount2++;
            return CharacterCounter.CountOccurrencesNamed(src, ch);
        };

        CharacterCountDelegate combined = del1 + del2;

        // Act
        int result = combined("Multicast test", 't');

        // Assert
        Assert.Equal(1, invocationCount1);
        Assert.Equal(1, invocationCount2);
        // Multicast returns the result of the LAST delegate in the invocation list
        Assert.Equal(4, result);

        // GetInvocationList inspection
        Delegate[] list = combined.GetInvocationList();
        Assert.Equal(2, list.Length);
    }

    [Fact]
    public void NegativeNumberDivision_ProducesCorrectQuotientAndRemainder()
    {
        var calculator = new MathCalculator();
        IntegerDivisionEventArgs? args = null;

        calculator.IntegerDivision += (_, e) => args = e;

        // Act 1: -25 / 4 = -6, remainder -1
        long q1 = calculator.IntegerDivide(-25, 4);
        Assert.Equal(-6, q1);
        Assert.NotNull(args);
        Assert.Equal(-1, args.Remainder);

        // Act 2: -25 / -4 = 6, remainder -1
        long q2 = calculator.IntegerDivide(-25, -4);
        Assert.Equal(6, q2);
        Assert.Equal(-1, args.Remainder);
    }

    [Fact]
    public void OneAuditor_CanTrackMultipleCalculatorsConcurrently()
    {
        var calc1 = new MathCalculator("EngineeringCore");
        var calc2 = new MathCalculator("FinanceEngine");
        var auditor = new CalculationAuditLogger("CentralAuditHub");

        auditor.Subscribe(calc1);
        auditor.Subscribe(calc2);

        calc1.IntegerDivide(100, 20, "Batch 1");
        calc2.IntegerDivide(88, 11, "Invoice #42");
        calc1.IntegerDivide(33, 5, "Batch 2");

        Assert.Equal(3, auditor.TotalLoggedEntries);

        Assert.Equal("EngineeringCore", auditor.Logs[0].SenderName);
        Assert.Equal("FinanceEngine", auditor.Logs[1].SenderName);
        Assert.Equal("EngineeringCore", auditor.Logs[2].SenderName);
    }

    [Fact]
    public void SubscriberMethods_ThrowArgumentNullException_OnNullArguments()
    {
        var notificationHandler = new ConsoleNotificationHandler();
        var auditLogger = new CalculationAuditLogger();
        var statsTracker = new DivisionStatisticsTracker();

        Assert.Throws<ArgumentNullException>(() => notificationHandler.Subscribe(null!));
        Assert.Throws<ArgumentNullException>(() => notificationHandler.Unsubscribe(null!));
        Assert.Throws<ArgumentNullException>(() => notificationHandler.OnIntegerDivision(null, null!));

        Assert.Throws<ArgumentNullException>(() => auditLogger.Subscribe(null!));
        Assert.Throws<ArgumentNullException>(() => auditLogger.Unsubscribe(null!));
        Assert.Throws<ArgumentNullException>(() => auditLogger.OnIntegerDivision(null, null!));

        Assert.Throws<ArgumentNullException>(() => statsTracker.Subscribe(null!));
        Assert.Throws<ArgumentNullException>(() => statsTracker.Unsubscribe(null!));
        Assert.Throws<ArgumentNullException>(() => statsTracker.OnIntegerDivision(null, null!));
    }
}
