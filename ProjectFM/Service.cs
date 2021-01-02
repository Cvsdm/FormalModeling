using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ProjectFM
{
    public class Service : ISender, IReceiver
    {
        public string Name { get; private set; }
        public Semaphore WaitingResponse { get; set; }
        public bool IsDemandAccepted { get; set; }

        private readonly ResourceProvider _provider;

        private int AvailableSeatInWaitingRoom { get; set; }
        private int AvailableNurses { get; set; }
        private int AvailableEmergencyRoom { get; set; }
        private int AvailablePhysicians { get; set; }

        public Dictionary<EnumMessage, Func<bool>> ExecutorArray { get; set; }

        public ConcurrentQueue<Message> Queue { get; set; }
        public Semaphore Semaphore { get; set; }

        private int _numberOfPatientInsideService;

        private int NumberOfPatientInsideService
        {
            get { return _numberOfPatientInsideService; }
            set
            {
                _numberOfPatientInsideService = value;
                // Check if there is still patient inside the service
                if (value != 0) return;

                // If the service is empty
                // Check if there is some free Rooms
                if (AvailableEmergencyRoom > 0)
                {
                    // Start thread to give a room to the provider
                    var threadDonateRoom = new Thread(DonateRoom);
                    threadDonateRoom.Start();
                }

                // Check if there is some free Physicians
                if (AvailablePhysicians > 0)
                {
                    // Start thread to give a physician to the provider
                    var threadDonatePhysician = new Thread(DonatePhysician);
                    threadDonatePhysician.Start();
                }
            }
        }


        /**
         * Constructor of Service 
         */
        public Service(ResourceProvider provider, string name)
        {
            _provider = provider;
            Name = name;

            // Initialize queue and semaphore
            Semaphore = new Semaphore(0, int.MaxValue);
            Queue = new ConcurrentQueue<Message>();
            WaitingResponse = new Semaphore(0, 1);

            // Initialize execution function
            ExecutorArray = new Dictionary<EnumMessage, Func<bool>>(8)
            {
                {EnumMessage.ReleaseSeatInWaitingRoom, ReleaseSeatInWaitingRoom},
                {EnumMessage.AskSeatInWaitingRoom, GetSeatInWaitingRoom},

                {EnumMessage.AskNurse, GetNurse},
                {EnumMessage.ReleaseNurse, ReleaseNurse},

                {EnumMessage.AcquireEmergencyRoom, GetEmergencyRoom},
                {EnumMessage.ReleaseEmergencyRoom, ReleaseEmergencyRoom},

                {EnumMessage.AcquirePhysician, GetPhysician},
                {EnumMessage.ReleasePhysician, ReleasePhysician},

                {EnumMessage.PatientLeaves, PatientLeaves}
            };

            // Initialize base variables
            AvailableSeatInWaitingRoom = 10;
            AvailableNurses = 5;
            AvailableEmergencyRoom = 5;
            AvailablePhysicians = 5;
            NumberOfPatientInsideService = 0;
        }

        /**
         * Function used by patients to send a message to the service
         */
        public void SendMessage(Message message)
        {
            Queue.Enqueue(message);
            Semaphore.Release();
        }

        private void ExecutorFunction(EnumMessage type, ISender sender)
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

                    Console.WriteLine("{0} needs to {1} for {2}", Name, message.Type, message.Sender.Name);
                    var sender = (Patient) message.Sender;
                    ExecutorFunction(message.Type, sender);
                }
            }
        }

        /**
         * Acquire a seat in the waiting Room
         */
        private bool GetSeatInWaitingRoom()
        {
            var rand = new Random();
            if (AvailableSeatInWaitingRoom <= 0 || rand.Next(0, 99) <= 10) return false;

            AvailableSeatInWaitingRoom--;
            _numberOfPatientInsideService++;
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

            // Check if the Resource Provider has a room for us
            _provider.SendMessage(new Message(this, EnumMessage.RequestEmergencyRoom));

            WaitingResponse.WaitOne();

            return IsDemandAccepted;
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

            // No physician is available in the service Thus we can ask the resource provider
            _provider.SendMessage(new Message(this, EnumMessage.RequestPhysician));

            // Wait for the response of the provider
            WaitingResponse.WaitOne();

            return IsDemandAccepted;
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

        private void DonateRoom()
        {
            // Wait before donating
            Thread.Sleep(15000);

            // Check if there is still rooms free
            if (AvailableEmergencyRoom <= 0) return;

            // Give Rooms to ResourceProvider
            while (AvailableEmergencyRoom > 1)
            {
                _provider.SendMessage(new Message(this, EnumMessage.DonateEmergencyRoom));
                WaitingResponse.WaitOne();
                AvailableEmergencyRoom--;
            }
        }

        private void DonatePhysician()
        {
            // Wait before donating
            Thread.Sleep(15000);

            // Give Physician to ResourceProvider
            while (AvailablePhysicians > 1)
            {
                _provider.SendMessage(new Message(this, EnumMessage.DonatePhysician));
                WaitingResponse.WaitOne();
                AvailablePhysicians--;
            }
        }
    }
}