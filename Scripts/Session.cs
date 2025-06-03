using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Levonin.Scripts.Models;

namespace Levonin.Scripts
{
	public partial class Session: Node
	{
		public string Username { get; set; }
		public int UserID { get; set; }
		public bool LoggedIn { get; set; } = false;
	   
		public static Session Instance { get; set; }
		public string Token { get; set;}
		public Channel CurrentChannel { get; set; }

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
			UserID = 0;
			LoggedIn = false;
			Token = null;
			
		}
	}
}
