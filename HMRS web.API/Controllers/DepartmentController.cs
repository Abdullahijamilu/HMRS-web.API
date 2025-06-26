using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMRS_web.API.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly HmrsContext dept;
        public DepartmentController(HmrsContext dept)
        {
            this.dept = dept;
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await this.dept.Departments
                .Select(d => new DeReadDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    //EmployeeCount = d.Employees.Count // Count employees in department
                })
                .ToListAsync();

            return Ok(departments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(Guid id)
        {
            var department = await this.dept.Departments
                .Where(d => d.Id == id)
                .Select(d => new DeReadDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    //EmployeeCount = d.Employees.Count
                })
                .FirstOrDefaultAsync();

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        // POST: api/Department
        // Creates a new department
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CreateDepartment([FromBody] DeCreateDTO dto)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(), 
                Name = dto.Name,
                Description = dto.Description
            };

            this.dept.Departments.Add(department);
            await this.dept.SaveChangesAsync();

            var readDto = new DeReadDTO
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                //EmployeeCount = 0 // New department has no employees
            };

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, readDto);
        }

        // PUT: api/Department
        // Updates a department
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DeUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Department ID mismatch");
            }

            var department = await this.dept.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(dto.Name)) department.Name = dto.Name;
            if (dto.Description != null) department.Description = dto.Description;

            await this.dept.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Department/123e4567-e89b-12d3-a456-426614174000
        // Deletes a department
        [HttpDelete("{id}")]// Only Admins can delete
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var department = await this.dept.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            if (department.Employees.Any())
            {
                return BadRequest("Cannot delete department with employees");
            }

            this.dept.Departments.Remove(department);
            await this.dept.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Department/search?name=IT
        // Searches departments by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchDepartments([FromQuery] string? name)
        {
            var query = this.dept.Departments.AsQueryable();

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
                    //EmployeeCount = d.Employees.Count
                })
                .ToListAsync();

            return Ok(departments);
        }
    }
}
    

