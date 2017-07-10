using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFDemo.Entities;

namespace WCFDemo.IPC
{
    public interface IIPC
    {
        bool Hi();
        AIPCEntity SendChatMSG(AIPCEntity entity);
    }
}
