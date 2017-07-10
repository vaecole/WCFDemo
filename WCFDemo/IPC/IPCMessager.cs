using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFDemo.Entities;
using System.ServiceModel;
using System.Net.Sockets;

namespace WCFDemo.IPC
{
    public class IPCMessager : IIPC
    {
        Socket socket;
        public IPCMessager(string host,int port)
        {
        }
        public bool Hi()
        {
            throw new NotImplementedException();
        }

        public AIPCEntity SendChatMSG(AIPCEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
