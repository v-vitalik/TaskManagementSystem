namespace TaskManagementSystem.Application.Options
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMQ";

        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public int RetryCount { get; set; }
    }

}
