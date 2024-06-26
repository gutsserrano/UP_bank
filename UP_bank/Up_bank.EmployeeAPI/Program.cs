using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.AddressApiServices;
using UP_bank.EmployeeAPI.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UP_bankEmployeeAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UP_bankEmployeeAPIContext") ?? throw new InvalidOperationException("Connection string 'UP_bankEmployeeAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddSingleton<IAddressApiService, UP_bank.AddressApi>();

builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<UPBankAddressApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
