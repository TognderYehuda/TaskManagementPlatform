using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementPlatform.Models
{
    public class TaskCustomField
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; }

        [Required]
        public string FieldValue { get; set; }

        [StringLength(50)]
        public string FieldType { get; set; } = "string"; // string, int, decimal, etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        [ForeignKey("TaskId")]
        public virtual AppTask appTask { get; set; }
    }
}
