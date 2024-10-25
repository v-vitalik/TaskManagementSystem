using TaskManagementSystem.Application.Enums;

namespace TaskManagementSystem.Application.Results
{
    public class GetOperationResult<T> : OperationResult
    {
        public T Data { get; set; }
        public GetOperationResult(T data) : base()
        {
            Data = data;
        }
        public GetOperationResult(OperationStatus status, List<string> errors) : base(status, errors) { }
    }
}
