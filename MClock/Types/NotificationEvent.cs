using System;

namespace MClock.Types;

public sealed class NotificationEvent
{
    public Guid Id { get; set; }
    public NotificationEventType NotificationEventType { get; set; }
    public TimeOnly OccurredAt { get; set; }
    public bool CanNotifyAgain { get; set; }

    public NotificationEvent(NotificationEventType notificationEventType, TimeOnly occurredAt, bool canNotifyAgain)
    {
        Id = Guid.NewGuid();
        NotificationEventType = notificationEventType;
        OccurredAt = occurredAt;
        CanNotifyAgain = canNotifyAgain;
    }
}