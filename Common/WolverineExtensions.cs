using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Common;

public static class WolverineExtensions
{
    public static async Task UseWolverineWithRabbitMqAsync(
        this IHostApplicationBuilder builder,
        Action<WolverineOptions>? configureMessageBus)
    {
        
        var retryPolicy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount) =>
                {
                    Console.WriteLine($"Retrying {retryCount} of {timeSpan.TotalSeconds} seconds after {exception.Message}");
                }
            );

        await retryPolicy.ExecuteAsync(async () =>
        {
            var endpoint = builder.Configuration.GetConnectionString("messaging")
                           ?? throw new InvalidOperationException("No messaging connection string found");
            var factory = new ConnectionFactory
            {
                Uri = new Uri(endpoint)
            };
            await using var connection = await factory.CreateConnectionAsync();
        });
        
        builder.Services.AddOpenTelemetry().WithTracing(traceProviderBuilder =>
        {
            traceProviderBuilder.SetResourceBuilder(ResourceBuilder
                    .CreateDefault().AddService(builder.Environment.ApplicationName))
                .AddSource("Wolverine");
        });
        
        builder.UseWolverine(opts =>
        {
            opts.UseRabbitMqUsingNamedConnection("messaging")
                .AutoProvision()
                .DeclareExchange("questions");
            
            configureMessageBus?.Invoke(opts);
        });
    }
}