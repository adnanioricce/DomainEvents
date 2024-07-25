public abstract class DomainEvent
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
public interface IEventHandler<TDomainEvent> where TDomainEvent : DomainEvent
{
    void Handle(TDomainEvent @event);
}
public interface IEventHandlerAsync<TDomainEvent> where TDomainEvent : DomainEvent
{
    Task Handle(TDomainEvent @event,CancellationToken token = default);
}
public static class DomainEvents
{
    private static readonly Dictionary<Type,List<object>> _subscribers = [];    
    private static readonly Dictionary<Type,List<object>> _subscribersAsync = [];
    public static void Register<T>(IEventHandler<T> callback) where T : DomainEvent
    {
        var handlers = _subscribers.GetValueOrDefault(typeof(T)) ?? [];
        
        if(handlers.Contains(callback))
            return;

        handlers.Add(callback);

        _subscribers[typeof(T)] = handlers;
    }
    public static void RegisterAsync<T>(IEventHandlerAsync<T> callback) where T : DomainEvent
    {
        var handlers = _subscribersAsync.GetValueOrDefault(typeof(T)) ?? [];
        
        if(handlers.Contains(callback))
            return;

        handlers.Add(callback);

        _subscribersAsync[typeof(T)] = handlers;
    }
    public static void Raise<T>(T domainEvent) where T : DomainEvent
    {
        var @eventType = domainEvent.GetType();
        var subscribers = _subscribers.GetValueOrDefault(@eventType);
        foreach (var subscriber in subscribers)
        {
            var handleMethodInfo = subscriber.GetType().GetMethod("Handle");
            var ct = CancellationToken.None;
            handleMethodInfo.Invoke(subscriber,new object[] {domainEvent,ct});
        }
    }
    public static async Task RaiseAsync<T>(T domainEvent) where T : DomainEvent
    {        
        var @eventType = domainEvent.GetType();
        var subscribers = _subscribersAsync.GetValueOrDefault(@eventType) ?? [];
        foreach (var subscriber in subscribers)
        {
            var handleMethodInfo = subscriber.GetType().GetMethod("Handle");
            var ct = CancellationToken.None;
            await (Task)handleMethodInfo.Invoke(subscriber,new object[] {domainEvent,ct});
        }
    }
}
public static class DomainEventsWithDI
{
    private static IServiceProvider _sp;
    public static void Initialize(IServiceProvider sp){
        _sp = sp;
    }
    private static object InstanceSubscriber(Type subscriberType)
    {
        var constructors = subscriberType.GetConstructors();
        var firstConstructor = constructors.FirstOrDefault();
        var parameters = firstConstructor.GetParameters()
            .Select(parameter => _sp.GetRequiredService(parameter.ParameterType))
            .ToArray();
        var subscriber = firstConstructor.Invoke(parameters);
        return subscriber;
        
    }

    private static readonly Dictionary<Type,List<Type>> _subscribers = [];
    private static readonly Dictionary<Type,List<Type>> _subscribersAsync = [];
    // public static void Register<IEventHandler<TDomainEvent>>() where TDomainEvent : DomainEvent
    // {
    //     _subscribers.Add(typeof(IEventHandler<TDomainEvent>));
    // }
    public static void Register<TDomainEvent,THandler>()
        where THandler : IEventHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {        
        if(!_subscribers.ContainsKey(typeof(TDomainEvent)))
            _subscribers.Add(typeof(TDomainEvent),[]);
            
        var handlers = _subscribers[typeof(TDomainEvent)];
        if(handlers.Contains(typeof(THandler)))
            return;
        handlers.Add(typeof(THandler));
    }
    public static void RegisterAsync<TDomainEvent,THandler>() 
        where THandler : IEventHandlerAsync<TDomainEvent>
        where TDomainEvent : DomainEvent
    {
        if(!_subscribersAsync.ContainsKey(typeof(TDomainEvent)))
            _subscribersAsync.Add(typeof(TDomainEvent),[]);
        
        var handlers = _subscribersAsync[typeof(TDomainEvent)];

        if(handlers.Contains(typeof(THandler)))
            return;
        handlers.Add(typeof(THandler));
    }
    public static void Raise<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : DomainEvent
    {
        if(!_subscribers.TryGetValue(domainEvent.GetType(),out var subscribersTypes))
            return;
        foreach (var subscriberType in subscribersTypes)
        {
            var subscriber = InstanceSubscriber(subscriberType);
            ((IEventHandler<TDomainEvent>)subscriber).Handle(domainEvent);
        }
    }

    
    public static async Task RaiseAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : DomainEvent
    {
        if(!_subscribersAsync.TryGetValue(domainEvent.GetType(),out var subscribersTypes))
            return;
        foreach (var subscriberType in subscribersTypes)
        {
            var subscriberBoxed = InstanceSubscriber(subscriberType);
            var subscriber = (IEventHandlerAsync<TDomainEvent>)subscriberBoxed;
            await subscriber.Handle(domainEvent);
        }
    }
}