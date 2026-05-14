using FoodDelivery.OrderService.Data;
using FoodDelivery.OrderService.Facade;
using FoodDelivery.OrderService.Interfaces;
using FoodDelivery.OrderService.Observer;
using FoodDelivery.OrderService.Repositories;
using FoodDelivery.OrderService.Services;
using FoodDelivery.OrderService.Strategy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService,    OrderService>();

// Facade + HttpClient for NotificationService
builder.Services.AddScoped<OrderFacade>();
builder.Services.AddHttpClient("NotificationService", c =>
    c.BaseAddress = new Uri("https://localhost:7129"));

// Strategy — default Standard, swappable via POST /orders/strategy/{name}
builder.Services.AddSingleton(_ => new DeliveryContext(new StandardDeliveryStrategy()));

// Observer — 3 subscribers registered at startup
builder.Services.AddSingleton(sp =>
{
    var subject = new OrderStatusSubject();
    subject.Subscribe(new CustomerNotificationObserver());
    subject.Subscribe(new CourierNotificationObserver());
    subject.Subscribe(new OrderAuditObserver());
    return subject;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "OrderService", Version = "v1" }));
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
