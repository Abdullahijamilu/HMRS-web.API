//using HMRS_web.API.DTO;
//using HMRS_web.API.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HMRS_web.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class EvaluationController : ControllerBase
//    {
//        private readonly HmrsContext _context;

//        public EvaluationController(HmrsContext context)
//        {
//            _context = context;
//        }
//        [HttpGet]
//        [Authorize("Manager,Admin")]
//        public async Task<IActionResult> Evaluateemployee([FromBody] EvaluationCreateDTO dto)
//        {
//            //validating employee and reviewer
//            if (!await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId))
//                return BadRequest("Invalid EmployeeId");
//            if (!await _context.Employees.AnyAsync(e => e.Id == dto.ReviewerId))
//                return BadRequest("Invalid ReviewerId");

//            // Create evaluation
//            var evaluation = new Evaluation
//            {
//                Id = Guid.NewGuid(),
//                EmployeeId = dto.EmployeeId,
//                ReviewerId = dto.ReviewerId,
//                Score = dto.Score,
//                Remarks = dto.Remarks,
//                //EvaluationDate = dto.DateOnly
//            };

//            _context.Evaluations.Add(evaluation);
//            await _context.SaveChangesAsync();

//            // Return response
//            var readDto = new EvalautionReadDTO
//            {
//                Id = evaluation.Id,
//                EmployeeId = evaluation.EmployeeId,
//                //EmployeeName = (await _context.Employees.FindAsync(evaluation.EmployeeId))?.FullName ?? "Unknown",
//                ReviewerId = evaluation.ReviewerId,
//                //ReviewerName = (await _context.Employees.FindAsync(evaluation.ReviewerId))?.FullName ?? "Unknown",
//                Score = evaluation.Score,
//                Remarks = evaluation.Remarks,
//                EvaluationDate = evaluation.EvaluationDate
//            };

//            return CreatedAtAction(nameof(GetEvaluation), new { id = evaluation.Id }, readDto);
//        }

       
//        // Employees see their evaluations, Managers/Admins see department evaluations
//        [HttpGet]
//        public async Task<IActionResult> GetEvaluations()
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId))
//                return Unauthorized();

//            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
//            if (employee == null)
//                return BadRequest("Employee not found for user");

//            if (User.IsInRole("Manager") || User.IsInRole("Admin"))
//            {
//                var departmentId = employee.DepartmentId;
//                var evaluations = await _context.Evaluations
//                    .Where(e => _context.Employees
//                        .Where(emp => emp.DepartmentId == departmentId)
//                        .Select(emp => emp.Id)
//                        .Contains(e.EmployeeId))
//                    .Select(e => new EvalautionReadDTO
//                    {
//                        Id = e.Id,
//                        EmployeeId = e.EmployeeId,
//                       // EmployeeName = e.Employee.FullName,
//                        ReviewerId = e.ReviewerId,
//                        //ReviewerName = e.Reviewer.FullName,
//                        Score = e.Score,
//                        Remarks = e.Remarks,
//                        EvaluationDate = e.EvaluationDate
//                    })
//                    .ToListAsync();
//                return Ok(evaluations);
//            }
//            else
//            {
//                var evaluations = await _context.Evaluations
//                    .Where(e => e.EmployeeId == employee.Id)
//                    .Select(e => new EvalautionReadDTO
//                    {
//                        Id = e.Id,
//                        EmployeeId = e.EmployeeId,
//                        //EmployeeName = e.Employee.FullName,
//                        ReviewerId = e.ReviewerId,
//                        //ReviewerName = e.Reviewer.FullName,
//                        Score = e.Score,
//                        Remarks = e.Remarks,
//                        EvaluationDate = e.EvaluationDate
//                    })
//                    .ToListAsync();
//                return Ok(evaluations);
//            }
//        }

//        // GET: api/Evaluation/{id}
//        // Get a specific evaluation
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetEvaluation(Guid id)
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId))
//                return Unauthorized();

//            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
//            if (employee == null)
//                return BadRequest("Employee not found for user");

//            var evaluation = await _context.Evaluations
//                .Where(e => e.Id == id)
//                .Select(e => new EvalautionReadDTO
//                {
//                    Id = e.Id,
//                    EmployeeId = e.EmployeeId,
//                    //EmployeeName = e.Employee.FullName,
//                    ReviewerId = e.ReviewerId,
//                    //ReviewerName = e.Reviewer.FullName,
//                    Score = e.Score,
//                    Remarks = e.Remarks,
//                    EvaluationDate = e.EvaluationDate
//                })
//                .FirstOrDefaultAsync();

//            if (evaluation == null)
//                return NotFound();

//            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && evaluation.EmployeeId != employee.Id)
//                return Forbid();

//            if (User.IsInRole("Manager") || User.IsInRole("Admin"))
//            {
//                var evaluatedEmployee = await _context.Employees.FindAsync(evaluation.EmployeeId);
//                if (evaluatedEmployee?.DepartmentId != employee.DepartmentId)
//                    return Forbid();
//            }

//            return Ok(evaluation);
//        }
//    }
//}
