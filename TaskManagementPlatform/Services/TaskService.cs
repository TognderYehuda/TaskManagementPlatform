using Microsoft.EntityFrameworkCore;
using TaskManagementPlatform.Data;
using TaskManagementPlatform.Models;


namespace TaskManagementPlatform.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskManagementDbContext _context;

        public TaskService(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<AppTask> CreateTaskAsync(string title, string taskType, int assignedUserId)
        {
            var user = await _context.Users.FindAsync(assignedUserId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (taskType != "Procurement" && taskType != "Development")
                throw new ArgumentException("Invalid task type");

            var task = new AppTask
            {
                Title = title,
                TaskType = taskType,
                CurrentStatus = 1,
                AssignedUserId = assignedUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Record initial status history
            var statusHistory = new TaskStatusHistory
            {
                TaskId = task.Id,
                FromStatus = 0,
                ToStatus = 1,
                AssignedUserId = assignedUserId,
                ChangedAt = DateTime.UtcNow,
                Notes = "Task created"
            };

            _context.TaskStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            return await GetTaskAsync(task.Id);
        }

        public async Task<AppTask> GetTaskAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedUser)
                .Include(t => t.CustomFields)
                .Include(t => t.StatusHistory)
                .ThenInclude(h => h.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<IEnumerable<AppTask>> GetUserTasksAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedUser)
                .Include(t => t.CustomFields)
                .Where(t => t.AssignedUserId == userId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<AppTask> ChangeTaskStatusAsync(int taskId, int newStatus, int assignedUserId, Dictionary<string, string> customFields = null)
        {
            var task = await GetTaskAsync(taskId);
            if (task == null)
                throw new ArgumentException("Task not found");

            if (task.IsClosed)
                throw new InvalidOperationException("Cannot change status of a closed task");

            var user = await _context.Users.FindAsync(assignedUserId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Validate status change rules
            if (!await ValidateStatusChangeAsync(task, newStatus, customFields))
            {
                string errorMessage = GetValidationErrorMessage(task, newStatus, customFields);
                throw new InvalidOperationException($"Status change validation failed: {errorMessage}");
            }

            // Validate sequential forward moves (Rule 4)
            if (newStatus > task.CurrentStatus && newStatus != task.CurrentStatus + 1)
                throw new InvalidOperationException("Forward moves must be sequential");

            var oldStatus = task.CurrentStatus;

            // Update task
            task.CurrentStatus = newStatus;
            task.AssignedUserId = assignedUserId;

            // Add custom fields if provided
            if (customFields != null)
            {
                foreach (var field in customFields)
                {
                    var existingField = task.CustomFields.FirstOrDefault(f => f.FieldName == field.Key);
                    if (existingField != null)
                    {
                        existingField.FieldValue = field.Value;
                    }
                    else
                    {
                        task.CustomFields.Add(new TaskCustomField
                        {
                            TaskId = task.Id,
                            FieldName = field.Key,
                            FieldValue = field.Value,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            // Record status history
            var statusHistory = new TaskStatusHistory
            {
                TaskId = task.Id,
                FromStatus = oldStatus,
                ToStatus = newStatus,
                AssignedUserId = assignedUserId,
                ChangedAt = DateTime.UtcNow,
                Notes = $"Status changed from {oldStatus} to {newStatus}"
            };

            _context.TaskStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            return await GetTaskAsync(taskId);
        }

        private string GetValidationErrorMessage(AppTask task, int newStatus, Dictionary<string, string> customFields)
        {
            if (task.TaskType == "Procurement")
            {
                switch (newStatus)
                {
                    case 2:
                        return "Moving to status 2 requires both PriceQuote1 and PriceQuote2 fields";
                    case 3:
                        return "Moving to status 3 requires Receipt field";
                    default:
                        return "Invalid status for Procurement task";
                }
            }
            else if (task.TaskType == "Development")
            {
                switch (newStatus)
                {
                    case 2:
                        return "Moving to status 2 requires Specification field";
                    case 3:
                        return "Moving to status 3 requires BranchName field";
                    case 4:
                        return "Moving to status 4 requires VersionNumber field";
                    default:
                        return "Invalid status for Development task";
                }
            }

            return "Unknown validation error";
        }

        public async Task<AppTask> CloseTaskAsync(int taskId)
        {
            var task = await GetTaskAsync(taskId);
            if (task == null)
                throw new ArgumentException("Task not found");

            if (task.IsClosed)
                throw new InvalidOperationException("Task is already closed");

            // Check if task is at final status
            var finalStatus = GetFinalStatus(task.TaskType);
            if (task.CurrentStatus != finalStatus)
                throw new InvalidOperationException($"Task can only be closed from status {finalStatus}");

            task.IsClosed = true;
            task.ClosedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> ValidateStatusChangeAsync(AppTask task, int newStatus, Dictionary<string, string> customFields = null)
        {
            // Type-specific validation
            switch (task.TaskType)
            {
                case "Procurement":
                    return ValidateProcurementStatusChange(task, newStatus, customFields);
                case "Development":
                    return ValidateDevelopmentStatusChange(task, newStatus, customFields);
                default:
                    return false;
            }
        }

        private bool ValidateProcurementStatusChange(AppTask task, int newStatus, Dictionary<string, string> customFields)
        {
            switch (newStatus)
            {
                case 1:
                    return true; // Can always go back to created
                case 2:
                    // Need 2 price quotes - check both new fields and existing fields
                    var hasQuote1 = (customFields != null && customFields.ContainsKey("PriceQuote1") && !string.IsNullOrEmpty(customFields["PriceQuote1"])) ||
                                   task.CustomFields.Any(f => f.FieldName == "PriceQuote1" && !string.IsNullOrEmpty(f.FieldValue));

                    var hasQuote2 = (customFields != null && customFields.ContainsKey("PriceQuote2") && !string.IsNullOrEmpty(customFields["PriceQuote2"])) ||
                                   task.CustomFields.Any(f => f.FieldName == "PriceQuote2" && !string.IsNullOrEmpty(f.FieldValue));

                    return hasQuote1 && hasQuote2;

                case 3:
                    // Need receipt - check both new fields and existing fields
                    var hasReceipt = (customFields != null && customFields.ContainsKey("Receipt") && !string.IsNullOrEmpty(customFields["Receipt"])) ||
                                    task.CustomFields.Any(f => f.FieldName == "Receipt" && !string.IsNullOrEmpty(f.FieldValue));

                    return hasReceipt;

                default:
                    return false;
            }
        }

        private bool ValidateDevelopmentStatusChange(AppTask task, int newStatus, Dictionary<string, string> customFields)
        {
            switch (newStatus)
            {
                case 1:
                    return true; // Can always go back to created
                case 2:
                    // Need specification - check both new fields and existing fields
                    var hasSpecification = (customFields != null && customFields.ContainsKey("Specification") && !string.IsNullOrEmpty(customFields["Specification"])) ||
                                          task.CustomFields.Any(f => f.FieldName == "Specification" && !string.IsNullOrEmpty(f.FieldValue));

                    return hasSpecification;

                case 3:
                    // Need branch name - check both new fields and existing fields
                    var hasBranchName = (customFields != null && customFields.ContainsKey("BranchName") && !string.IsNullOrEmpty(customFields["BranchName"])) ||
                                       task.CustomFields.Any(f => f.FieldName == "BranchName" && !string.IsNullOrEmpty(f.FieldValue));

                    return hasBranchName;

                case 4:
                    // Need version number - check both new fields and existing fields
                    var hasVersionNumber = (customFields != null && customFields.ContainsKey("VersionNumber") && !string.IsNullOrEmpty(customFields["VersionNumber"])) ||
                                          task.CustomFields.Any(f => f.FieldName == "VersionNumber" && !string.IsNullOrEmpty(f.FieldValue));

                    return hasVersionNumber;

                default:
                    return false;
            }
        }

        private int GetFinalStatus(string taskType)
        {
            switch (taskType)
            {
                case "Procurement":
                    return 3;
                case "Development":
                    return 4;
                default:
                    return 1;
            }
        }
    }
}
