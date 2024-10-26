using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Text.Json;
using TaskManagementSystem.Application.Events;
using TaskManagementSystem.Application.Options;

namespace TaskManagementSystem.Application.Services
{
    public class ServiceBusHandler : IServiceBusHandler
    {
        private readonly IBus _bus;
        private readonly string _queueName;
        private readonly ILogger<ServiceBusHandler> _logger;
        private readonly ResiliencePipeline _pipeline;
        private readonly int _retryCount;

        public ServiceBusHandler(
            IBus bus,
            IOptions<RabbitMqOptions> options, 
            ILogger<ServiceBusHandler> logger)
        {
            _bus = bus;
            _logger = logger;
            _retryCount = options.Value.RetryCount;
            _queueName = options.Value.QueueName;
            _pipeline = InitPipeline();
        }

        public async Task SendMessage<T>(T message)
        {
            _logger.LogInformation("Starting to send message to queue: {QueueName}", _queueName);

            try
            {
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_queueName}"));
                await _pipeline.ExecuteAsync(async (ct) =>
                {
                    await endpoint.Send(message, ct);
                });
                
                _logger.LogInformation("Message sent to queue: {QueueName}", _queueName);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to {@queue} queue after {@retriesCount} attampts", _queueName, _retryCount);
            }
        }

        public void ReceiveMessage<T>() where T : class
        {
            _logger.LogInformation($"Starting to recieve  messages");
            try
            {
                _bus.ConnectReceiveEndpoint(_queueName, cfg =>
                {
                    cfg.Handler<T>(async (context) => await HandleMessage(context.Message));
                    cfg.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Interval(_retryCount, TimeSpan.FromSeconds(2));
                    });
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to recieve message from {@queue} queue after {@retriesCount} attampts", _queueName, _retryCount);
            }
        }

        private async Task HandleMessage<T>(T message)
        {
            _logger.LogInformation("Message recived: {@message}", JsonSerializer.Serialize(message));

            // System action simulation
            await Task.Delay(TimeSpan.FromSeconds(1));

            var confirmationEvent = new ActionCompletedEvent
            {
                ActionName = "Message Received",
                Success = true
            };

            await _bus.Publish(confirmationEvent);
        }

        private ResiliencePipeline InitPipeline()
        {
            return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                Delay = TimeSpan.FromSeconds(2),
                MaxRetryAttempts = _retryCount,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = retryArguments =>
                {
                    var exceptionMessage = retryArguments.Outcome.Exception?.Message ?? "Unknown error";
                    _logger.LogWarning("Retry {RetryCount} for operation failed. Waiting {Delay} before next retry. Error: {ErrorMessage}",
                        retryArguments.AttemptNumber, retryArguments.RetryDelay, exceptionMessage);

                    return ValueTask.CompletedTask;
                },
            })
            .Build();
        }
    }
}
