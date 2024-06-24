using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UPBank.AddressAPI.Data;
using UPBank.AddressAPI.PostalServices.Abstract;
using UPBank.AddressAPI.PostalServices;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UPBankAddressAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UPBankAddressAPIContext") ?? throw new InvalidOperationException("Connection string 'UPBankAddressAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddSingleton<IPostalAddressService, ViaCepService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
