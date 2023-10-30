using Serilog;
using Serilog.Sinks.File;
using Serilog.Formatting.Compact;
using Hall_Of_Fame.Interface;
using Hall_Of_Fame.Repositories;
using Hall_Of_Fame.Services;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using System.Net;
using Hall_Of_Fame.Controllers.V1;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IPersonService, PersonService>();
builder.Services.AddTransient<IPersonRepository, PersonRepository>();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        new RenderedCompactJsonFormatter(),
        $"log-{DateTime.Now:yyyy-MM}.txt",
        LogEventLevel.Information,
        rollingInterval: RollingInterval.Month)
    .CreateLogger();

var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());

builder.Services.AddSingleton(loggerFactory);
builder.Services.AddLogging();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//удалена строчка подключения из файла Program.cs, теперь продключение в appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable("__EFMigrationsHistory", "Hall_Of_Fame"))
);


//добавлена автомиграция 
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
    catch (Npgsql.PostgresException ex)
    {

        if (ex.SqlState != "42P07")
        {


        }
        
    }
}

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
