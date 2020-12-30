using System.Threading;

namespace ProjectFM
{
    public interface ISender
    {
        string Name { get; }
        
        Semaphore WaitingResponse { get; set; }
        bool IsDemandAccepted { get; set; }
        
    }
}