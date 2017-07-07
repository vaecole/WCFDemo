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

    public class ControlParameters
    {
        public string Id { get; set; }
        public string DisplayText { get; set; }
        public ControlType ControlType { get; set; }
        public DataType DataType { get; set; }
        public string ParamName { get; set; }
        public bool IsRequired { get; set; }
        public string Remark { get; set; }
        public ControlOptions ControlOptions { get; set; }
        public Dictionary<string, object> DataTypeOptions { get; set; }
    }

    public class ControlOptions
    {
        public string DataSource { get; set; }
        public string DataSourceFilter { get; set; }
        public string ParentField { get; set; }
        public string ChildField { get; set; }
    }

    public enum ControlType
    {
        Input,
        MultiInput,
        CheckBox,
        Dropdown,
        CheckBoxList,
        TimePicker,
        DateTimePicker,
        Composite
    }

    public enum DataType
    {
        String,
        Decimal,
        DateTime,
        Unspecified
    }

}
