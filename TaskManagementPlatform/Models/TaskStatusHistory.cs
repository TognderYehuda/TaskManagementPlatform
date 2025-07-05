using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementPlatform.Models
{
    public class TaskStatusHistory
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public int FromStatus { get; set; }

        public int ToStatus { get; set; }

        public int AssignedUserId { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string Notes { get; set; }

        // Foreign Keys
        [ForeignKey("TaskId")]
        public virtual AppTask Task { get; set; }

        [ForeignKey("AssignedUserId")]
        public virtual User AssignedUser { get; set; }
    }
}
