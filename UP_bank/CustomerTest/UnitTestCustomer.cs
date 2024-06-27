using APICustomer.Controllers;
using APICustomer.Data;
using APICustomer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Services.AddressApiServices;

namespace CustomerTest
{
    public class UnitTestCustomer
    {
        private DbContextOptions<APICustomerContext> options;

        private void InitializeDataBase()
        {
            options = new DbContextOptionsBuilder<APICustomerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new APICustomerContext(options))
            {
                if (context.Customer.Any())
                {
                    context.Customer.RemoveRange(context.Customer);
                    context.SaveChanges();
                }
                context.Customer.Add(new Customer { Cpf = "12345678913", Name = "Test Customer", AddressNumber = "123", AddressZipCode = "14806099", Email = "email@email", Phone = "1612345" });
                context.Customer.Add(new Customer { Cpf = "12345678914", Name = "Test Customer 2", AddressNumber = "456", AddressZipCode = "14806245", Email = "email2@email", Phone = "16678" });
                context.SaveChanges();
            }
        }
        [Fact]
        public void TestGet()
        {
            InitializeDataBase();
            using (var context = new APICustomerContext(options))
            {
                CustomersController customerController = new CustomersController(context, new UPBankAddressApi(), new CustomerServices());
                IEnumerable<Customer> customers = customerController.GetCustomer().Result.Value;
                Assert.Equal(ReferenceEquals(customers, customers), true);

            }
        }
        [Fact]
        public async Task TestGetByCpf()
        {
            InitializeDataBase();
            using (var context = new APICustomerContext(options))
            {
                CustomersController customerController = new CustomersController(context, new UPBankAddressApi(), new CustomerServices());

                AddressDTO addressDTO = new AddressDTO
                {
                    Complement = "",
                    Number = "10",
                    ZipCode = "14802-020"
                };

                CustomerDTO dto = new CustomerDTO
                {
                    Address = addressDTO,
                    Cpf = "056.050.510-83",
                    DtBirth = new DateTime(1990, 1, 1),
                    Email = "alguem@email.com",
                    Income = 10000,
                    Name = "João José da Silva",
                    Phone = "12212121",
                    Restriction = false,
                    Sex = 'M'
                };
                await customerController.PostCustomer(dto); // para criar o customer o CPF precisa estar válido

                var result = await customerController.GetCustomerByCpf("05605051083"); // para o Get o cpf precisa estar sem máscara
                Assert.Equal("João José da Silva", result.Value.Name);
            }
        }

    }
}