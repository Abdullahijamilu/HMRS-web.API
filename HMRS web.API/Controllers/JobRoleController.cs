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
    [Route("api/[controller]")] // Base URL: api/JobRole
    [ApiController]
    [Authorize] // Requires JWT token
    public class JobRoleController : ControllerBase
    {
        private readonly HmrsContext _context;

        public JobRoleController(HmrsContext context)
        {
            _context = context; // Initialize database context
        }

        // GET: api/JobRole
        // Returns all job roles
        [HttpGet]
        public async Task<IActionResult> GetJobRoles()
        {
            var jobRoles = await _context.JobRoles
                .Select(j => new JobRoleReadDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    EmployeeCount = j.Employees.Count // Number of employees
                })
                .ToListAsync();

            return Ok(jobRoles); // Returns 200 OK with job role list
        }

        // GET: api/JobRole/123e4567-e89b-12d3-a456-426614174000
        // Returns one job role by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobRole(Guid id)
        {
            var jobRole = await _context.JobRoles
                .Where(j => j.Id == id)
                .Select(j => new JobRoleReadDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    EmployeeCount = j.Employees.Count
                })
                .FirstOrDefaultAsync();

            if (jobRole == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            return Ok(jobRole); // Returns 200 OK with job role
        }

        // POST: api/JobRole
        // Creates a new job role
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admins can create
        public async Task<IActionResult> CreateJobRole([FromBody] JobRoleCreateDTO dto)
        {
            var jobRole = new JobRole
            {
                Id = Guid.NewGuid(), // Generate unique ID
                Title = dto.Title,
                Description = dto.Description
            };

            _context.JobRoles.Add(jobRole);
            await _context.SaveChangesAsync();

            var readDto = new JobRoleReadDTO
            {
                Id = jobRole.Id,
                Title = jobRole.Title,
                Description = jobRole.Description,
                EmployeeCount = 0 // New job role has no employees
            };

            return CreatedAtAction(nameof(GetJobRole), new { id = jobRole.Id }, readDto); // Returns 201 Created
        }

        // PUT: api/JobRole/123e4567-e89b-12d3-a456-426614174000
        // Updates a job role
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can update
        public async Task<IActionResult> UpdateJobRole(Guid id, [FromBody] JobRoleUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Job Role ID mismatch");
            }

            var jobRole = await _context.JobRoles.FindAsync(id);
            if (jobRole == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            if (!string.IsNullOrEmpty(dto.Title)) jobRole.Title = dto.Title;
            if (dto.Description != null) jobRole.Description = dto.Description;

            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content
        }

        // DELETE: api/JobRole/123e4567-e89b-12d3-a456-426614174000
        // Deletes a job role
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete
        public async Task<IActionResult> DeleteJobRole(Guid id)
        {
            var jobRole = await _context.JobRoles.FindAsync(id);
            if (jobRole == null)
            {
                return NotFound(); // Returns 404 if not found
            }

            if (jobRole.Employees.Any())
            {
                return BadRequest("Cannot delete job role with employees");
            }

            _context.JobRoles.Remove(jobRole);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content
        }

        // GET: api/JobRole/search?title=Developer
        // Searches job roles by title
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobRoles([FromQuery] string? title)
        {
            var query = _context.JobRoles.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                title = title.ToLower();
                query = query.Where(jr => jr.Title.ToLower().Contains(title));
            }

            var jobRoles = await query
                .Select(jr => new JobRoleReadDTO
                {
                    Id = jr.Id,
                    Title = jr.Title,
                    Description = jr.Description,
                    EmployeeCount = jr.Employees.Count
                })
                .ToListAsync();

            return Ok(jobRoles); // Returns 200 OK with search results
        }
    }
}