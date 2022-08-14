using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Processing.StorageQueue;

public class QueueReciver
{
    private QueueClient? _client;
    private readonly ILogger _log;
    private readonly IConfiguration _config;
    private readonly IMessageHandler _handler;

    public QueueReciver(IConfiguration config, ILogger<QueueReciver> log, IMessageHandler handler)
    {
        _log = log;
        _config = config;
        _handler = handler;
    }

    public async Task StartReceiveAsync(CancellationToken stoppingToken)
    {
        if (_client == null)
            //_client = await _config.GetQueueClient();

        await ProcessQueueItemsAsync(_client, stoppingToken);
    }

    private async Task ProcessQueueItemsAsync(QueueClient client, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await client.ReceiveMessagesAsync(5, TimeSpan.FromMinutes(5), stoppingToken);
                if (messages != null && messages.Value.Length > 0)
                {
                    await ExecuteAsync(client, messages, stoppingToken);
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to process send email Message");
            }
        }
    }

    private async Task ExecuteAsync(QueueClient client, Response<QueueMessage[]> messages, CancellationToken stoppingToken)
    {
        foreach (var message in messages.Value)
        {
            if (message.DequeueCount > 9)
            {
                _log.LogError($"Processing send email Message '{message.MessageId}' for the {message.DequeueCount} time. Something is broken...");
            }
            else if (message.DequeueCount > 4)
            {
                _log.LogWarning($"Processing send email Message '{message.MessageId}' for the {message.DequeueCount} time. Something is not working...");
            }
            else
            {
                _log.LogInformation($"Processing send email Message '{message.MessageId}' for the {message.DequeueCount} time.");
            }

            try
            {
                var success = await _handler.HandleMessage(message);

                if (success)
                    await client.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"An error occurred when sending email '{message.MessageId}'");
            }
        }
    }
}