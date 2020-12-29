using System;
using System.Threading;

namespace MiniProjectFM
{
    public class Service : ISender
    {
        public string Name { get; private set; }
        public Semaphore WaitingResponse { get; set; }
        public bool? IsDemandAccepted { get; set; }

        private readonly ResourceManager _manager;

        private int AvailableSeatInWaitingRoom { get; set; }
        private int AvailableNurses { get; set; }
        private int AvailableEmergencyRoom { get; set; }
        private int AvailablePhysicians { get; set; }


        private int _numberOfPatientInsideService;

        private int NumberOfPatientInsideService
        {
            get => _numberOfPatientInsideService;
            set
            {
                _numberOfPatientInsideService = value;
                // Check if there is still patient inside the service
                if (value != 0) return;

                // If the service is empty
                // Check if there is some free Rooms
                while (AvailableEmergencyRoom > 1)
                {
                    // Give Room to ResourceManager
                    // TODO

                    AvailableEmergencyRoom--;
                }

                // Check if there is some free Physicians
                while (AvailablePhysicians > 1)
                {
                    // Give Physician to ResourceManager
                    // TODO
                    AvailablePhysicians--;
                }
            }
        }


        /**
         * Constructor of Service 
         */
        public Service(ResourceManager manager, string name)
        {
            _manager = manager;
            Name = name;

            // Initialize base variables
            AvailableSeatInWaitingRoom = 10;
            AvailableNurses = 5;
            AvailableEmergencyRoom = 5;
            AvailablePhysicians = 5;
            NumberOfPatientInsideService = 0;
        }

        public void getMessage(Message message)
        {
        }


        /**
         * Loop function to listen to events
         */
        public void Loop()
        {
        }

        /**
         * Acquire a seat in the waiting Room
         */
        private bool GetSeatInWaitingRoom()
        {
            var rand = new Random();
            if (AvailableSeatInWaitingRoom <= 0 || rand.Next(0, 99) <= 10) return false;

            AvailableSeatInWaitingRoom--;
            return true;
        }

        /**
         * A seat has become free in the waiting room
         */
        private bool ReleaseSeatInWaitingRoom()
        {
            AvailableSeatInWaitingRoom++;
            return true;
        }

        /**
         * Acquire a nurse
         */
        private bool GetNurse()
        {
            if (AvailableNurses <= 0) return false;

            AvailableNurses--;
            return true;
        }

        /**
         * A nurse became free after processing paperwork
         */
        private bool ReleaseNurse()
        {
            AvailableNurses++;
            return true;
        }

        /**
         * Acquire a Emergency Room to be able to examine a patient
         */
        private bool GetEmergencyRoom()
        {
            // Check if we have a free room in the service
            if (AvailableEmergencyRoom > 0)
            {
                AvailableEmergencyRoom--;
                return true;
            }

            // Check if the Resource Manager has a room for us
            _manager.SendMessage(new Message(this, EnumMessage.requestEmergencyRoom));
            
            WaitingResponse.WaitOne();

            switch (IsDemandAccepted)
            {
                case true:
                    // TODO : see if need smth
                    IsDemandAccepted = null;
                    return true;
                case false:
                    IsDemandAccepted = null;
                    return false;
                default:
                    throw new Exception("receive null accepted demand");
            }
        }

        /**
         * Function triggered when a emergency room becomes free
         */
        private bool ReleaseEmergencyRoom()
        {
            AvailableEmergencyRoom++;
            return true;
        }

        /**
         * Acquire a physician to proceed to a examination
         */
        private bool GetPhysician()
        {
            if (AvailablePhysicians > 0)
            {
                AvailablePhysicians--;
                return true;
            }

            // No physician is available in the service Thus we can ask the resource manager
            _manager.SendMessage(new Message(this, EnumMessage.requestPhysician));

            // Wait for the response of the manager
            WaitingResponse.WaitOne();

            switch (IsDemandAccepted)
            {
                case true:
                    // TODO : see if need smth
                    IsDemandAccepted = null;
                    return true;
                case false:
                    IsDemandAccepted = null;
                    return false;
                default:
                    throw new Exception("receive null accepted demand");
            }
        }

        /**
     * Function triggered when a physician finished his examination
     */
        private bool ReleasePhysician()
        {
            AvailablePhysicians++;
            return true;
        }

        /**
     * Function triggered when a patient leaves
     */
        private bool PatientLeaves()
        {
            NumberOfPatientInsideService--;
            return true;
        }

        /**
     * Function to donate a room to the resource manager
     */
        private bool DonateRoomToResourceManager()
        {
            if (NumberOfPatientInsideService > 0)
                return false;
            AvailableEmergencyRoom--;
            return true;
        }

        /**
     * Function to donate a physician to the resource manager 
     */
        private bool DonatePhysicianToResourceManager()
        {
            if (NumberOfPatientInsideService > 0)
                return false;
            AvailablePhysicians--;
            return true;
        }
    }
}