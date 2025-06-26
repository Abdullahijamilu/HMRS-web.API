using System.Data;
using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using HMRS_web.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMRS_web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly AuthenticateServices services;
        private readonly HmrsContext context;

        public EmployeeController(AuthenticateServices services, HmrsContext context)
        {
            this.services = services;
            this.context = context;
        }

        public object Id { get; private set; }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateEmployee([FromBody] ECreateDTO eCreate)
        {
            if (eCreate == null || string.IsNullOrEmpty(eCreate.UserName) || string.IsNullOrEmpty(eCreate.Email) || string.IsNullOrEmpty(eCreate.Password))
            {
                var registerdto = new RegisterDTO
                {
                    UserName = eCreate.UserName,
                    Email = eCreate.Email,
                    Password = eCreate.Password,
                    Role = eCreate.Role,
                    Id = eCreate.Id
                };

                var result = await this.services.RegisterUser(registerdto);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                var user = await this.context.Users.FirstOrDefaultAsync(u => u.UserName == eCreate.UserName);
                var userId = user.Id;
                var role = eCreate.Role;
            }

            var employee = new Employee
            {
                FullName = eCreate.FullName,
                HireDate = eCreate.HireDate,
                Address = eCreate.Address,
                Phone = eCreate.Phone,
                DepartmentId = eCreate.DepartmentId,
            };
            this.context.Add(employee);
            await this.context.SaveChangesAsync();

            var readDto = await this.context.Employees
                .Where(e => e.Id == employee.Id)
                .Select(e => new EReadDTO
                {
                    FullName = e.FullName,
                    Phone = e.Phone,
                    Address = e.Address,
                    DepartmentId = e.DepartmentId,
                    HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                    DepartmentName = e.Department != null ? e.Department.Name : null,
                    UserName = e.User != null ? e.User.UserName : null
                })
                .FirstOrDefaultAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, readDto);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EReadDTO>>> GetEmployee()
        {
            var employees = await this.context.Employees
                .Select(e => new EReadDTO
                {
                    FullName = e.FullName,
                    JobRole = e.JobRole,
                    HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                    DepartmentName = e.Department != null ? e.Department.Name : null,
                    UserName = e.User != null ? e.User.UserName : null,
                    Role = this.context.UserRoles
                        .Where(ur => ur.UserId == e.UserId)
                        .Join(this.context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(employees);
        }
        //[HttpGet("{id}")]
        //public async Task<ActionResult<EReadDTO>> GetEmployee(Guid id)
        //{
        //    var employee = await this.context.Employees
        //        .Where(e => e.Id == id)
        //        .Select(e => new EReadDTO
        //        {
        //            FullName = e.FullName,
        //            Address = e.Address,
        //            HireDate = e.HireDate.HasValue ? e.HireDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
        //            DepartmentName = e.Department.Name,
        //            UserName = e.User != null ? e.User.UserName : null,
        //            Role = this.context.UserRoles
        //                .Where(ur => ur.UserId == e.UserId)
        //                .Join(this.context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
        //                .FirstOrDefault()
        //        })
        //        .FirstOrDefaultAsync();

        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(employee);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<EReadDTO>> GetEmployee(Guid id)
        {
            var employee = await this.context.Employees
                .Where(e => e.Id == id)
                .Select(e => new EReadDTO
                {
                    FullName = e.FullName,
                    Address = e.Address,
                    HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                    DepartmentName = e.Department.Name,
                    UserName = e.User != null ? e.User.UserName : null,
                    Role = this.context.UserRoles
                        .Where(ur => ur.UserId == e.UserId)
                        .Join(this.context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateEmployee(Guid id, EUpdateDTO dto)
        {
            if (id != dto.Id)  
            {
                return BadRequest("ID mismatch");
            }

            var employee = await this.context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            
            employee.FullName = dto.FullName;
            employee.Phone = dto.Phone;
            employee.Address = dto.Address;
            employee.DepartmentId = dto.DepartmentId;

            
            this.context.Employees.Update(employee);
            await this.context.SaveChangesAsync();

            return Ok(employee);   
        }
    }
}

