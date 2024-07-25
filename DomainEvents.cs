public abstract class DomainEvent
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
}
public delegate Task HandleAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : DomainEvent;
public delegate void Handle<TDomainEvent>(TDomainEvent @event) where TDomainEvent : DomainEvent;
public static class DomainEvents
{
    private static List<Delegate> _subscribers = new List<Delegate>();

    public static void Register<T>(Handle<T> callback) where T : DomainEvent
    {
        _subscribers.Add(callback);
    }
    public static void RegisterAsync<T>(HandleAsync<T> callback) where T : DomainEvent
    {
        _subscribers.Add(callback);
    }
    public static void Raise<T>(T domainEvent) where T : DomainEvent
    {
        foreach (var subscriber in _subscribers.OfType<Handle<T>>())
        {
            subscriber(domainEvent);
        }
    }
    public static async Task RaiseAsync<T>(T domainEvent) where T : DomainEvent
    {
        foreach (var subscriber in _subscribers.OfType<HandleAsync<T>>())
        {
            await subscriber(domainEvent);
        }
    }
}
