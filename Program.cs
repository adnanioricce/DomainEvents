using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddTransient<IOrderService,OrderService>();
var app = builder.Build();
// Registre o manipulador(handler) do evento no publicador
DomainEvents.RegisterAsync(new NotificarParceirosEventHandler(app.Services.GetRequiredService<ILogger<NotificarParceirosEventHandler>>()));
DomainEvents.RegisterAsync(new AtualizarEstoqueEventHandler(app.Services.GetRequiredService<ILogger<AtualizarEstoqueEventHandler>>()));
DomainEvents.RegisterAsync(new EnviarSmsParaClienteEventHandler(app.Services.GetRequiredService<ILogger<EnviarSmsParaClienteEventHandler>>()));
DomainEvents.RegisterAsync(new GerarNotaFiscalEventHandler(app.Services.GetRequiredService<ILogger<GerarNotaFiscalEventHandler>>()));
// DomainEvents.RegisterAsync(new ProdutoCriadoEventHandler(app.Services.GetRequiredService<ILogger<ProdutoCriadoEventHandler>>()));
// com DI
// DomainEventsWithDI.Initialize(app.Services);
// DomainEventsWithDI.RegisterAsync<PedidoRealizadoEvent,NotificarParceirosEventHandler>();
// DomainEventsWithDI.RegisterAsync<PedidoRealizadoEvent,AtualizarEstoqueEventHandler>();
// DomainEventsWithDI.RegisterAsync<PedidoRealizadoEvent,EnviarSmsParaClienteEventHandler>();
// DomainEventsWithDI.RegisterAsync<PedidoRealizadoEvent,GerarNotaFiscalEventHandler>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapPost("/orders",async ([FromServices]IOrderService orderService) => {
    var product = RNG.GenerateProduct();
    var quantity = RNG.RandomQuantity();
    var domainEvents = orderService.PlaceOrderAsync(product,quantity);
    await foreach(var domainEvent in domainEvents){
        await DomainEvents.RaiseAsync(domainEvent);
    }
    return Results.Ok(new {
        domainEvents
    });
});
app.MapPost("/simpleorders",async ([FromServices]IOrderService orderService) => {
    var product = RNG.GenerateProduct();
    var quantity = RNG.RandomQuantity();
    var order = await orderService.SimplePlaceOrderAsync(product,quantity);
    return Results.Ok(order);
});
app.Run();