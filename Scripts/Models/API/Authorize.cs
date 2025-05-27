using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.API
{
    public class Authorize
    {
        public string username { get; set; }
        public string password { get; set;}

        public Authorize(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
