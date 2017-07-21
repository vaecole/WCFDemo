using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using System.Threading;
using System.Security.Cryptography;
using Octopus.Entity;
using WCFDemo.Entities;
using System.IO;

namespace WCFDemo
{
    class aaa
    {
        static void Main(params string[] paras)
        {
            var res = new StringHelper().Replace();
            File.WriteAllText("./res.txt", res);
            Console.WriteLine(res);

        }
    }

}
