


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
public class NotificarParceirosEventHandler(ILogger<NotificarParceirosEventHandler> logger) : IEventHandlerAsync<PedidoRealizadoEvent>
{
    public async Task Handle(PedidoRealizadoEvent @event, CancellationToken token = default)
    {
        logger.LogInformation("Notificado parceiros de que novo pedido de Id = {orderId} foi realizado.",@event.OrderId);
    }
}
public class AtualizarEstoqueEventHandler(ILogger<AtualizarEstoqueEventHandler> _logger) : IEventHandlerAsync<PedidoRealizadoEvent>
{
    public async Task Handle(PedidoRealizadoEvent @event, CancellationToken token = default)
    {
        _logger.LogInformation("Atualizando estoque resultante do pedido Id = {orderId} realizado às {createdAt}.", @event.OrderId, @event.CreatedAt);
        await Task.CompletedTask;
    }
}

public class EnviarSmsParaClienteEventHandler(ILogger<EnviarSmsParaClienteEventHandler> _logger) : IEventHandlerAsync<PedidoRealizadoEvent>
{
    public async Task Handle(PedidoRealizadoEvent @event, CancellationToken token = default)
    {
        _logger.LogInformation("Notificando clientes que realizaram o pedido Id = {orderId} realizado às {createdAt}.", @event.OrderId, @event.CreatedAt);
        await Task.CompletedTask;
    }
}

public class GerarNotaFiscalEventHandler(ILogger<GerarNotaFiscalEventHandler> _logger) : IEventHandlerAsync<PedidoRealizadoEvent>
{
    public async Task Handle(PedidoRealizadoEvent @event, CancellationToken token = default)
    {
        _logger.LogInformation("Gerando nota fiscal do pedido Id = {orderId} realizado às {createdAt}.", @event.OrderId, @event.CreatedAt);
        await Task.CompletedTask;
    }
}
public class ProdutoCriadoEventHandler(ILogger<ProdutoCriadoEventHandler> _logger) : IEventHandlerAsync<ProdutoCriado>
{
    public Task Handle(ProdutoCriado @event, CancellationToken token = default)
    {
        throw new NotImplementedException();
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
