using TaskManagementPlatform.Models;

namespace TaskManagementPlatform.ViewModel
{
    public class UserTasksViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<AppTask> Tasks { get; set; } = new List<AppTask>();
        public List<User> AvailableUsers { get; set; } = new List<User>();
    }
}
