using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.API
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleID { get; set; }
        public int UserStatusID { get; set; }
    }
}
