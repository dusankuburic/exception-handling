using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Tegla.Application.Services.Items;
using Tegla.Application.Services.Items.Mappings;
using Tegla.Persistence;
using Tegla.Persistence.Brokers.Loggings;
using Tegla.Persistence.Brokers.Storages;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build();

var logger = new LoggerConfiguration()
    // Read from appsettings.json
    .ReadFrom.Configuration(configuration)
    // Create the actual logger
    .CreateLogger();

Log.Logger = logger;

builder.Services.AddSingleton(Log.Logger);
// Add services to the container.
builder.Services.AddScoped<ILoggingBroker, LoggingBroker>();
builder.Services.AddScoped<IStorageBroker, StorageBroker>();
builder.Services.AddScoped<IItemService, ItemService>();

var config = new MapperConfiguration(c => {
    c.AddProfile<ItemsMappingProfile>();
});
builder.Services.AddSingleton(s => config.CreateMapper());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
