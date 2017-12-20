using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoClass dc = new DemoClass();
            for (int i = 0; i < 100; i++)
            {
                new Task(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        Thread.Sleep(new Random().Next(500, 1000));
                        string user = (new Random().Next(1, 100) % 3).ToString();
                        dc.AddRefund(user);
                        user = (new Random().Next(1, 100) % 3).ToString();
                        dc.ConsumeRefund(user);
                        user = (new Random().Next(1, 100) % 3).ToString();
                        dc.HowManyRefund(user);
                    }
                }).Start();
            }
            Thread.Sleep(int.MaxValue);
        }
      }
}
