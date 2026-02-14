namespace OnlineFoodOrderingSystem.Models
{
    // View model used for displaying error pages
    // Contains information about the error for debugging purposes
    public class ErrorViewModel
    {
        // Unique identifier for the request that caused the error
        // Used for tracking and debugging in logs
        public string? RequestId { get; set; }

        // Determines whether to show the RequestId on the error page
        // Only shown when RequestId is available
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}