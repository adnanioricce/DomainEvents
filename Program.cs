using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
var app = builder.Build();
// Registre o manipulador(handler) do evento no publicador
DomainEvents.RegisterAsync(OrderEventHandlers.NotificarParceirosAsync(app.Services.GetRequiredService<ILogger>()));
DomainEvents.RegisterAsync(OrderEventHandlers.AtualizarEstoqueAsync(app.Services.GetRequiredService<ILogger>()));
DomainEvents.RegisterAsync(OrderEventHandlers.EnviarSmsParaClienteAsync(app.Services.GetRequiredService<ILogger>()));
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
    await foreach(var domainEvent in orderService.PlaceOrderAsync(product,quantity)){
        await DomainEvents.RaiseAsync(domainEvent);
    }
});
app.MapPost("/simpleorders",async ([FromServices]IOrderService orderService) => {
    var product = RNG.GenerateProduct();
    var quantity = RNG.RandomQuantity();
    await foreach(var domainEvent in orderService.PlaceOrderAsync(product,quantity)){
        await DomainEvents.RaiseAsync(domainEvent);
    }
});
app.Run();