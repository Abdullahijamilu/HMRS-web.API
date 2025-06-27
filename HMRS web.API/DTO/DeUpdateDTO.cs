namespace HMRS_web.API.DTO
{
    public class DeUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
