using HMRS_web.API.Models;

namespace HMRS_web.API.DTO
{
    public class DeReadDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
