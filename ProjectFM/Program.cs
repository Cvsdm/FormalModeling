using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectFM
{
    internal static class Program
    {
        // Variable representing the names of the patient to come into the hospital
        private static readonly string[] PatientNames =
        {
            "Elise", "Katie", "Julie", "Jean-Paul", "Chloe", "Jacques", "Celine", "John", "Antony", "Patrick", "Jennifer",
            "Thomas", "Clement", "Sophia", "Emmanuel", "Edouard", "Philippe", "Lisa", "Will"
        };

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome into the emergency room - You will enter our simulation");
            var rand = new Random();

            // Create the manager thread
            var manager = new ResourceManager();
            var managerThread = new Thread(manager.Loop) {Name = "Manager"};
            managerThread.Start();
            

            // Create the services threads
            Service[] services =
            {
                new Service(manager, "Service réanimation n°1"),
                new Service(manager, "Service réanimation n°1"),
            };
            foreach (var service in services)
            {
                var servicesThread = new Thread(service.Loop) {Name = service.Name};
                servicesThread.Start();
            }
            
            
            // Create the patients threads
            var patients = new List<Thread>();
            
            foreach (var name in PatientNames)
            {
                var randomService = rand.Next(0, services.Length);
                var p = new Patient(name, services[randomService]);
                var thread = new Thread(p.EmergencyJourney) {Name = name};
                patients.Add(thread);
                thread.Start();
                // we create a random between 0 and 6 minutes for the arrival of new patient
                var arrivalTime = rand.Next(0, 6000);
                Thread.Sleep(arrivalTime);
            }
            
            // We wait for the patient thread to terminate one by one
            foreach(var thread in patients)
                thread.Join();
            
            // We stop the services threads
            
            // We stop the manager thread

        }
    }
}