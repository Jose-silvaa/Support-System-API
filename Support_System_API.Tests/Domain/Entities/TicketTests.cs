using Support_System_API.Domain.Entities;
using Support_System_API.Domain.Enums;

namespace Support_System_API.Tests.Domain.Entities;

public class TicketTests
{
    private Ticket _ticket;

    [SetUp]
    public void Setup()
    {
        _ticket = new Ticket { Status = TicketStatus.Open };
    }

    [Test]
    public void UpdateStatus_SameStatus_ReturnsFailure()
    {
        var (result, activity) = _ticket.UpdateStatus(TicketStatus.Open, "Admin");

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Status is already the same"));
            Assert.That(activity, Is.Null);
            Assert.That(_ticket.Status, Is.EqualTo(TicketStatus.Open));
        });
    }

    [Test]
    public void UpdateStatus_NonAdminRole_ReturnsFailure()
    {
        var (result, activity) = _ticket.UpdateStatus(TicketStatus.InProgress, "User");

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("You must be a system administrator to perform this action"));
            Assert.That(activity, Is.Null);
            Assert.That(_ticket.Status, Is.EqualTo(TicketStatus.Open));
        });
    }

    [Test]
    public void UpdateStatus_AdminValidTransition_ReturnsSuccess()
    {
        var (result, activity) = _ticket.UpdateStatus(TicketStatus.InProgress, "Admin");

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(activity, Is.EqualTo("Status changed from Open to InProgress"));
            Assert.That(_ticket.Status, Is.EqualTo(TicketStatus.InProgress));
        });
    }

    [Test]
    public void UpdateStatus_AdminValidTransition_ClosedFromInProgress()
    {
        _ticket.Status = TicketStatus.InProgress;

        var (result, activity) = _ticket.UpdateStatus(TicketStatus.Closed, "Admin");

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(activity, Is.EqualTo("Status changed from InProgress to Closed"));
            Assert.That(_ticket.Status, Is.EqualTo(TicketStatus.Closed));
        });
    }
}
