namespace MiniProjectFM
{
    public class Patient
    {
        private string Name { get; set; }
        
        /**
         * Patient constructor including its name
         */
        public Patient(string name)
        {
            Name = name;
        }
        
        /**
         * Function determining the path of the patient inside the emergency service in the hospital
         */
        public void EmergencyJourney()
        {
            Arrives();
            if (CheckIn() == false)
             return;
            EnterWaitingRoom();
            // ESTCE QUE ON FAIT LA PERSONNE REMPLI LE PAPERWORK ? paperwork(); 
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
            
        }

        /**
         * The patient checks in into the emergency service
         */
        private bool CheckIn()
        {
            return false;
        }

        private void EnterWaitingRoom()
        {
         
        }

        /**
         * The nurse starts to process the paperwork
         */
        private void NurseStartProcessPaperwork()
        {
            
        }

        /**
         * The nurse finishes to process the paperwork
         */
        private void NurseEndProcessPaperwork()
        {
            
        }

        /**
         * The patient enters the emergency room
         */
        private void EnterEmergencyRoom()
        {
            
        }

        /**
         * The patient is examined when the physician arrives 
         */
        private void PhysicianStartExamination()
        {
            
        }

        /**
         * The patient leaves the hospital
         */
        private void Leaves()
        {
            
        }
        
    }
}