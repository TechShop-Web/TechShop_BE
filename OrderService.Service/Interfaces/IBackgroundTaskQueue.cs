﻿namespace OrderService.Service.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<IServiceProvider, Task> workItem);
        Task<Func<IServiceProvider, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
