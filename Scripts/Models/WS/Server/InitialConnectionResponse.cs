using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.WS.Server
{
    public class InitialConnectionResponse: WSMessage
    {
        public string header { get; set; }
        public bool success { get; set; }
    }
}
