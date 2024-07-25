

public record Order(int Id,string Product,int Quantity)
{
}
public sealed class PedidoRealizadoEvent(int orderId) : DomainEvent
{
    public int OrderId => orderId;
}
public sealed class ProdutoCriado(int orderId) : DomainEvent
{
    public int OrderId => orderId;
}
public static partial class OrderEventHandlers 
{
    public static HandleAsync<PedidoRealizadoEvent> NotificarParceirosAsync(ILogger logger)
    {
        return async (PedidoRealizadoEvent @event) => {
            logger.LogInformation("Notificado parceiros de que novo pedido de Id = {orderId} foi realizado.",@event.OrderId);
        };
    }

    public static HandleAsync<PedidoRealizadoEvent> AtualizarEstoqueAsync(ILogger logger)
    {
        return async (PedidoRealizadoEvent @event) => {
            logger.LogInformation("Atualizando estoque resultando do pedido Id = {orderId} realizado as {createdAt}.",@event.OrderId,@event.CreatedAt);
        };
    }

    public static HandleAsync<PedidoRealizadoEvent> EnviarSmsParaClienteAsync(ILogger logger)
    {
        return async (PedidoRealizadoEvent @event) => {            
            logger.LogInformation("Notificando clientes que realizaram o pedido Id = {orderId} realizado as {createdAt}.",@event.OrderId,@event.CreatedAt);
        };
    }
    public static HandleAsync<PedidoRealizadoEvent> GerarNotaFiscalAsync(ILogger logger)
    {
        return async (PedidoRealizadoEvent @event) => {            
            logger.LogInformation("Gerando nota fiscal do pedido Id = {orderId} realizado as {createdAt}.",@event.OrderId,@event.CreatedAt);
        };
    }
}
public interface IOrderService
{
    Task<Order> SimplePlaceOrderAsync(string product, int quantity);
    IAsyncEnumerable<DomainEvent> PlaceOrderAsync(string product, int quantity);
}
public class OrderService : IOrderService
{
    public async IAsyncEnumerable<DomainEvent> PlaceOrderAsync(string product, int quantity)
    {
        var order = new Order(RNG.RandomQuantity(),product,quantity);
        // Faça alguma coisa com o pedido
        // await OrderRepository.Insert(order);
        yield return new PedidoRealizadoEvent(order.Id);
        yield return new ProdutoCriado(order.Id);
    }

    public async Task<Order> SimplePlaceOrderAsync(string product, int quantity)
    {
        var order = new Order(RNG.RandomQuantity(),product,quantity);
        // Faça alguma coisa com o pedido
        // await OrderRepository.Insert(order);
        await DomainEvents.RaiseAsync(new PedidoRealizadoEvent(order.Id));
        await DomainEvents.RaiseAsync(new ProdutoCriado(order.Id));
        return order;
    }
}
