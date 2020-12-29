namespace MiniProjectFM
{
    public class Service
    {
        private string Name { get; set; }
        private ResourceManager _manager;

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
    }
}