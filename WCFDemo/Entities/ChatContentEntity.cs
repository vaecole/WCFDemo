using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo.Entities
{
    public class ChatContentEntity:AIPCEntity
    {
        public string TextMessage { get; set; }
        public List<Image> ImagesMessage { get; set; }
    }
}
