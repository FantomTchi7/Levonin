using Godot;
using System;
using Levonin.Resources.Scripts.Models;
using System.Collections.Generic;
using Levonin.Scripts.Models;
using Levonin.Scripts.Handlers;
using Levonin.Scripts.Models.API;

public partial class MessengerController : PanelContainer
{
	[Export]
	public PackedScene PMTemplate { get; set; }

	[ExportGroup("Statuses")]
	[Export]
	public StyleBoxFlat OfflineStyleBox { get; set; }
	[Export]
	public StyleBoxFlat OnlineStyleBox { get; set; }
	[Export]
	[ExportGroup("Other")]
	public PackedScene LoginButtonTemplate { get; set; }

	public Control LoginButton { get; set; }
	
	public List<PMTemplate> Templates { get; set; }
	public VBoxContainer PMContainer { get; set; }

	public List<ChannelModel> ChannelModels { get; set; } = new List<ChannelModel>();
	
	public override void _Ready()
	{
		LoginButton = LoginButtonTemplate.Instantiate<Control>();
		LoginButton.GetNode<Button>("MarginContainer/Button").Pressed += () =>
		{
			Controller.Instance.CurrentPageName = "login";
		};
		LoginButton.Name = "LoginButton";
		ToggleLoginButton();

		EventHandler.Instance.AddListener("loggedIn", async param => {
			if(param is bool success)
				if (success)
				{ 
					ToggleLoginButton();
					ApiMessage mess = await ApiHandler.Instance.GetChannels();
					if(mess.Response is List<Levonin.Scripts.Models.Channel> channels)
					{
						RenderComponents(channels);
					}
				}
				else
				{
					ClearContainer();
					ToggleLoginButton();
				}
					
		});

		foreach(Node child in GetChildren())
		{
			if (child is VBoxContainer container)
			{
				PMContainer = container;
			}
		}

		Templates = new List<PMTemplate>()
		{
			new PMTemplate("Dain", Status.Offline), new PMTemplate("David", Status.Online), new PMTemplate("Lennuk", Status.Online)
		};

	}
	private void ToggleLoginButton()
	{
		if(HasNode("LoginButton")) RemoveChild(LoginButton);
		else AddChild(LoginButton);
	}

	private void ClearContainer()
	{
		foreach (var node in PMContainer.GetChildren())
		{
			if (node is Node) node.QueueFree();
		}
	}

	public void RenderComponents(List<Channel> channels)
	{
		ClearContainer();

		foreach (Channel channel in channels)
		{
			if (PMTemplate != null)
			{
				ChatType chatType = GetChatType(channel.ChatType).Value;
				Status? status = null;
				if(chatType != ChatType.Group && channel.Status != null) status = GetStatus(channel.Status).Value;
				Node componentInstance = PMTemplate.Instantiate();
				ChannelModels.Add(new ChannelModel() { ChatID = channel.ChatID, ChatName = channel.ChatName, ChatType = chatType, ImageUrl = channel.ImageUrl, Node = componentInstance, Status = channel.Status, StatusType = status, UserID = channel.UserID  });
				MarginContainer marginContainer = componentInstance.GetNode<MarginContainer>("MarginContainer");
				HBoxContainer hboxContainer = marginContainer.GetNode<HBoxContainer>("HBoxContainer"); 
				AspectRatioContainer aspectRatioContainer = hboxContainer.GetNode<AspectRatioContainer>("AspectRatioContainer"); 
				MarginContainer infoContainer = hboxContainer.GetNode<MarginContainer>("InfoContainer"); 
				infoContainer.GetNode<Label>("Name").Text = channel.ChatName; 
				if (chatType != ChatType.Group) infoContainer.GetNode<Label>("Status").Text = status.ToString(); 

				PanelContainer panelContainer = aspectRatioContainer.GetNode<PanelContainer>("UserImagePanel"); 
				Panel statusPanel = panelContainer.GetNode<Panel>("StatusPanel"); 
				StyleBoxFlat styleBoxFlat = (StyleBoxFlat)statusPanel.GetThemeStylebox("panel");

				if (styleBoxFlat != null && chatType != ChatType.Group)
				{
					switch (status)
					{
						case Status.Online:
							statusPanel.AddThemeStyleboxOverride("panel", OnlineStyleBox);
							break;
						case Status.Offline:
							statusPanel.AddThemeStyleboxOverride("panel", OfflineStyleBox);
							break;
					}
				}
				PMContainer.AddChild(componentInstance);
			}
		}
	}

	private Status? GetStatus(string name)
	{
		switch (name)
		{
			case "online": return Status.Online;
			case "offline": return Status.Offline;
		}
		return null;
	}
	private ChatType? GetChatType(string type)
	{
		switch (type)
		{
			case "group": return ChatType.Group;
			case "PM": return ChatType.PM;
		}
		return null;
	}
}
public enum ChatType
{
	Group, PM
}

public class ChannelModel: Channel
{
	public Node Node { get; set; }
	private Status? _statusType;
	public Status? StatusType
	{ 
		get
		{
			return _statusType;
		}
		set
		{
			if(value != _statusType)
			{
				_statusType = value;
			}
			
		}
	}
	public ChatType ChatType { get; set; }

}
