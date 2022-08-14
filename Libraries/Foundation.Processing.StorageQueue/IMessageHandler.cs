using Azure.Storage.Queues.Models;

namespace Foundation.Processing.StorageQueue;

public interface IMessageHandler
{
    public Task<bool> HandleMessage(QueueMessage message);
}
