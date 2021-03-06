﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ProjectFM
{
    public class ResourceProvider : IReceiver
    {
        private int RoomBuffer { get; set; }
        private int PhysicianBuffer { get; set; }

        // IReceiver
        public Semaphore Semaphore { get; set; }

        public Dictionary<EnumMessage, Func<bool>> ExecutorArray { get; set; }

        public ConcurrentQueue<Message> Queue { get; set; }

        /**
         * Constructor of Resource Provider
         */
        public ResourceProvider()
        {
            RoomBuffer = 0;
            PhysicianBuffer = 0;

            // Initialize queue and semaphore
            Semaphore = new Semaphore(0, int.MaxValue);
            Queue = new ConcurrentQueue<Message>();

            // Initialize execution function
            ExecutorArray = new Dictionary<EnumMessage, Func<bool>>(8)
            {
                {EnumMessage.RequestEmergencyRoom, RequestRoom},
                {EnumMessage.RequestPhysician, RequestPhysician},
                {EnumMessage.DonatePhysician, DonatePhysician},
                {EnumMessage.DonateEmergencyRoom, DonateRoom},
            };
        }

        /**
         * Function uses by Services to request a additional Room 
         */
        private bool RequestRoom()
        {
            // Check if there is a room to give
            if (RoomBuffer <= 0) return false;

            RoomBuffer--;
            return true;
        }

        /**
         * Function uses by Services to donate a Room 
         */
        private bool DonateRoom()
        {
            RoomBuffer++;
            return true;
        }

        /**
         * Function uses by Services to request a additional Physician 
         */
        private bool RequestPhysician()
        {
            // Check if there is a physician available 
            if (PhysicianBuffer <= 0) return false;

            PhysicianBuffer--;
            return true;
        }

        /**
         * Function uses by Services to donate a Physician 
         */
        private bool DonatePhysician()
        {
            PhysicianBuffer++;
            return true;
        }

        /**
         * Function used by services to send a message to the provider
         */
        public void SendMessage(Message message)
        {
            Queue.Enqueue(message);
            Semaphore.Release();
        }

        public void ExecutorFunction(EnumMessage type, ISender sender)
        {
            // launch the function corresponding to the type of message
            var result = ExecutorArray[type]();

            // send response to sender
            sender.IsDemandAccepted = result;
            sender.WaitingResponse.Release();
        }


        /**
         * Loop function to listen to events
         */
        public void Loop()
        {
            while (true)
            {
                Message message;
                Semaphore.WaitOne();
                if (Queue.TryDequeue(out message))
                {
                    if (message.Type == EnumMessage.EndJob)
                        return;

                    Console.WriteLine("\t Provider needs to do {0} for {1}", message.Type, message.Sender.Name);
                    ExecutorFunction(message.Type, message.Sender);
                }
            }
        }
    }
}