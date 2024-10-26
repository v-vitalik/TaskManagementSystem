namespace TaskManagementSystem.Application.Services
{
    public interface IServiceBusHandler
    {
        Task SendMessage<T>(T message);
        void ReceiveMessage<T>() where T : class;
    }
}
