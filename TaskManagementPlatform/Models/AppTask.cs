using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementPlatform.Models
{
    public class AppTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string TaskType { get; set; } // "Procurement" or "Development"

        public int CurrentStatus { get; set; } = 1; // Starting status

        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ClosedAt { get; set; }

        // Current assigned user
        public int AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public virtual User AssignedUser { get; set; }

        // Navigation properties
        public virtual ICollection<TaskStatusHistory> StatusHistory { get; set; } = new List<TaskStatusHistory>();
        public virtual ICollection<TaskCustomField> CustomFields { get; set; } = new List<TaskCustomField>();
    }
}
