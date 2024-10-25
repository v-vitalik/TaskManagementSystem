using TaskManagementSystem.Application.Enums;

namespace TaskManagementSystem.Application.Results
{
    public class OperationResult
    {
        public OperationStatus Status { get; set; }
        public List<string> Errors { get; set; }
        public OperationResult()
        {
            Status = OperationStatus.Success;
        }

        public OperationResult(OperationStatus status, List<string> errors)
        {
            Status = status;
            Errors = errors;
        }
    }
}
