using Hall_Of_Fame.Services;
using Microsoft.EntityFrameworkCore;
using Hall_Of_Fame.Entities;
using System;
using Hall_Of_Fame.Interface;
using Hall_Of_Fame.Repositories;
using Serilog;
using Serilog.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IPersonService, PersonService>();
builder.Services.AddTransient<IPersonRepository, PersonRepository>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt") 
    .CreateLogger();

var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());

builder.Services.AddSingleton(loggerFactory);
builder.Services.AddLogging();


var conString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234;")
);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
