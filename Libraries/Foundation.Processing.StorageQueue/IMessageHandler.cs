using Azure.Storage.Queues.Models;

namespace Foundation.Processing.StorageQueue;

public interface IMessageHandler
{
    public Task<bool> HandleMessage(QueueMessage message);
}


public class MessageHandler : IMessageHandler
{
    public Task<bool> HandleMessage(QueueMessage message)
    {
        throw new NotImplementedException();
    }
}