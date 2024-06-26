using AccountAPI.Services;
using AccountAPI.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<MongoDataSettings>(builder.Configuration.GetSection(nameof(MongoDataSettings)));
builder.Services.AddSingleton<IMongoDataSettings>(sp => sp.GetRequiredService<IOptions<MongoDataSettings>>().Value);


builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<CreditCardService>();
builder.Services.AddSingleton<TransactionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
