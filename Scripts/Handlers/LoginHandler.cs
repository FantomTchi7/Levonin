using Godot;
using Levonin.Scripts;
using Levonin.Scripts.Handlers;
using Levonin.Scripts.Models;
using Levonin.Scripts.Models.API;
using Levonin.Scripts.Models.WS.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
		GD.Print($"UNLUUUCK {message.Success}");
		if (message.Success)
		{
			Session.Instance.Username = name;
			Session.Instance.LoggedIn = true;
			EventHandler.Instance.CallEvent("successNotification", "Succesfly logged in!");
			GD.Print("Login is success!");
			EventHandler.Instance.CallEvent("loggedIn", true);
			Controller.Instance.CurrentPageEnum = Levonin.Resources.Scripts.GUI.Page.Messenger;
			ApiMessage mess = await ApiHandler.Instance.GetUserId(name);
			Session.Instance.UserID = (int) mess.Response;

			InitialConnection initialConnection = new InitialConnection { name = Session.Instance.Username, token = Session.Instance.Token, userId = Session.Instance.UserID };
			string json = JsonConvert.SerializeObject(initialConnection, Formatting.Indented);
			await WebSocketHandler.Instance.Send(json);
		}
		else
		{
			GD.Print(message.ErrorMessage);
			if(!NotificationHandler.Instance.isQueueClearing) EventHandler.Instance.CallEvent("failureNotification", "Invalid datas.");
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

	public void Logout()
	{
		EventHandler.Instance.CallEvent("loggedIn", false);
		Session.Instance.Username = null;
		Session.Instance.LoggedIn = false;
	}
}
