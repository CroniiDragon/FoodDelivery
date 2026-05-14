using FoodDelivery.NotificationService.Adapter;
using FoodDelivery.NotificationService.Data;
using FoodDelivery.NotificationService.Factories;
using FoodDelivery.NotificationService.Interfaces;
using FoodDelivery.NotificationService.Repositories;
using FoodDelivery.NotificationService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDb")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService,    NotificationService>();

// Channels — Adapter replaces the default EmailChannel
builder.Services.AddScoped<INotificationChannel>(sp =>
    new EmailServiceAdapter(new ExternalEmailService()));
builder.Services.AddScoped<INotificationChannel, SmsChannel>();
builder.Services.AddScoped<INotificationChannel, PushChannel>();

// Factory Method — one factory per channel
builder.Services.AddScoped<INotificationMessageFactory, EmailMessageFactory>();
builder.Services.AddScoped<INotificationMessageFactory, SmsMessageFactory>();
builder.Services.AddScoped<INotificationMessageFactory, PushMessageFactory>();

// Chain of Responsibility is built inside NotificationService constructor

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "NotificationService", Version = "v1" }));
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
