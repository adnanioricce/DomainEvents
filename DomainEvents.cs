public abstract class DomainEvent
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
}
public delegate Task HandleAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : DomainEvent;
public delegate void Handle<TDomainEvent>(TDomainEvent @event) where TDomainEvent : DomainEvent;
public interface IDomainHandlerAsync<TDomainEvent> where TDomainEvent : DomainEvent
{
    Task HandleAsync(TDomainEvent @event);
}
public interface IDomainHandler<TDomainEvent> where TDomainEvent : DomainEvent
{
    void Handle(TDomainEvent @event);
}
public static class DomainEvents
{
    private static List<Delegate> _subscribers = new List<Delegate>();

    public static void Register<T>(Action<T> callback) where T : DomainEvent
    {
        _subscribers.Add(callback);
    }

    public static void Raise<T>(T domainEvent) where T : DomainEvent
    {
        foreach (var subscriber in _subscribers.OfType<Action<T>>())
        {
            subscriber(domainEvent);
        }
    }
}
