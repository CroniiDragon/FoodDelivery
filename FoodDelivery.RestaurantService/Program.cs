using FoodDelivery.RestaurantService.Data;
using FoodDelivery.RestaurantService.Interfaces;
using FoodDelivery.RestaurantService.Proxy;
using FoodDelivery.RestaurantService.Repositories;
using FoodDelivery.RestaurantService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RestaurantDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDb")));

builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IMenuItemRepository,   MenuItemRepository>();
builder.Services.AddScoped<IRestaurantService,    RestaurantService>();

// Proxy — wraps MenuItemService, checks restaurant.IsOpen before returning menu
builder.Services.AddScoped<MenuItemService>();
builder.Services.AddScoped<IMenuItemService>(sp =>
    new MenuAccessProxy(
        sp.GetRequiredService<MenuItemService>(),
        sp.GetRequiredService<IRestaurantRepository>()
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "RestaurantService", Version = "v1" }));
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
