using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Levonin.Scripts
{
    public partial class Session: Node
    {
        public string Username { get; set; }
        public bool LoggedIn { get; set; } = false;
        public static Session Instance { get; set; }

        public override void _Ready()
        {
            if(Instance != null && Instance != this)
            {
                QueueFree();
                return;
            }
            Instance = this;
        }
        public void Clear()
        {
            Username = null;
        }
    }
}
