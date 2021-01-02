using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ProjectFM
{
    public interface IReceiver
    {
        Dictionary<EnumMessage, Func<bool>> ExecutorArray { get; set; }

        ConcurrentQueue<Message> Queue { get; set; }
        Semaphore Semaphore { get; set; }

        void SendMessage(Message message);

        void ExecutorFunction(EnumMessage type, ISender sender);

        void Loop();
    }
}