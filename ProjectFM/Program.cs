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
            Console.WriteLine("\t------------------------------------------------------------------------------");
            Console.WriteLine("\t------ Welcome into the emergency room - You will enter our simulation -------");
            Console.WriteLine("\t-- Your simulated time is one second correspond to one minute in real life. --");
            Console.WriteLine("\t------------------------------------------------------------------------------\n\n");
            var rand = new Random();

            // Create the provider thread
            var provider = new ResourceProvider();
            var providerThread = new Thread(provider.Loop) {Name = "Provider"};
            providerThread.Start();
            
            // Create the services threads
            Service[] services =
            {
                new Service(provider, "Service réanimation n°1"),
                new Service(provider, "Service réanimation n°1"),
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
            foreach (var service in services)
            {
                service.SendMessage(new Message(null, EnumMessage.EndJob));
            }
            
            // We stop the provider thread
            provider.SendMessage(new Message(null, EnumMessage.EndJob));
        }
    }
}