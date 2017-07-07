using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo
{
    class GlobalCache
    {
        public static Dictionary<string, int> MoniteredTasks = new Dictionary<string, int>();
    }
}
