using TaskManagementPlatform.Models;

namespace TaskManagementPlatform.ViewModel
{
    public class TaskDetailsViewModel
    {
        public AppTask Task { get; set; }
        public List<User> AvailableUsers { get; set; } = new List<User>();
        public int NewAssignedUserId { get; set; }
        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();

        
        public string PriceQuote1 { get; set; }
        public string PriceQuote2 { get; set; }
        public string Receipt { get; set; }
        public string Specification { get; set; }
        public string BranchName { get; set; }
        public string VersionNumber { get; set; }
    }
}
