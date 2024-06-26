using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.AddressApiServices;
using Services.AgencyServices;
using UPBank.AgencyAPI.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UPBankAgencyAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UPBankAgencyAPIContext") ?? throw new InvalidOperationException("Connection string 'UPBankAgencyAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<AgencyService>();
builder.Services.AddSingleton<IAddressApiService, UPBankAddressApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
