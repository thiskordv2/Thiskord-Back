using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Init du paquet SignalR (Broadcast)
builder.Services.AddSignalR();

builder.Services.AddScoped<Thiskord_Back.Services.IDbConnectionService, Thiskord_Back.Services.DBService>();
builder.Services.AddScoped<Thiskord_Back.Services.AuthService>();
builder.Services.AddScoped<Thiskord_Back.Services.JsonService>();
builder.Services.AddScoped<Thiskord_Back.Services.LogService>();
builder.Services.AddScoped<Thiskord_Back.Services.ProjectService>();
builder.Services.AddScoped<Thiskord_Back.Services.ChannelService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
