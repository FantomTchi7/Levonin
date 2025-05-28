using Godot;
using Levonin.Scripts;
using Levonin.Scripts.Handlers;
using Levonin.Scripts.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class LoginHandler: Node
{
	public static LoginHandler Instance { get; set; }
	public override void _Ready()
	{
		if(Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
		
	}
	public async Task<ApiMessage> Login(string name, string password)
	{
		GD.Print("login stared");
		ApiMessage message = await ApiHandler.Instance.Authorize(name, password);
		if (message.Success)
		{
			Session.Instance.Username = name;
			EventHandler.Instance.CallEvent("loggedIn", true);
			GD.Print("Login is success!");
			Controller.Instance.CurrentPageEnum = Levonin.Resources.Scripts.GUI.Page.Messenger;
		}
		else
		{
			GD.Print(message.ErrorMessage);
			EventHandler.Instance.CallEvent("loggedIn", false);
		}
		return message;
	}
	public async Task<ApiMessage> SignUp(string name, string password)
	{
		ApiMessage message = await ApiHandler.Instance.SignUp(name, password, "zxc");
		if (message.Success)
		{
			await Login(name, password);
		}
		else
		{
			GD.Print(message.ErrorMessage);
		}
		return message;
	}
}
