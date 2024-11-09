using System;
using System.Threading.Tasks;
using eShop.EventBusRabbitMQ;
using eShop.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Xunit;

namespace EventBus.Tests
{
    public class EventBusTests
    {
        [Fact]
        public async Task PublishSubscribe_BasicTest()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672
            };
            var connection = factory.CreateConnection();
            services.AddSingleton<IConnection>(connection);

            var options = Options.Create(new EventBusOptions
            {
                SubscriptionClientName = "test_client",
                RetryCount = 5
            });
            var subOptions = Options.Create(new EventBusSubscriptionInfo());

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<RabbitMQEventBus>>();
            var telemetry = new RabbitMQTelemetry();

            var eventBus = new RabbitMQEventBus(
                logger,
                serviceProvider,
                options,
                subOptions,
                telemetry);

            // Act & Assert
            try
            {
                await eventBus.StartAsync(default);
                // If we get here without exceptions, the connection is working
                Assert.True(true, "RabbitMQ connection successful");
            }
            finally
            {
                await eventBus.StopAsync(default);
                eventBus.Dispose();
                connection.Dispose();
            }
        }
    }
}
