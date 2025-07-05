namespace TaskManagementPlatform.Dtos
{
    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string TaskType { get; set; }
        public int AssignedUserId { get; set; }
    }
}
