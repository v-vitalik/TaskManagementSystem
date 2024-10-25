using TaskManagementSystem.Application.Enums;

namespace TaskManagementSystem.Application.Results
{
    public class AddOperationResult<T> : OperationResult
    {
        public T? Id { get; set; }
        public AddOperationResult(T id) : base()
        {
            Id = id;
        }

        public AddOperationResult(OperationStatus status, List<string> errors) : base(status, errors) { }
    }
}
