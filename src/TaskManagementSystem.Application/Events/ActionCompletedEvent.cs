namespace TaskManagementSystem.Application.Events
{
    public class ActionCompletedEvent
    {
        public string ActionName { get; set; }
        public bool Success { get; set; }
    }
}
