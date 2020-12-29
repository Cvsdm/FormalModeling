namespace MiniProjectFM
{
    public enum MessageType
    {
        askSeatInWaitingRoom,
        releaseSeatInWaitingRoom,

        askNurse,
        releaseNurse,

        acquireEmergencyRoom,
        releaseEmergencyRoom,
        
        requestEmergencyRoom,
        donateEmergencyRoom,

        acquirePhysician,
        releasePhysician,

        requestPhysician,
        donatePhysician,

        patientCheckout,

        terminateProgram
    }
}