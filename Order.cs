
using MediatR;

public record Order
{    
    public int Id { get; private set; } = RNG.RandomQuantity();
    public string Product { get; private set; }
    public int Quantity { get; private set; }

    public Order(string product, int quantity)
    {        
        Product = product;
        Quantity = quantity;
    }

    public IEnumerable<DomainEvent> PlaceOrder()
    {
        // Lógica de pedido
        // ...

        // Adiciona um evento de domínio
        yield return new OrderPlacedEvent(Id);
    }
}
public sealed class OrderPlacedEvent : DomainEvent
{
    public int OrderId { get; }
    
    public OrderPlacedEvent(int orderId)
    {
        OrderId = orderId;
    }
}
public sealed class OrderPlacedEventHandler : IDomainHandler<OrderPlacedEvent>
{
    private readonly ILogger<OrderPlacedEventHandler> _logger;
    public OrderPlacedEventHandler(ILogger<OrderPlacedEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken = default)
    {
        var order = await OrderRepository.GetAsync(notification.OrderId);
        _logger.LogInformation("OrderPlacedEvent fired at {dateCreated}",notification.CreatedAt);
        _logger.LogInformation("Order with Id {orderId} has been placed.",notification.OrderId);
        _logger.LogInformation("Order placed = {order}",order);
    }
}
public sealed class NotificarParceirosEventHandler : IDomainHandler<OrderPlacedEvent>
{
    private readonly ILogger<NotificarParceirosEventHandler> _logger;
    public NotificarParceirosEventHandler(ILogger<NotificarParceirosEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var order = await OrderRepository.GetAsync(notification.OrderId);
        _logger.LogInformation("Notificando parceiros: pedido com Id = {orderId} foi registrado as {createdAt}.",notification.OrderId,notification.CreatedAt);
        _logger.LogInformation("Dados do pedido Id = {orderId} -> {order}",notification.OrderId,order);
    }
}

public interface IOrderService
{
    IAsyncEnumerable<DomainEvent> PlaceOrderAsync(string product,int quantity);
    Task<Order> SimplePlaceOrderAsync(string product,int quantity);
}
public sealed class OrderService : IOrderService
{
    private readonly IMediator _mediator;
    public OrderService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async IAsyncEnumerable<DomainEvent> PlaceOrderAsync(string product,int quantity){        
        var order = new Order(product,quantity);
        
        //Faça alguma coisa com o pedido;
        await OrderRepository.Insert(order);
        yield return new OrderPlacedEvent(order.Id);
    }
    public async Task<Order> SimplePlaceOrderAsync(string product,int quantity){        
        var order = new Order(product,quantity);        
        //Faça alguma coisa com o pedido;
        await OrderRepository.Insert(order);
        await _mediator.Publish(new OrderPlacedEvent(order.Id));
        return order;
    }    
}
