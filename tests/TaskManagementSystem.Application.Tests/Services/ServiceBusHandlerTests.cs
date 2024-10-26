using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaskManagementSystem.Application.Options;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Application.Tests.Services
{
    public class ServiceBusHandlerTests
    {
        private Mock<IBus> _mockBus;
        private Mock<ILogger<ServiceBusHandler>> _mockLogger;
        private IOptions<RabbitMqOptions> _options;
        private ServiceBusHandler _serviceBusHandler;
        private const int RetryCount = 2;

        [SetUp]
        public void Setup()
        {
            _mockBus = new Mock<IBus>();
            _mockLogger = new Mock<ILogger<ServiceBusHandler>>();
            _options = Microsoft.Extensions.Options.Options.Create(new RabbitMqOptions { QueueName = "test-queue", RetryCount = RetryCount });

            _serviceBusHandler = new ServiceBusHandler(_mockBus.Object, _options, _mockLogger.Object);
        }

        [Test]
        public async Task SendMessage_ShouldSendToQueueAndLogInformation_WhenMessageIsSentSuccessfully()
        {
            // Arrange
            var message = new { Content = "Test Message" };
            var mockSendEndpoint = new Mock<ISendEndpoint>();
            _mockBus.Setup(bus => bus.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(mockSendEndpoint.Object);

            // Act
            await _serviceBusHandler.SendMessage(message);

            // Assert
            mockSendEndpoint.Verify(endpoint => 
                endpoint.Send(It.Is<object>(m => m.Equals(message)), It.IsAny<CancellationToken>()), Times.Once);

            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Message sent to queue")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendMessage_ShouldLogError_WhenGetSendEndpointFails()
        {
            // Arrange
            var message = new { Content = "Test Message" };
            _mockBus.Setup(bus => bus.GetSendEndpoint(It.IsAny<Uri>()))
                    .ThrowsAsync(new Exception("Failed to get endpoint"));

            // Act
            await _serviceBusHandler.SendMessage(message);

            // Assert
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to send message")),
                It.Is<Exception>(ex => ex.Message == "Failed to get endpoint"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendMessage_ShouldLogErrorAfterRetries_WhenSendMessageFails()
        {
            // Arrange
            var message = new { Content = "Test Message" };
            var mockSendEndpoint = new Mock<ISendEndpoint>();
            _mockBus.Setup(bus => bus.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(mockSendEndpoint.Object);
            mockSendEndpoint.Setup(se => se.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Send failed"));

            // Act
            await _serviceBusHandler.SendMessage(message);

            // Assert
            mockSendEndpoint.Verify(se =>
                se.Send(It.Is<object>(m => m.Equals(message)), It.IsAny<CancellationToken>()), Times.Exactly(RetryCount + 1));
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to send message")),
                It.Is<Exception>(ex => ex.Message == "Send failed"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void ReceiveMessage_ShouldConfigureReceiveEndpointCorrectly()
        {
            // Arrange
            var receiveEndpointConfiguratorMock = new Mock<IReceiveEndpointConfigurator>();
            _mockBus.Setup(bus => bus.ConnectReceiveEndpoint(It.IsAny<string>(), It.IsAny<Action<IReceiveEndpointConfigurator>>()))
                    .Callback<string, Action<IReceiveEndpointConfigurator>>((queueName, configure) =>
                    {
                        configure(receiveEndpointConfiguratorMock.Object);
                    });

            // Act
            _serviceBusHandler.ReceiveMessage<object>();

            // Assert
            _mockBus.Verify(bus => 
                bus.ConnectReceiveEndpoint(_options.Value.QueueName, It.IsAny<Action<IReceiveEndpointConfigurator>>()), Times.Once);
        }

        [Test]
        public void ReceiveMessage_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Failed to connect to the bus";
            _mockBus.Setup(bus => bus.ConnectReceiveEndpoint(It.IsAny<string>(), It.IsAny<Action<IReceiveEndpointConfigurator>>()))
                .Throws(new Exception(exceptionMessage));


            // Act
            _serviceBusHandler.ReceiveMessage<object>();

            // Assert
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Failed to recieve message from")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
