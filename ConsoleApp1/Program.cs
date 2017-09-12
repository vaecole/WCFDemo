using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using WcfServiceLibrary1;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string enc = "59a839879fe5053bd83d8a9c";
            enc = Octopus.Utility.EncryptHelper.Encrypt("59a839879fe5053bd83d8a9c");
            Console.WriteLine(Octopus.Utility.EncryptHelper.Decrypt("bYvAU 3r1b5mDLc71o3mKmV9SKSmM9iqoH/2H75e7og="));
            Console.ReadKey();
            //var service1 = new ServiceHost(typeof(ServiceImps.Service1Imp), new Uri("http://localhost:8080/service1"));
            //Console.WriteLine(nameof(service1) + " opening...");
            //service1.Open();
            //Console.WriteLine(nameof(service1) + " opened");
            //while (true)
            //{
            //    System.Threading.Thread.Sleep(100);
            //}
        }
    }
}
