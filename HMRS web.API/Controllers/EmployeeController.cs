using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using HMRS_web.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMRS_web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires JWT token
    public class EmployeeController : ControllerBase
    {
        private readonly AuthenticateServices _services;
        private readonly HmrsContext _context;

        public EmployeeController(AuthenticateServices services, HmrsContext context)
        {
            _services = services; // Initialize authentication service
            _context = context; // Initialize database context
        }

        // GET: api/Employee
        // Returns all employees
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new EReadDTO
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Phone = e.Phone,
                    Address = e.Address,
                    HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.Name : "Unknown",
                    JobRoleId = e.JobRoleId,
                    JobRoleTitle = e.JobRole != null ? e.JobRole.Title : "Unknown",
                    UserName = e.User != null ? e.User.UserName : null,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == e.UserId)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(employees); // Returns 200 OK with employee list
        }

        // GET: api/Employee/123e4567-e89b-12d3-a456-426614174000
        // Returns one employee by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            var employee = await _context.Employees
                .Where(e => e.Id == id)
                .Select(e => new EReadDTO
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Phone = e.Phone,
                    Address = e.Address,
                    HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.Name : "Unknown",
                    JobRoleId = e.JobRoleId,
                    JobRoleTitle = e.JobRole != null ? e.JobRole.Title : "Unknown",
                    UserName = e.User != null ? e.User.UserName : null,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == e.UserId)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            return Ok(employee); // Returns 200 OK with employee
        }

        // POST: api/Employee
        // Creates a new employee
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")] // Only Admins or Managers can create
        public async Task<IActionResult> CreateEmployee([FromBody] ECreateDTO dto)
        {
            // Validate DepartmentId and JobRoleId
            if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId))
            {
                return BadRequest("Invalid DepartmentId");
            }
            if (!await _context.JobRoles.AnyAsync(jr => jr.Id == dto.JobRoleId))
            {
                return BadRequest("Invalid JobRoleId");
            }

            string? userId = null;
            string? role = null;
            if (!string.IsNullOrEmpty(dto.UserName) && !string.IsNullOrEmpty(dto.Email) && !string.IsNullOrEmpty(dto.Password))
            {
                var registerDto = new RegisterDTO
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Password = dto.Password,
                    Role = dto.Role ?? "Employee" // Default to Employee role
                };

                var result = await _services.RegisterUser(registerDto);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors); // Returns 400 with errors
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
                userId = user?.Id;
                role = registerDto.Role;
            }

            var employee = new Employee
            {
                Id = Guid.NewGuid(), // Generate unique ID
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                DepartmentId = dto.DepartmentId,
                JobRoleId = dto.JobRoleId,
                HireDate = dto.HireDate,
                UserId = userId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var readDto = new EReadDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Phone = employee.Phone,
                Address = employee.Address,
                HireDate = employee.HireDate.HasValue ? new DateTime(employee.HireDate.Value.Year, employee.HireDate.Value.Month, employee.HireDate.Value.Day) : DateTime.MinValue,
                DepartmentId = employee.DepartmentId,
                DepartmentName = (await _context.Departments.FindAsync(employee.DepartmentId))?.Name ?? "Unknown",
                JobRoleId = employee.JobRoleId,
                JobRoleTitle = (await _context.JobRoles.FindAsync(employee.JobRoleId))?.Title ?? "Unknown",
                UserName = employee.User?.UserName,
                Role = role
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, readDto); // Returns 201 Created
        }

        // PUT: api/Employee/123e4567-e89b-12d3-a456-426614174000
        // Updates an employee
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Only Admins or Managers can update
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Employee ID mismatch");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            if (!string.IsNullOrEmpty(dto.FullName)) employee.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Phone)) employee.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Address)) employee.Address = dto.Address;
            if (dto.DepartmentId.HasValue)
            {
                if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId.Value))
                {
                    return BadRequest("Invalid DepartmentId");
                }
                employee.DepartmentId = dto.DepartmentId.Value;
            }
            if (dto.JobRoleId.HasValue)
            {
                if (!await _context.JobRoles.AnyAsync(jr => jr.Id == dto.JobRoleId.Value))
                {
                    return BadRequest("Invalid JobRoleId");
                }
                employee.JobRoleId = dto.JobRoleId.Value;
            }
            if (dto.HireDate.HasValue) employee.HireDate = dto.HireDate;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content
        }

        // DELETE: api/Employee/123e4567-e89b-12d3-a456-426614174000
        // Deletes an employee
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content
        }

        // GET: api/Employee/search?name=Sarah
        // Searches employees by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees([FromQuery] string? name)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                query = query.Where(e => e.FullName.ToLower().Contains(name));
            }

            var employees = await query
                .Select(e => new EReadDTO
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Phone = e.Phone,
                    Address = e.Address,
                    HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.Name : "Unknown",
                    JobRoleId = e.JobRoleId,
                    JobRoleTitle = e.JobRole != null ? e.JobRole.Title : "Unknown",
                    UserName = e.User != null ? e.User.UserName : null,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == e.UserId)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(employees); // Returns 200 OK with search results
        }
    }
}