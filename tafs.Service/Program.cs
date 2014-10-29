using System;
using System.ServiceProcess;

namespace tafs.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            using (var service = new Service())
            {
                if (Environment.UserInteractive)
                {
                    service.Start();

                    Console.ReadLine();

                    service.Stop();
                    
                }
                else
                    ServiceBase.Run(service);
            }
        }
    }
}
