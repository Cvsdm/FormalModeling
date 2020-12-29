using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MiniProjectFM
{
    public class ResourceManager
    {
        private int RoomBuffer { get; set; }
        private int PhysicianBuffer { get; set; }
        
        private Semaphore Semaphore { get; set; }
        
        private ConcurrentQueue<Message> Queue { get; set; }
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
         * Function used by services to send a message to the manager
         */
        public void SendMessage(Message message)
        {
            Queue.Enqueue(message);
            Semaphore.Release();
        }

        /**
         * Loop function to listen to events
         */
        public void Loop()
        {
            Message message;
            while (true)
            {
                
            }
        }
    }
}