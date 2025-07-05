using System.ComponentModel.DataAnnotations;
using TaskManagementPlatform.Models;

namespace TaskManagementPlatform.ViewModel
{
    public class CreateTaskViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string TaskType { get; set; }

        [Required]
        public int AssignedUserId { get; set; }

        public List<User> AvailableUsers { get; set; } = new List<User>();
    }
}
