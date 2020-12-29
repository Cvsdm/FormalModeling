using System.Collections.Generic;

namespace MiniProjectFM
{
    public class ResourceManager
    {
        private int RoomBuffer { get; set; }
        private int PhysicianBuffer { get; set; }

        /**
         * Constructor of Resource Manager
         */
        public ResourceManager()
        {
            RoomBuffer = 0;
            PhysicianBuffer = 0;
        }

        /**
         * Function uses by Services to request a additional Room 
         */
        public bool RequestRoom()
        {
            // Check if there is a room to give
            if (RoomBuffer <= 0) return false;
            
            RoomBuffer--;
            return true;
        }

        /**
         * Function uses by Services to donate a Room 
         */
        public bool DonateRoom()
        {
            RoomBuffer++;
            return true;
        }

        /**
         * Function uses by Services to request a additional Physician 
         */
        public bool RequestPhysician()
        {
            // Check if there is a physician available 
            if (PhysicianBuffer <= 0) return false;
            
            PhysicianBuffer--;
            return true;
        }

        /**
         * Function uses by Services to donate a Physician 
         */
        public bool DonatePhysician()
        {
            PhysicianBuffer++;
            return true;
        }

        /**
         * Loop function to listen to events
         */
        public void Loop()
        {
            
        }
    }
}