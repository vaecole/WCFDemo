using EasyNetQ;
using System;
using System.Threading;

namespace MQ_Subscriptor
{
    class Program
    {
        private static string[] names = new string[] { "Mark", "Eva", "Ruby", "Jody" };
        static void Main(string[] args)
        {
            var rd = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");
            while (true)
            {
                var msg = names[rd.Next(3)] + rd.Next(1, 10000) + " Say Hello!";
                Console.WriteLine(msg);
                bus.Publish(msg);
                Thread.Sleep(500);
            }
        }
    }
}
