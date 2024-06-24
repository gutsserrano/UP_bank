using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using APICustomer.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<APICustomerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("APICustomerContext") ?? throw new InvalidOperationException("Connection string 'APICustomerContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
