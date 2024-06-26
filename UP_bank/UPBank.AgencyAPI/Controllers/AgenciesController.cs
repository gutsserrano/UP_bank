using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using NuGet.Versioning;
using Services.AddressApiServices;
using Services.AgencyServices;
using UPBank.AgencyAPI.Data;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

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

            List<Agency> agencies = await _context.Agency.ToListAsync();

            foreach (var item in agencies)
            {
                Address? address = _addressService.GetAddress(new AddressDTO()
                {
                    ZipCode = item.AddressZipCode,
                    Number = item.AddressNumber
                }).Result;

                if (address == null)
                {
                    return NotFound("Address not found.");
                }

                item.Address = address;

                item.EmployeesCpf = _context.AgencyEmployee.Where(ae => ae.AgencyNumber == item.Number).ToList();

                item.Employees = _agencyService.GetEmployees(item.EmployeesCpf).Result;
            }

            return agencies;
        }

        // GET: api/Agencies/5
        [HttpGet("{number}")]
        public async Task<ActionResult<Agency>> GetAgency(string number, bool deleted = false)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }

            var agency = await _context.Agency.FindAsync(number);

            Address? address = _addressService.GetAddress(new AddressDTO()
            {
                ZipCode = agency.AddressZipCode,
                Number = agency.AddressNumber
            }).Result;

            if (address == null)
            {
                return NotFound("Address not found.");
            }

            agency.Address = address;

            agency.EmployeesCpf = _context.AgencyEmployee.Where(ae => ae.AgencyNumber == number).ToList();

            agency.Employees = _agencyService.GetEmployees(agency.EmployeesCpf).Result;

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

            List<AgencyEmployee> agencyEmployees = new();
            foreach(var item in agencyDTO.EmployeesCpf)
            {
                agencyEmployees.Add(new AgencyEmployee()
                {
                    Cpf = item
                });
            }

            List <Employee> employees = _agencyService.GetEmployees(agencyEmployees).Result;

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
        [HttpDelete("{number}")]
        public async Task<IActionResult> DeleteAgency(string number)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.FindAsync(number);
            if (agency == null)
            {
                return NotFound();
            }

            agency.EmployeesCpf = _context.AgencyEmployee.Where(ae => ae.AgencyNumber == number).ToList();

            _context.DeletedAgency.Add(new DeletedAgency(agency));

            foreach(var item in agency.EmployeesCpf)
            {
                _context.AgencyEmployee.Remove(item);
            }

            _context.Agency.Remove(agency);

            await _context.SaveChangesAsync();

            return Ok("Agency deleted.");
        }

        [HttpPost("restore/{number}")]
        public async Task<ActionResult<DeletedAgency>> RestoreAgency(string number)
        {
            if (_context.DeletedAgency == null)
            {
                return NotFound();
            }

            var deletedAgency = await _context.DeletedAgency.FindAsync(number);

            if (deletedAgency == null)
            {
                return NotFound();
            }

            List<DeletedAgencyEmployee> deletedAgencyEmployees = new();

            deletedAgency.EmployeesCpf = _context.DeletedAgencyEmployee.Where(ae => ae.DeletedAgencyNumber == number).ToList();

            _context.Agency.Add(new Agency(deletedAgency));

            foreach (var item in deletedAgency.EmployeesCpf)
            {
                _context.DeletedAgencyEmployee.Remove(item);
            }

            _context.DeletedAgency.Remove(deletedAgency);

            await _context.SaveChangesAsync();

            return Ok("Agency restored.");
        }

        private bool AgencyExists(string number)
        {
            return (_context.Agency?.Any(e => e.Number == number)).GetValueOrDefault();
        }
    }
}
