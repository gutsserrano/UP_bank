using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Services.AddressApiServices;
using Services.AgencyServices;
using UPBank.AgencyAPI.Data;

namespace UPBank.AgencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenciesController : ControllerBase
    {
        private readonly UPBankAgencyAPIContext _context;
        private IAddressApiService _addressService;
        private readonly AgencyService _agencyService;

        public AgenciesController(UPBankAgencyAPIContext context, AgencyService agencyService, IAddressApiService addressService)
        {
            _context = context;
            _agencyService = agencyService;
            _addressService = addressService;
        }

        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgency()
        {
          if (_context.Agency == null)
          {
              return NotFound();
          }
            return await _context.Agency.ToListAsync();
        }

        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agency>> GetAgency(string id)
        {
          if (_context.Agency == null)
          {
              return NotFound();
          }
            var agency = await _context.Agency.FindAsync(id);

            if (agency == null)
            {
                return NotFound();
            }

            return agency;
        }

        // PUT: api/Agencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgency(string id, Agency agency)
        {
            if (id != agency.Number)
            {
                return BadRequest();
            }

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Agencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(AgencyDTO agencyDTO)
        {
            if (_context.Agency == null)
            {
                return Problem("Entity set 'UPBankAgencyAPIContext.Agency'  is null.");
            }

            List <Employee> employees = _agencyService.GetEmployees(agencyDTO.EmployeesCpf).Result;

            // Populando o agencyEmployees apenas enquanto o método GetEmployees não é implementado
            List<AgencyEmployee> agencyEmployees = agencyDTO.EmployeesCpf;
            foreach (var item in employees)
            {
                agencyEmployees.Add(new AgencyEmployee
                {
                    Cpf = item.Cpf
                });
            }

            if (!employees.Any(e => e.Manager))
            {
                return BadRequest("The agency must contains at least one Manager");
            }

            var agency = new Agency
            {
                Number = agencyDTO.Number,
                Cnpj = agencyDTO.Cnpj,
                Restriction = agencyDTO.Restriction,
                Employees = employees,
                EmployeesCpf = agencyEmployees,
                AddressZipCode = agencyDTO.AddressDTO.ZipCode,
                AddressNumber = agencyDTO.AddressDTO.Number
            };

            Address? address = _addressService.GetAddress(new AddressDTO()
            {
                ZipCode = agencyDTO.AddressDTO.ZipCode,
                Number = agencyDTO.AddressDTO.Number,
                Complement = agencyDTO.AddressDTO.Complement
            }).Result;

            if (address == null)
            {
                address = _addressService.CreateAddress(new AddressDTO()
                {
                    ZipCode = agencyDTO.AddressDTO.ZipCode,
                    Number = agencyDTO.AddressDTO.Number,
                    Complement = agencyDTO.AddressDTO.Complement
                }).Result;
            }

            if (address == null)
            {
                return Problem("Address not found or could not be created.");
            }

            agency.Address = address;

            _context.Agency.Add(agency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AgencyExists(agency.Number))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAgency", new { id = agency.Number }, agency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency(string id)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agency.Remove(agency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AgencyExists(string id)
        {
            return (_context.Agency?.Any(e => e.Number == id)).GetValueOrDefault();
        }
    }
}
