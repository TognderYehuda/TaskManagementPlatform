using Microsoft.AspNetCore.Mvc;
using TaskManagementPlatform.Services;
using TaskManagementPlatform.ViewModel;

namespace TaskManagementPlatform.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
           
            var users = await _service.GetUsersAsync();
            var firstUser = users.FirstOrDefault();

            if (firstUser == null)
            {
                ViewBag.Message = "No users found. Please add users to the system.";
                return View(new UserTasksViewModel());
            }

            return RedirectToAction("UserTasks", new { userId = firstUser.Id });
        }

        public async Task<IActionResult> UserTasks(int userId)
        {
            var users = await _service.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var tasks = await _service.GetUserTasksAsync(userId);

            var viewModel = new UserTasksViewModel
            {
                UserId = userId,
                UserName = user.Name,
                Tasks = tasks.ToList(),
                AvailableUsers = users.ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var users = await _service.GetUsersAsync();
            var viewModel = new CreateTaskViewModel
            {
                AvailableUsers = users.ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var task = await _service.CreateTaskAsync(model.Title, model.TaskType, model.AssignedUserId);
                    TempData["Success"] = "Task created successfully!";
                    return RedirectToAction("Details", new { id = task.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var users = await _service.GetUsersAsync();
            model.AvailableUsers = users.ToList();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _service.GetTaskAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var users = await _service.GetUsersAsync();
            var viewModel = new TaskDetailsViewModel
            {
                Task = task,
                AvailableUsers = users.ToList(),
                NewAssignedUserId = task.AssignedUserId
            };

            // Populate custom fields for display
            foreach (var field in task.CustomFields)
            {
                viewModel.CustomFields[field.FieldName] = field.FieldValue;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(ChangeStatusViewModel model)
        {
            try
            {
                var customFields = new Dictionary<string, string>();

                // Add non-empty fields to custom fields dictionary
                if (!string.IsNullOrEmpty(model.PriceQuote1))
                    customFields["PriceQuote1"] = model.PriceQuote1;
                if (!string.IsNullOrEmpty(model.PriceQuote2))
                    customFields["PriceQuote2"] = model.PriceQuote2;
                if (!string.IsNullOrEmpty(model.Receipt))
                    customFields["Receipt"] = model.Receipt;
                if (!string.IsNullOrEmpty(model.Specification))
                    customFields["Specification"] = model.Specification;
                if (!string.IsNullOrEmpty(model.BranchName))
                    customFields["BranchName"] = model.BranchName;
                if (!string.IsNullOrEmpty(model.VersionNumber))
                    customFields["VersionNumber"] = model.VersionNumber;

                await _service.ChangeTaskStatusAsync(model.TaskId, model.NewStatus, model.AssignedUserId, customFields);
                TempData["Success"] = "Task status changed successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = model.TaskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                await _service.CloseTaskAsync(id);
                TempData["Success"] = "Task closed successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = id });
        }

        private string GetStatusDescription(string taskType, int status)
        {
            switch (taskType)
            {
                case "Procurement":
                    return status switch
                    {
                        1 => "Created",
                        2 => "Supplier offers received",
                        3 => "Purchase completed",
                        _ => "Unknown"
                    };
                case "Development":
                    return status switch
                    {
                        1 => "Created",
                        2 => "Specification completed",
                        3 => "Development completed",
                        4 => "Distribution completed",
                        _ => "Unknown"
                    };
                default:
                    return "Unknown";
            }
        }



    }
}
