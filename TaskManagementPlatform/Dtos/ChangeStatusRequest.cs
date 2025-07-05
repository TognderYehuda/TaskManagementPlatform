namespace TaskManagementPlatform.Dtos
{
    public class ChangeStatusRequest
    {
        public int NewStatus { get; set; }
        public int AssignedUserId { get; set; }
        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();
    }
}
