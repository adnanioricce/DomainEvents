using MediatR;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddTransient<IOrderService,OrderService>();
builder.Services.AddMediatR(config => {
   config.RegisterServicesFromAssembly(typeof(Order).Assembly);   
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/orders",async ([FromServices]IOrderService orderService,[FromServices]IMediator mediator) => {
    var product = RNG.GenerateProduct();
    var quantity = RNG.RandomQuantity();
    var @events = orderService.PlaceOrderAsync(product,quantity);
    await foreach(var @event in @events){
        await mediator.Publish(@event);
    }
    return Results.Ok(@events);
});
app.MapPost("/simpleorders",async ([FromServices]IOrderService orderService) => {
    var product = RNG.GenerateProduct();
    var quantity = RNG.RandomQuantity();
    var order = await orderService.SimplePlaceOrderAsync(product,quantity);
    return Results.Ok(order);
});
app.Run();