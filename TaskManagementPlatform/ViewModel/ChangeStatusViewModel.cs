namespace TaskManagementPlatform.ViewModel
{
    public class ChangeStatusViewModel
    {
        public int TaskId { get; set; }
        public int NewStatus { get; set; }
        public int AssignedUserId { get; set; }

        
        public string PriceQuote1 { get; set; }
        public string PriceQuote2 { get; set; }
        public string Receipt { get; set; }

        
        public string Specification { get; set; }
        public string BranchName { get; set; }
        public string VersionNumber { get; set; }
    }
}
