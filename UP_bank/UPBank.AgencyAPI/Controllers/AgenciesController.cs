using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Newtonsoft.Json;
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
        private readonly HttpClient _httpClient = new HttpClient();

        public AgenciesController(UPBankAgencyAPIContext context, AgencyService agencyService, IAddressApiService addressService)
        {
            _context = context;
            _agencyService = agencyService;
            _addressService = addressService;
        }

        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgency(bool deleted = false)
        {
            if (!deleted)
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
            else
            {
                if (_context.DeletedAgency == null)
                {
                    return NotFound();
                }

                List<DeletedAgency> agencies = await _context.DeletedAgency.ToListAsync();

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

                    item.EmployeesCpf = _context.DeletedAgencyEmployee.Where(ae => ae.DeletedAgencyNumber == item.Number).ToList();

                    List<AgencyEmployee> agencyEmployees = new();
                    foreach (var e in item.EmployeesCpf)
                    {
                        agencyEmployees.Add(new AgencyEmployee(e));
                    }

                    item.Employees = _agencyService.GetEmployees(agencyEmployees).Result;
                }

                return agencies.Select(a => new Agency(a)).ToList();
            }
        }

        // GET: api/Agencies/5
        [HttpGet("{number}")]
        public async Task<ActionResult<Agency>> GetAgency(string number, bool deleted = false)
        {
            if (!deleted)
            {
                if (_context.Agency == null)
                {
                    return NotFound();
                }

                var agency = await _context.Agency.FindAsync(number);

                if(agency == null)
                {
                    return NotFound();
                }

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
            else
            {
                if (_context.DeletedAgency == null)
                {
                    return NotFound();
                }

                var agency = await _context.DeletedAgency.FindAsync(number);

                if (agency == null)
                {
                    return NotFound();
                }

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

                agency.EmployeesCpf = _context.DeletedAgencyEmployee.Where(ae => ae.DeletedAgencyNumber == number).ToList();

                List<AgencyEmployee> agencyEmployees = new();
                foreach (var item in agency.EmployeesCpf)
                {
                    agencyEmployees.Add(new AgencyEmployee(item));
                }

                agency.Employees = _agencyService.GetEmployees(agencyEmployees).Result;

                if (agency == null)
                {
                    return NotFound();
                }

                return new Agency(agency);
            }
        }

        [HttpGet("employee/{cpf}")]
        public async Task<ActionResult<Agency>> GetAgencyByEmployee(string cpf)
        {
            if (_context.AgencyEmployee == null)
            {
                return NotFound();
            }

            if (cpf.Length != 11)
            {
                return BadRequest("Invalid Cpf.");
            }

            cpf = _agencyService.InsertCpfMask(cpf);

            AgencyEmployee agencyEmployee = _context.AgencyEmployee.Find(cpf);

            if (agencyEmployee == null)
            {
                return NotFound();
            }

            return GetAgency(agencyEmployee.AgencyNumber).Result;
        }

        [HttpGet("agencyNumber/{number}/allAccounts")]
        public async Task<ActionResult<List<Account>>> GetAccounts(string number)
        {
            var response = _httpClient.GetAsync($"https://localhost:7244/api/accounts/type/0").Result;
            string jsonResponse = await response.Content.ReadAsStringAsync();
            List<Account> accountList = JsonConvert.DeserializeObject<List<Account>>(jsonResponse);

            return accountList.Where(a => a.Agency.Number == number).ToList();
        }

        [HttpGet("agencyNumber/{number}/restrictedAccounts")]
        public async Task<ActionResult<List<Account>>> GetRestrictedAccounts(string number)
        {
            var response = _httpClient.GetAsync($"https://localhost:7244/api/accounts/type/1").Result;
            string jsonResponse = await response.Content.ReadAsStringAsync();
            List<Account> accountList = JsonConvert.DeserializeObject<List<Account>>(jsonResponse);

            return accountList.Where(a => a.Agency.Number == number).ToList();
        }

        [HttpGet("agencyNumber/{number}/accountsByProfile/{profile}")]
        public async Task<ActionResult<List<Account>>> GetAccountsByPerfil(string number, int profile)
        {
            if (profile < 0 || profile > 2)
                return BadRequest("Invalid profile.");

            var response = _httpClient.GetAsync($"https://localhost:7244/api/accounts/profile/{profile}").Result;
            string jsonResponse = await response.Content.ReadAsStringAsync();
            List<Account> accountList = JsonConvert.DeserializeObject<List<Account>>(jsonResponse);

            return accountList.Where(a => a.Agency.Number == number).ToList();
        }

        [HttpGet("agencyNumber/{number}/loanAccounts")]
        public async Task<ActionResult<List<Account>>> GetloanAccounts(string number)
        {
            var response = _httpClient.GetAsync($"https://localhost:7244/api/accounts/type/2").Result;
            string jsonResponse = await response.Content.ReadAsStringAsync();
            List<Account> accountList = JsonConvert.DeserializeObject<List<Account>>(jsonResponse);

            return accountList.Where(a => a.Agency.Number == number).ToList();
        }

        // PUT: api/Agencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{number}")]
        public async Task<IActionResult> PutAgency(string number, AgencyUpdateDTO agencyUpdateDTO)
        {
            if (number != agencyUpdateDTO.Number)
            {
                return BadRequest();
            }

            if (_context.Agency == null)
            {
                return NotFound();
            }

            var agency = await _context.Agency.FindAsync(number);

            if(agency == null)
            {
                return NotFound();
            }

            agency.Restriction = agencyUpdateDTO.Restriction;

            /*https://localhost:7244/api/accounts/agency/0064*/

            AgencyRestrictionDTO restrictionDto = new AgencyRestrictionDTO { Restriction = agencyUpdateDTO.Restriction };

            StringContent content = new(JsonConvert.SerializeObject(restrictionDto), Encoding.UTF8, "application/json");

            var response = _httpClient.PatchAsync($"https://localhost:7244/api/accounts/agency/{number}", content).Result;

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(number))
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

        [HttpPut("addEmployee/{cpf}/agency/{agencyNumber}")]
        public async Task<ActionResult<Agency>> AddEmployee(string agencyNumber, string cpf)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }

            var agency = await _context.Agency.FindAsync(agencyNumber);

            if (agency == null)
            {
                return NotFound();
            }

            if (cpf.Length != 11)
            {
                return BadRequest("Invalid Cpf.");
            }

            cpf = _agencyService.InsertCpfMask(cpf);

            agency.EmployeesCpf = _context.AgencyEmployee.Where(ae => ae.AgencyNumber == agencyNumber).ToList();

            if (agency.EmployeesCpf.Any(e => e.Cpf == cpf))
            {
                return BadRequest("Employee already exists in this agency.");
            }

            agency.Employees = _agencyService.GetEmployees(agency.EmployeesCpf).Result;

            Employee employee = _agencyService.GetEmployee(cpf).Result;

            if (employee == null)
            {
                return NotFound();
            }

            agency.Employees.Add(employee);

            agency.EmployeesCpf.Add(new AgencyEmployee()
            {
                Cpf = cpf
            });

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(agencyNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return agency;
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

            if(_context.Agency.Find(agencyDTO.Number) != null || _context.DeletedAgency.Find(agencyDTO.Number) != null)
            {
                return BadRequest("Agency already exists.");
            }

            List<AgencyEmployee> agencyEmployees = new();
            foreach(var item in agencyDTO.EmployeesCpf)
            {
                AgencyEmployee ae = new AgencyEmployee()
                {
                    Cpf = item
                };

                if(_context.AgencyEmployee.Any(ae => ae.Cpf == item) || _context.DeletedAgencyEmployee.Any(ae => ae.Cpf == item))
                {
                    return BadRequest($"Employee {item} already exists in another agency.");
                }

                agencyEmployees.Add(ae);
            }

            List <Employee> employees = _agencyService.GetEmployees(agencyEmployees).Result;

            if(employees == null)
            {
                return NotFound("Employees not found.");
            }

            if (!employees.Any(e => e.Manager))
            {
                return BadRequest("The agency must contains at least one Manager");
            }

            string cnpjMask = _agencyService.RemoveCnpjMask(agencyDTO.Cnpj);

            if (!_agencyService.VerifyCnpj(cnpjMask))
            {
                return BadRequest("Invalid Cnpj.");
            }

            cnpjMask = _agencyService.InsertCnpjMask(cnpjMask);

            var agency = new Agency
            {
                Number = agencyDTO.Number,
                Cnpj = cnpjMask,
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

            HttpResponseMessage response = new();

            string restoreAccountsUri = $"https://localhost:7244/api/accounts/restore/agency/{number}";
            response = await _httpClient.PostAsync(restoreAccountsUri, null);

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

            /*https://localhost:7244/api/accounts/close-account/agency/0064*/

            HttpResponseMessage response = new();

            string deleteAccountsUri = $"https://localhost:7244/api/accounts/close-account/agency/{number}";
            response = await _httpClient.DeleteAsync(deleteAccountsUri);

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

        [HttpDelete("removeEmployee/{cpf}/agency/{agencyNumber}")]
        public async Task<ActionResult<Agency>> RemoveEmployee(string agencyNumber, string cpf)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }

            var agency = await _context.Agency.FindAsync(agencyNumber);

            if (agency == null)
            {
                return NotFound();
            }

            if (cpf.Length != 11)
            {
                return BadRequest("Invalid Cpf.");
            }

            cpf = _agencyService.InsertCpfMask(cpf);

            agency.EmployeesCpf = _context.AgencyEmployee.Where(ae => ae.AgencyNumber == agencyNumber).ToList();

            if (!agency.EmployeesCpf.Any(e => e.Cpf == cpf))
            {
                return BadRequest("Employee does not exist in this agency.");
            }

            agency.Employees = _agencyService.GetEmployees(agency.EmployeesCpf).Result;

            Employee employee = _agencyService.GetEmployee(cpf).Result;

            if (employee == null)
            {
                return NotFound();
            }

            if(employee.Manager && agency.Employees.Count(e => e.Manager) == 1)
            {
                return BadRequest("The agency must contains at least one Manager");
            }

            agency.Employees.Remove(employee);

            var employeeToRemove = agency.EmployeesCpf.FirstOrDefault(e => e.Cpf == cpf);

            agency.EmployeesCpf.Remove(employeeToRemove);

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(agencyNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetAgency(agency.Number, false);
        }

        private bool AgencyExists(string number)
        {
            return (_context.Agency?.Any(e => e.Number == number)).GetValueOrDefault();
        }
    }
}
