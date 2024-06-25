﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using NuGet.ContentModel;
using Services;
using UP_bank.EmployeeAPI.Data;

namespace Up_bank.EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly UP_bankEmployeeAPIContext _context;
        private readonly EmployeeService _employeeService;

        public EmployeesController(UP_bankEmployeeAPIContext context, EmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }

            List<Employee> employees = await _context.Employee.ToListAsync();

            foreach (Employee employee in employees)
            {
                Address address = await _employeeService.GetAddress(employee.AddressZipCode, employee.AddressNumber);
                employee.Address = address;
            }

            return Ok(employees);

        }

        // GET: api/Employees/5
        [HttpGet("{cpf}")]
        public async Task<ActionResult<Object>> GetEmployee(string cpf)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            else if (cpf.Count() == 14 ) { return BadRequest("Insert the CPF without any formatting in the URL."); }
            else { return BadRequest("The CPF is wrong!"); }


            if (_context.Employee == null)
            {
                return NotFound();
            }

            
            Employee employee = await _context.Employee.Where(p => p.Cpf == cpf).FirstOrDefaultAsync();
            
            if (EmployeeExists(cpf) == true)
            {
                if (employee == null)
                {
                    return NotFound();
                }

                Address address = await _employeeService.GetAddress(employee.AddressZipCode, employee.AddressNumber);
                employee.Address = address;
                return employee;
            }
            
            else { return NotFound(); }
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{cpf}")]
        public async Task<ActionResult<Employee>> PutEmployee(string cpf, EmployeeUpdateDTO employeeUpdateDTO)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            else if (cpf.Count() == 14) { return BadRequest("Insert the CPF without any formatting in the URL."); }
            else { return BadRequest("The CPF is wrong!"); }


            if (cpf != employeeUpdateDTO.Cpf) { return BadRequest("This CPF doesn't corresponds to this employee"); }

            Employee employee = await _context.Employee.Where(p => p.Cpf == cpf).FirstOrDefaultAsync();

            if (employee == null) { return NotFound("This employee doesn't exists!"); }

            _context.Entry(_employeeService.UpdateEmployee(employee, employeeUpdateDTO)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(cpf))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return employee;

        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(EmployeeDTO employeeDTO)
        {
            Employee employee = null;

            if (employeeDTO.Cpf.Count() == 11) { employeeDTO.Cpf = InsertMask(employeeDTO.Cpf); }
            else if (employeeDTO.Cpf.Count() != 14 && employeeDTO.Cpf.Count() != 11) { return BadRequest("The CPF is wrong!"); }

            var cpfIstRegistered = await _context.FindAsync<Employee>(employeeDTO.Cpf);
            if (cpfIstRegistered != null) { return BadRequest("This CPF already exists!"); }

            try
            {
                Address address = await _employeeService.GetAddressPostMethod(employeeDTO.AddressDTO);

                if(address == null) { return BadRequest("This Zip Code cannot be found!"); }

                employee = await _employeeService.CreateEmployee(employeeDTO, address);

                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }

            return Ok(employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{cpf}")]
        public async Task<IActionResult> DeleteEmployee(string cpf)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            else if (cpf.Count() == 14) { return BadRequest("Insert the CPF without any formatting in the URL."); }
            else { return BadRequest("The CPF is wrong!"); }

            if (_context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.Where(p => p.Cpf == cpf).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            var deletedEmployee = new DeletedEmployee(employee);

            _context.DeletedEmployee.Add(deletedEmployee);

            _context.Employee.Remove(employee);

            await _context.SaveChangesAsync();

            return Ok(deletedEmployee);
        }

        private bool EmployeeExists(string cpf)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            return (_context.Employee?.Any(e => e.Cpf == cpf)).GetValueOrDefault();
        }

        private bool DeletedEmployeeExists(string cpf)
        {
            if (cpf.Count() == 11) { cpf = InsertMask(cpf); }
            return (_context.Employee?.Any(de => de.Cpf == cpf)).GetValueOrDefault();
        }

        public static string RemoveMask(string cpf)
        {
            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");
            return cpf;
        }

        public static string InsertMask(string cpf)
        {
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }
    }
}