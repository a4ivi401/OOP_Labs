using System;
using System.Collections.Generic;
using Lab04.Core.Handlers;
using Lab04.Core.Math;
using Xunit;

namespace Lab04.Tests.Handlers;

public class SubscribersTests
{
    [Fact]
    public void ConsoleNotificationHandler_ReceivesAndFormatsEventDetails()
    {
        // Arrange
        var calculator = new MathCalculator("AlphaCalc");
        var messages = new List<string>();
        var handler = new ConsoleNotificationHandler("UserNotifier", msg => messages.Add(msg));
        handler.Subscribe(calculator);

        // Act
        calculator.IntegerDivide(47, 5, "Batch processing");

        // Assert
        Assert.Equal(1, handler.NotificationsSentCount);
        Assert.Single(messages);

        string message = messages[0];
        Assert.Contains("AlphaCalc", message);
        Assert.Contains("47 / 5 = 9", message);
        Assert.Contains("остача = 2", message);
        Assert.Contains("Batch processing", message);
        Assert.Contains("UserNotifier", message);
    }

    [Fact]
    public void ConsoleNotificationHandler_Unsubscribe_StopsReceivingEvents()
    {
        // Arrange
        var calculator = new MathCalculator();
        var handler = new ConsoleNotificationHandler();
        handler.Subscribe(calculator);

        // Act 1
        calculator.IntegerDivide(10, 2);
        Assert.Equal(1, handler.NotificationsSentCount);

        // Act 2: Unsubscribe
        handler.Unsubscribe(calculator);
        calculator.IntegerDivide(20, 4);

        // Assert: Count remains 1
        Assert.Equal(1, handler.NotificationsSentCount);
    }

    [Fact]
    public void CalculationAuditLogger_RecordsAndFiltersEntries()
    {
        // Arrange
        var calculator = new MathCalculator("AuditCalc");
        var logger = new CalculationAuditLogger("SecurityAuditor");
        logger.Subscribe(calculator);

        // Act
        calculator.IntegerDivide(100, 10, "Exact 1");
        calculator.IntegerDivide(100, 30, "With Remainder 1");
        calculator.IntegerDivide(50, 25, "Exact 2");
        calculator.IntegerDivide(17, 4, "With Remainder 2");

        // Assert
        Assert.Equal(4, logger.TotalLoggedEntries);

        var exact = logger.GetExactDivisions();
        Assert.Equal(2, exact.Count);
        Assert.All(exact, e => Assert.True(e.IsExact));

        var withRemainder = logger.GetDivisionsWithRemainder();
        Assert.Equal(2, withRemainder.Count);
        Assert.All(withRemainder, e => Assert.False(e.IsExact));

        // FindById
        Guid sampleId = exact[0].OperationId;
        AuditLogEntry? found = logger.FindById(sampleId);
        Assert.NotNull(found);
        Assert.Equal("Exact 1", found.Note);
    }

    [Fact]
    public void CalculationAuditLogger_Clear_EmptiesTheLog()
    {
        var calculator = new MathCalculator();
        var logger = new CalculationAuditLogger();
        logger.Subscribe(calculator);

        calculator.IntegerDivide(10, 2);
        Assert.Equal(1, logger.TotalLoggedEntries);

        logger.Clear();
        Assert.Equal(0, logger.TotalLoggedEntries);
    }

    [Fact]
    public void DivisionStatisticsTracker_CalculatesAccurateMetrics()
    {
        // Arrange
        var calculator = new MathCalculator();
        var tracker = new DivisionStatisticsTracker("MetricsTracker");
        tracker.Subscribe(calculator);

        // Act: 3 operations:
        // 1) 20 / 4 = 5 (rem 0)
        // 2) 15 / 2 = 7 (rem 1)
        // 3) 10 / 5 = 2 (rem 0)
        calculator.IntegerDivide(20, 4);
        calculator.IntegerDivide(15, 2);
        calculator.IntegerDivide(10, 5);

        // Assert
        Assert.Equal(3, tracker.TotalDivisions);
        Assert.Equal(2, tracker.ExactDivisionsCount);
        Assert.Equal(1, tracker.NonExactDivisionsCount);
        Assert.Equal(14, tracker.SumOfQuotients); // 5 + 7 + 2 = 14
        Assert.Equal(14.0 / 3.0, tracker.AverageQuotient, precision: 4);
        Assert.Equal(200.0 / 3.0, tracker.ExactDivisionPercentage, precision: 2); // 2 out of 3 = 66.67%
        Assert.Equal(20, tracker.MaxDividend);
        Assert.Equal(10, tracker.MinDividend);

        string summary = tracker.ToString();
        Assert.Contains("Усього операцій ділення: 3", summary);
        Assert.Contains("Націло: 2", summary);

        // Reset
        tracker.Reset();
        Assert.Equal(0, tracker.TotalDivisions);
        Assert.Equal(0, tracker.ExactDivisionsCount);
        Assert.Equal(0.0, tracker.AverageQuotient);
        Assert.Null(tracker.MaxDividend);
    }

    [Fact]
    public void MulticastDelegate_DispatchesToAllSubscribers_AndHandlesUnsubscribe()
    {
        // Arrange
        var calculator = new MathCalculator("MulticastEngine");
        var notificationHandler = new ConsoleNotificationHandler();
        var auditLogger = new CalculationAuditLogger();
        var statsTracker = new DivisionStatisticsTracker();

        // Subscribe all 3 (multicast chaining: +=, +=, +=)
        notificationHandler.Subscribe(calculator);
        auditLogger.Subscribe(calculator);
        statsTracker.Subscribe(calculator);

        // Act 1: Run 2 divisions
        calculator.IntegerDivide(30, 6);
        calculator.IntegerDivide(22, 5);

        // Assert 1: All 3 received both events
        Assert.Equal(2, notificationHandler.NotificationsSentCount);
        Assert.Equal(2, auditLogger.TotalLoggedEntries);
        Assert.Equal(2, statsTracker.TotalDivisions);

        // Act 2: Unsubscribe audit logger only
        auditLogger.Unsubscribe(calculator);

        calculator.IntegerDivide(50, 10);

        // Assert 2: Audit logger remained at 2, while notification and stats advanced to 3
        Assert.Equal(3, notificationHandler.NotificationsSentCount);
        Assert.Equal(2, auditLogger.TotalLoggedEntries);
        Assert.Equal(3, statsTracker.TotalDivisions);
    }
}
