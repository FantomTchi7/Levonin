using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.WS.Client
{
    public class MessageInChannel: WSMessage
    {
        public string header { get; set; } = "MessageInChannel";
        public int channel { get; set; }
    }
}
