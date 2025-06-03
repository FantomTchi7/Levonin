using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.WS.Client
{
    public class InitialConnection: WSMessage
    {
        public string header { get; set; } = "InitialConnection";
        public string token { get; set; }
        public string name { get; set; }
        public int userId { get; set; }
        
    }
}
