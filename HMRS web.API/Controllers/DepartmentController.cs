using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HMRS_web.API.Controllers
{
    [Route("api/[controller]")] // Base URL: api/Department
    [ApiController]
    [Authorize] // Requires JWT token
    public class DepartmentController : ControllerBase
    {
        private readonly HmrsContext _context;

        public DepartmentController(HmrsContext context)
        {
            _context = context; // Initialize database context
        }

        // GET: api/Department
        // Returns all departments with employee counts
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Departments
                .Select(d => new DeReadDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EmployeeCount = d.Employees.Count // Number of employees
                })
                .ToListAsync();

            return Ok(departments); // Returns 200 OK with department list
        }

        // GET: api/Department/123e4567-e89b-12d3-a456-426614174000
        // Returns one department by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(Guid id)
        {
            var department = await _context.Departments
                .Where(d => d.Id == id)
                .Select(d => new DeReadDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EmployeeCount = d.Employees.Count
                })
                .FirstOrDefaultAsync();

            if (department == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            return Ok(department); // Returns 200 OK with department
        }

        // POST: api/Department
        // Creates a new department
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admins can create
        public async Task<IActionResult> CreateDepartment([FromBody] DeCreateDTO dto)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(), // Generate unique ID
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var readDto = new DeReadDTO
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                EmployeeCount = 0 // New department has no employees
            };

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, readDto); // Returns 201 Created
        }

        // PUT: api/Department/123e4567-e89b-12d3-a456-426614174000
        // Updates a department
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can update
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DeUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Department ID mismatch");
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            if (!string.IsNullOrEmpty(dto.Name)) department.Name = dto.Name;
            if (dto.Description != null) department.Description = dto.Description;

            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content
        }

        // DELETE: api/Department/123e4567-e89b-12d3-a456-426614174000
        // Deletes a department
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            if (department.Employees.Any())
            {
                return BadRequest("Cannot delete department with employees");
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content
        }

        // GET: api/Department/search?name=IT
        // Searches departments by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchDepartments([FromQuery] string? name)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(name));
            }

            var departments = await query
                .Select(d => new DeReadDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EmployeeCount = d.Employees.Count
                })
                .ToListAsync();

            return Ok(departments); // Returns 200 OK with search results
        }
    }
}