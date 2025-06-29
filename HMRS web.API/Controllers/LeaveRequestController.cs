using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMRS_web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly HmrsContext context;
        public LeaveRequestController(HmrsContext context)
        {
            this.context = context;
        }

        // Employee requesting new leave
        [HttpPost]
        public async Task<IActionResult> createLeaveRequest([FromBody] LeaveRequestCreateDTO requestDTO) 
        {
            if (!await this.context.Employees.AnyAsync(e => e.Id == requestDTO.EmployeeId)) 
            {
                return BadRequest("Invalid EmployeeId");
            }

            // Create a new leave request
            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = requestDTO.EmployeeId, 
                Reason = requestDTO.Reason,        
                FromDate = requestDTO.FromDate,  
                ToDate = requestDTO.ToDate,       
            };

            this.context.LeaveRequests.Add(leaveRequest);
            await this.context.SaveChangesAsync();

            //return leave request after creating it
            var readdto = new LeaveRequestReadDTO
            {
                Id = Guid.NewGuid(),
                EmployeeId = requestDTO.EmployeeId,
                Reason = requestDTO.Reason,
                FromDate = requestDTO.FromDate,
                ToDate = requestDTO.ToDate,
            };
            return Ok(leaveRequest);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Manager,Admin")] 
        public async Task<IActionResult> ApproveLeaveRequest(Guid id, [FromBody] Guid approverId)
        {
            var leaveRequest = await this.context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound(); 
            }

            if (!await this.context.Employees.AnyAsync(e => e.Id == approverId))
            {
                return BadRequest("Invalid ApproverId"); 
            }

   
            leaveRequest.Status = "Approved"; 
            leaveRequest.ApprovedBy = approverId; 
            await this.context.SaveChangesAsync(); 

            return NoContent(); 
        }
        //Admiin or manager to reject leave reques
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> RejectLeaveRequest(Guid id, [FromBody] Guid ApproveId)
        {
            var leaverequest = await this.context.LeaveRequests.FindAsync(id);
            if (leaverequest == null)
            {
                return NotFound();
            }
            if (!await this.context.LeaveRequests.AnyAsync(e => e.Id == ApproveId))
            {
                return BadRequest("Invalid Id");
            }

            leaverequest.Status = "Reject";
            leaverequest.ApprovedBy = ApproveId;
            await this.context.SaveChangesAsync();

            return NoContent();
        }
        //employees can see thier own request and admins can also view departments request
        [HttpGet]
        public async Task<IActionResult> GetLeaveRequest ()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
            {
                return NotFound("Employee not found");
            }
            var employee = await this.context.Employees.FirstOrDefaultAsync(e=>e.UserId == userId);
            if(employee== null)
            {
                return NotFound();
            }
            if(User.IsInRole("Manager")|| User.IsInRole("Admin"))
            {
                var requests = await this.context.LeaveRequests
                    .Where(lr => this.context.Employees
                        
                        .Select(e => e.Id)
                        .Contains(lr.EmployeeId))
                    .Select(lr => new LeaveRequestReadDTO
                    {
                        Id = lr.Id,
                        EmployeeId = lr.EmployeeId,
                        
                        ApprovedBy = lr.ApprovedBy,
                        ApprovedByNavigation = lr.ApprovedByNavigation != null ? lr.ApprovedByNavigation.FullName : null,
                        FromDate = lr.FromDate,
                        ToDate = lr.ToDate,
                        Reason = lr.Reason,
                        Status = lr.Status,
                        RequestedAt = lr.RequestedAt
                    })
                    .ToListAsync();

                return Ok(requests); // Return 200 with list of requests
            }
            else
            {
                // Employees see only their own requests
                var requests = await this.context.LeaveRequests
                    .Where(lr => lr.EmployeeId == employee.Id)
                    .Select(lr => new LeaveRequestReadDTO
                    {
                        Id = lr.Id,
                        EmployeeId = lr.EmployeeId,
                        
                        ApprovedBy = lr.ApprovedBy,
                        ApprovedByNavigation = lr.ApprovedByNavigation != null ? lr.ApprovedByNavigation.FullName : null,
                        FromDate = lr.FromDate,
                        ToDate = lr.ToDate,
                        Reason = lr.Reason,
                        Status = lr.Status,
                        RequestedAt = lr.RequestedAt
                    })
                    .ToListAsync();

                return Ok(requests); 
            }
        }
    }
}