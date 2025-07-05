using TaskManagementPlatform.Models;

namespace TaskManagementPlatform.Services
{
    public interface ITaskService
    {
        Task<AppTask> CreateTaskAsync(string title, string taskType, int assignedUserId);
        Task<AppTask> GetTaskAsync(int taskId);
        Task<IEnumerable<AppTask>> GetUserTasksAsync(int userId);
        Task<AppTask> ChangeTaskStatusAsync(int taskId, int newStatus, int assignedUserId, Dictionary<string, string> customFields = null);
        Task<AppTask> CloseTaskAsync(int taskId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<bool> ValidateStatusChangeAsync(AppTask task, int newStatus, Dictionary<string, string> customFields = null);
    }
}
