using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.API
{
    public class SignUp
    {
        public string username { get; set;}
        public string password {  get; set; }
        public string email { get; set; }
        public SignUp(string username, string password, string email)
        {
            this.username = username;
            this.password = password;
            this.email = email;
        }
    }
}
