using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Resources.Scripts.Data
{
    [GlobalClass]
    public partial class UserDataResource: Resource
    {
        [Export]
        public string? UserName { get; set; }
        [Export]
        public string? Token { get; set; }
        [Export]
        public bool IsRegistered { get; set; }
    }
}
