using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.API
{
    public class Message
    {
        public int ChatMessageID { get; set;}
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ChatID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
    }
}
