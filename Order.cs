public class Order
{    
    public int Id { get; private set; }
    public string Product { get; private set; }
    public int Quantity { get; private set; }

    public Order(int id, string product, int quantity)
    {
        Id = id;
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
    public void Handle(OrderPlacedEvent domainEvent)
    {
        Console.WriteLine($"Order with Id {domainEvent.OrderId} has been placed.");
    }
}
public static partial class OrderEventHandlers 
{
    public static HandleAsync<OrderPlacedEvent> NotificarParceirosAsync = async (OrderPlacedEvent @event) => 
    {
        Console.WriteLine("Notificado parceiros de que novo pedido foi realizado.");           
    };
}
