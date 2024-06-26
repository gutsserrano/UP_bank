using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICustomer.Data;
using Models;
using Models.DTO;
using Services.AddressApiServices;
using APICustomer.Services;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace APICustomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly APICustomerContext _context;

        private readonly IAddressApiService _addressApiService;
        private readonly CustomerServices _customerServices;
        private readonly CustomerServices _verifyCpf;

        public CustomersController(APICustomerContext context,IAddressApiService addressApiService, CustomerServices customerServices)
        {
            _context = context;
            _addressApiService = addressApiService;
            _customerServices = customerServices;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
          if (_context.Customer == null)
          {
              return NotFound();
          }
            var customers = await _context.Customer.ToListAsync();

            foreach (var customer in customers)
            {
                Address? address = await _addressApiService.GetAddress(new AddressDTO()
                {
                    ZipCode = customer.AddressZipCode,
                    Number = customer.AddressNumber,
                });
                if (address == null)
                {
                    return Problem("Address not found.");
                }
                customer.Address = address;
            }
            return customers;
        }

        // GET: api/Customers/5
        [HttpGet("{cpf}")]
        public async Task<ActionResult<Customer>> GetCustomerByCpf(string cpf)
        {
          if (_context.Customer == null)
          {
              return NotFound();
          }


            var customer = await _context.Customer.FindAsync(cpf);

            if (customer == null)
            {
                return NotFound();
            }
            Address? address = await _addressApiService.GetAddress(new AddressDTO()
            {
                ZipCode = customer.AddressZipCode,
                Number = customer.AddressNumber
            });
            if (address == null)
            {
                return Problem("Address not found.");
            }
            customer.Address = address;

            if (customer == null)
            {
                return NotFound();
            }


            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{cpf}")]
        public async Task<ActionResult<Customer>> PutCustomer(string cpf, CustomerUpdateDTO customerUpdateDTO)
        {
            if (cpf == null)
                return BadRequest("CPF not found!");

            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            else if (cpf.Count() == 14) { return BadRequest("Insert the CPF without any formatting in the URL."); }
            else { return BadRequest("The CPF is wrong!"); }

            if (cpf != customerUpdateDTO.Cpf) { return BadRequest("This CPF doesn't corresponds to this customer"); }

            Customer customer = await _context.Customer.Where(p => p.Cpf == cpf).FirstOrDefaultAsync();

            if (customer == null)
             return NotFound("This customer doesn't exists!");

            _context.Entry(_customerServices.UpdateCustomer(customer, customerUpdateDTO)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(cpf))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return customer;
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
          if (_context.Customer == null)
          {
              return Problem("Entity set 'APICustomerContext.Customer' is null.");
          }

            string cpfMask = _customerServices.RemoveMask(customerDTO.Cpf);

            if (!_customerServices.VerifyCpf(cpfMask))
            {
                return BadRequest("Invalid CPF.");
            }
            cpfMask = _customerServices.InsertMask(cpfMask);


            var customer = new Customer
            {
                Cpf = cpfMask,
                Name = customerDTO.Name,
                DtBirth = customerDTO.DtBirth,
                Email = customerDTO.Email,
                Income = customerDTO.Income,
                Phone = customerDTO.Phone,
                Sex = customerDTO.Sex,
                Restriction = customerDTO.Restriction,
                AddressNumber = customerDTO.Address.Number,
                AddressZipCode = customerDTO.Address.ZipCode
            };

           Address? address = await _addressApiService.GetAddress(new AddressDTO()
           {
               ZipCode = customerDTO.Address.ZipCode,
               Number = customerDTO.Address.Number,
               Complement = customerDTO.Address.Complement
           });
            if (address == null)
            {
                address = await _addressApiService.CreateAddress(new AddressDTO()
                {
                    ZipCode = customerDTO.Address.ZipCode,
                    Number = customerDTO.Address.Number,
                    Complement = customerDTO.Address.Complement
                });
            }
            if (address == null)
            {
                return Problem("Address not found or could not be created.");
            }
            customer.Address = address;


            _context.Customer.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.Cpf))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomerByCpf", new { cpf = customer.Cpf }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{cpf}")]
        public async Task<IActionResult> CustomerDelete(string cpf)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            else if (cpf.Count() == 14) { return BadRequest("Insert the CPF without any formatting in the URL."); }
            else { return BadRequest("The CPF is wrong!"); }

            if (_context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(cpf);
            if (customer == null)
            {
                return NotFound();
            }

            var customerDelete = new CustomerDelete(customer);
            _context.CustomerDelete.Add(customerDelete);

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);

        }

        private bool CustomerExists(string cpf)
        {
            if(cpf.Count() == 11) { cpf = InsertMask(cpf); }
            return (_context.Customer?.Any(e => e.Cpf == cpf)).GetValueOrDefault();
        }
        private bool CustomerDeleteExists(string cpf)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            return (_context.Customer?.Any(de => de.Cpf == cpf)).GetValueOrDefault();
        }
        public static string InsertMask(string cpf)
        {
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }
    }
}