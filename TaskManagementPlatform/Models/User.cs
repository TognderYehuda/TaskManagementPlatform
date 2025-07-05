using System.ComponentModel.DataAnnotations;

namespace TaskManagementPlatform.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        // Navigation property
        public virtual ICollection<AppTask> AssignedTasks { get; set; } = new List<AppTask>();
    }
}
