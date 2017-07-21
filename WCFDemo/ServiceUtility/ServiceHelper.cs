using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo.ServiceUtility
{
    public class ServiceHelper
    {
        public static TChannel ResolveBasicHttpService<TChannel>(string address) // where TChannel : System.ServiceModel.Channels.IOutputChannel, System.ServiceModel.Channels.IInputChannel
        {
            var serviceProxy = default(TChannel);
            var channelFactory = new System.ServiceModel.ChannelFactory<TChannel>(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(address)));
            serviceProxy = channelFactory.CreateChannel();
            return serviceProxy;
        }
    }
}
