using APICustomer.Controllers;
using APICustomer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

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
                if(context.Customer.Any())
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
                CustomersController customerController = new CustomersController(context);
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
                CustomersController customerController = new CustomersController(context);
                ActionResult<Customer> customer = await customerController.GetCustomerByCpf("12345678913");
                Assert.Equal("Test Customer", customer.Value.Name);

            }
        }

    }
}