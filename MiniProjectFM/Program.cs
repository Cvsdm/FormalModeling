using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading;

namespace MiniProjectFM
{
    internal class Program
    {
        private static string[] _patientNames =
        {
            "Elise", "Julie", "Jean-Paul", "Chloe", "Jacques", "Celine", "John", "Antony", "Patrick", "Jennifer",
            "Thomas", "Clement", "Sophia", "Emmanuel", "Edouard", "Philippe", "Lisa", "Will", "Katie"
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
                var servicesThread = new Thread(service.Loop) {Name = s.Name};
                servicesThread.Start();
            }
            
            
            // Create the patients threads
            var patients = new List<Thread>();
            
            foreach (var name in _patientNames)
            {
                var randomService = rand.Next(0, services.Length);
                var p = new Patient(name, services[randomService]);
                var thread = new Thread(p.EmergencyJourney) {Name = name};
                patients.Add(thread);
                thread.Start();
                Thread.Sleep(1000);
            }
            foreach(var t in patients)
                t.Join();

        }
    }
}