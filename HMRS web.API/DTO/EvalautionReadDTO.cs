namespace HMRS_web.API.DTO
{
    public class EvalautionReadDTO
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid ReviewerId { get; set; }

        public int Score { get; set; }

        public string? Remarks { get; set; }

        public DateOnly EvaluationDate { get; set; }

    }
}
