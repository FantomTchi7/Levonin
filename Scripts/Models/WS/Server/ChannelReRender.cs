using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.WS.Server
{
    public class ChannelReRender: WSMessage
    {
        public string header { get; set; }
        public int channel { get; set; }
    }
}
