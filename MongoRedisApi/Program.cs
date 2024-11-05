using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoRedisApi.Models;
using MongoRedisApi.Services;
using StackExchange.Redis;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

//MongoDB 
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<UserService>();

//Redis
var redisConnectionString = builder.Configuration.GetSection("RedisSettings:ConnectionString").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<RedisCacheService>();
//Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
