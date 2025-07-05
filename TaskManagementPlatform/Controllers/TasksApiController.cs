using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskManagementPlatform.Dtos;
using TaskManagementPlatform.Models;
using TaskManagementPlatform.Services;

namespace TaskManagementPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksApiController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksApiController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var users = await _taskService.GetUsersAsync();
            var allTasks = new List<AppTask>();

            foreach (var user in users)
            {
                var userTasks = await _taskService.GetUserTasksAsync(user.Id);
                allTasks.AddRange(userTasks);
            }

            return Ok(allTasks);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTasks(int userId)
        {
            var tasks = await _taskService.GetUserTasksAsync(userId);
            return Ok(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(request.Title, request.TaskType, request.AssignedUserId);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeTaskStatus(int id, [FromBody] ChangeStatusRequest request)
        {
            try
            {
                var task = await _taskService.ChangeTaskStatusAsync(id, request.NewStatus, request.AssignedUserId, request.CustomFields);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseTask(int id)
        {
            try
            {
                var task = await _taskService.CloseTaskAsync(id);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }


}

