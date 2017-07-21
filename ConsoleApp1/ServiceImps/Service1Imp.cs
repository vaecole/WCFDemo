using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary1;

namespace ConsoleApp1.ServiceImps
{
    class Service1Imp : WcfServiceLibrary1.IMyService
    {
        public string GetData(int value)
        {
            return "Welcome to use Mart's WCF Serice, your int value input is :" + value;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            composite.StringValue = "Welcome to use Mart's WCF Serice, your composite's string value input is :" + composite.StringValue;
            return composite;
        }
    }
}
