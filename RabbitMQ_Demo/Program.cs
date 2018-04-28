using EasyNetQ;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Demo
{
    class Program
    {
        private static string[] names = new string[] { "Mark", "Eva", "Ruby", "Jody" };
        private const string mySBSTPID = "marko";
        static void Main(string[] args)
        {
            var rd = new Random();

            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<MQ.Models.Messages.TaskStartedMsg>(mySBSTPID, msg => Console.WriteLine(msg.TaskId + " Started."));
            var sbId = mySBSTPID + rd.Next(1, 1000000);
            Console.WriteLine(sbId);
            var subRes =  bus.Subscribe<string>(sbId, msg => Console.WriteLine(msg));
            Console.ReadKey();
            subRes.Dispose();
            // Demo demo1 = new Demo(names[rd.Next(3)] + rd.Next(1, 10000));
            //Demo demo2 = new Demo(names[rd.Next(3)] + rd.Next(1, 10000));
            //Demo demo3 = new Demo(names[rd.Next(3)] + rd.Next(1, 10000));
            //Demo demo4 = new Demo(names[rd.Next(3)] + rd.Next(1, 10000));

            //string msg = "Hello ";
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        demo1.SendMSG(msg + DateTime.Now);
            //        Thread.Sleep(rd.Next(1000, 10000));
            //    }
            //});
        }
    }
}
