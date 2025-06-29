namespace HMRS_web.API.DTO
{
    public class LeaveRequestCreateDTO
    {
        public Guid EmployeeId { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }

        public string? Reason { get; set; }
    }
}
