using System.Threading;

namespace MiniProjectFM
{
    public interface ISender
    {
        string Name { get; }
        
        Semaphore WaitingResponse { get; set; }
        bool IsDemandAccepted { get; set; }
        
    }
}