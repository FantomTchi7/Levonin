using Levonin.Resources.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models
{
    public class Channel
    {
        public int ChatID { get; set; }
        public string ChatName { get; set; }
        public int IsChatCreator { get; set; }
        public int UserID { get; set; }
        public string ImageUrl { get; set; }
        public string ChatType { get; set; }
        public string? Status { get; set; }
    }
}
