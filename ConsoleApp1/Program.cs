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
            var service1 = new ServiceHost(typeof(ServiceImps.Service1Imp), new Uri("http://localhost:8080/service1"));
            Console.WriteLine(nameof(service1) + " opening...");
            service1.Open();
            Console.WriteLine(nameof(service1) + " opened");
            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
