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
    [Route("api/[controller]")] 
    [ApiController]
    [Authorize] 
    public class JobRoleController : ControllerBase
    {
        private readonly HmrsContext _context;

        public JobRoleController(HmrsContext context)
        {
            _context = context; 
        }

        
        [HttpGet]
        public async Task<IActionResult> GetJobRoles()
        {
            var jobRoles = await _context.JobRoles
                .Select(j => new JobRoleReadDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    EmployeeCount = j.Employees.Count 
                })
                .ToListAsync();

            return Ok(jobRoles); 
        }

        
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
                return NotFound(); 
            }

            return Ok(jobRole); 
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CreateJobRole([FromBody] JobRoleCreateDTO dto)
        {
            var jobRole = new JobRole
            {
                Id = Guid.NewGuid(), 
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
                EmployeeCount = 0 
            };

            return CreatedAtAction(nameof(GetJobRole), new { id = jobRole.Id }, readDto); 
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateJobRole(Guid id, [FromBody] JobRoleUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Job Role ID mismatch");
            }

            var jobRole = await _context.JobRoles.FindAsync(id);
            if (jobRole == null)
            {
                return NotFound(); 
            }

            if (!string.IsNullOrEmpty(dto.Title)) jobRole.Title = dto.Title;
            if (dto.Description != null) jobRole.Description = dto.Description;

            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteJobRole(Guid id)
        {
            var jobRole = await _context.JobRoles.FindAsync(id);
            if (jobRole == null)
            {
                return NotFound(); 
            }

            if (jobRole.Employees.Any())
            {
                return BadRequest("Cannot delete job role with employees");
            }

            _context.JobRoles.Remove(jobRole);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

       
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

            return Ok(jobRoles); 
        }
    }
}