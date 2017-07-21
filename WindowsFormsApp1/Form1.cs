using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfServiceLibrary1;
using WCFDemo.ServiceUtility;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            IMyService service = ServiceHelper.ResolveBasicHttpService<IMyService>("http://localhost:8080/service1");
            textBox1.Text = service.GetData(123);
            int i = 0;
            do
            {
                textBox1.Text += "\r\n" + service.GetDataUsingDataContract(new CompositeType() { StringValue = "Hello, world!" + i }).StringValue;
            } while (i++ < 100);

        }
    }
}
