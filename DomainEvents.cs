using MediatR;

public abstract class DomainEvent : INotification
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
}
public interface IDomainHandlerAsync<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}
public interface IDomainHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : DomainEvent
{
}