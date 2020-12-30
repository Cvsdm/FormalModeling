using System;
using System.Threading;

namespace ProjectFM
{
    public class Patient: ISender
    {
        public string Name { get; private set; }
        public Semaphore WaitingResponse { get; set; }
        public bool IsDemandAccepted { get; set; }

        private Service Service { get; set; }

        /**
         * Patient constructor including its name
         */
        public Patient(string name, Service service)
        {
            Name = name;
            Service = service;
            
            // Initialize Semaphore
            WaitingResponse = new Semaphore(0, 1);
        }

        /**
         * Function determining the path of the patient inside the emergency service in the hospital
         */
        public void EmergencyJourney()
        {
            Arrives();
            if (EnterWaitingRoom() == false)
                return;
            NurseStartProcessPaperwork();
            NurseEndProcessPaperwork();
            EnterEmergencyRoom();
            PhysicianStartExamination();
            Leaves();
        }

        /** 
         * The patient enters the hospital
         */
        private void Arrives()
        {
            WriteAction("enters the hospital");
        }

        private bool EnterWaitingRoom()
        {
            // wait for the patient to check in - we consider 5 minutes so here 5 sec
            Thread.Sleep(5000);

            // check if the patient can enter the WR or not
            Service.SendMessage(new Message(this, EnumMessage.AskSeatInWaitingRoom));
            
            // Wait for service response
            WaitingResponse.WaitOne();
            
            WriteAction(IsDemandAccepted ? "was admitted" : "was rejected");
            return IsDemandAccepted;
        }

        /**
         * The nurse starts to process the paperwork
         */
        private void NurseStartProcessPaperwork()
        {
            WriteAction("is starting to fill his paperwork");
            // wait for the patient to fill out paperwork - we consider 5 minutes so here 5 sec
            Thread.Sleep(5000);
            WriteAction("finished his paperwork");
            
            
            // wait for a nurse to be available
            // acquire a nurse
            Service.SendMessage(new Message(this, EnumMessage.AskNurse));
            WaitingResponse.WaitOne();
            WriteAction("- the nurse start to process his paperwork");
        }

        /**
         * The nurse finishes to process the paperwork
         */
        private void NurseEndProcessPaperwork()
        {
            // wait for the nurse to finish processing the paperwork - we consider 5 minutes so here 5 sec
            Thread.Sleep(5000);

            // liberate a nurse
            Service.SendMessage(new Message(this, EnumMessage.ReleaseNurse));
            WaitingResponse.WaitOne();
            WriteAction("- the nurse has finished to process his paperwork");
        }

        /**
         * The patient enters the emergency room
         */
        private void EnterEmergencyRoom()
        {
            // wait for an ER to be available
            // acquire the ER resource
            WriteAction("waits for a free ER");
            Service.SendMessage(new Message(this, EnumMessage.AcquireEmergencyRoom));
            WaitingResponse.WaitOne();

            WriteAction("enters the ER");
        }

        /**
         * The patient is examined when the physician arrives 
         */
        private void PhysicianStartExamination()
        {
            // wait for a physician to be available
            // acquire the resource
            WriteAction("waits for a physician");
            Service.SendMessage(new Message(this, EnumMessage.AcquirePhysician));
            WaitingResponse.WaitOne();

            WriteAction("starts to be examined");
        }

        /**
         * The patient leaves the hospital
         */
        private void Leaves()
        {
            // wait for the end of examination - we consider 10 min so 10 sec
            Thread.Sleep(10000);
            WriteAction("has finished to be examined");

            // release the resource Physician
            Service.SendMessage(new Message(this, EnumMessage.ReleasePhysician));
            WaitingResponse.WaitOne();
            
            // release the resource ER
            Service.SendMessage(new Message(this, EnumMessage.ReleaseEmergencyRoom));
            WaitingResponse.WaitOne();

            // Patient leaves
            Service.SendMessage(new Message(this, EnumMessage.PatientLeaves));
            WriteAction("leaves the hospital");
        }

        private void WriteAction(string message)
        {
            Console.WriteLine("{0} {1}", Name, message);
        }
    }
}