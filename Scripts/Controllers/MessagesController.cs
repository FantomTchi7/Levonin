using Godot;
using Levonin.Resources.Scripts.Models;
using Levonin.Scripts;
using Levonin.Scripts.Handlers;
using Levonin.Scripts.Models.API;
using Levonin.Scripts.Models.WS.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MessagesController : PanelContainer
{
	[ExportCategory("Messages Templates")]
	[Export]
	public PackedScene LeftToRight { get; set; }
	[Export]
	public PackedScene RightToLeft { get; set; }
	[Export]
	public string PageName { get; set; }

	public Button SendMessageButton { get; set; }
	public TextEdit MessageInput { get; set; }

	public VBoxContainer MessagesContainer { get; set; }

	public override void _Ready()
	{
		EventHandler.Instance.AddListener("messagesPageRender", Render);

		MessagesContainer = GetNode<VBoxContainer>("VBoxContainer/MessagesContainer/VBoxContainer");
		SendMessageButton = GetNode<PanelContainer>("VBoxContainer/TextBoxContainer/MarginContainer/HBoxContainer/SendButtonContainer/AspectRatioContainer/SendButton").GetNode<Button>("Button");
		MessageInput = GetNode<TextEdit>("VBoxContainer/TextBoxContainer/MarginContainer/HBoxContainer/TextContainer/PanelContainer/MarginContainer/TextEdit");
		SendMessageButton.Pressed += SendMessageButton_Pressed;
		EventHandler.Instance.AddListener("reRenderChannel", ReRender);
				

	}

	private async void ReRender(object param)
	{
		ApiMessage messages = await ApiHandler.Instance.GetMessages(Session.Instance.CurrentChannel.ChatID);
		Render(messages);
	}

	private async void SendMessageButton_Pressed()
	{
		if (!String.IsNullOrEmpty(MessageInput.Text))
		{
			GD.Print("not null");
			await ApiHandler.Instance.SendMessage(MessageInput.Text, Session.Instance.CurrentChannel.ChatID);
			GD.Print("sended");
			AddMessage(Session.Instance.Username, MessageInput.Text);
			MessageInput.Text = "";

			MessageInChannel message = new MessageInChannel { channel = Session.Instance.CurrentChannel.ChatID };
			string json = JsonConvert.SerializeObject(message);
			await WebSocketHandler.Instance.Send(json);
		}

	}

	private void AddMessage(string username, string content)
	{
		if(username != Session.Instance.Username)
		{
			HBoxContainer messageNode = LeftToRight.Instantiate<HBoxContainer>();
			FillData(messageNode, username, content);
			MessagesContainer.AddChild(messageNode);
		}
		else
		{
			HBoxContainer messageNode = RightToLeft.Instantiate<HBoxContainer>();
			FillData(messageNode, username, content);
			MessagesContainer.AddChild(messageNode);
		}
	}

	private void Render(object param)
	{
		GD.Print("PISKA BOBRA");
		GD.Print(param.GetType());
		ClearContainer();
		

		if(param is ApiMessage apiMessage && apiMessage.Response is List<Message> messages)
		{
			messages.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
			VBoxContainer pmTemplate = GetNode<VBoxContainer>("VBoxContainer/PanelContainer/PmTemplate");
			pmTemplate.GetNode<Label>("MarginContainer/HBoxContainer/InfoContainer/Name").Text = Session.Instance.CurrentChannel.ChatName;
			pmTemplate.GetNode<Label>("MarginContainer/HBoxContainer/InfoContainer/Status").Text = Session.Instance.CurrentChannel.Status;
			GD.Print("countik kk --- " + messages.Count);
			foreach(var message in messages)
			{
				GD.Print(message.Content);
				if(message.Username != Session.Instance.Username)
				{
					HBoxContainer messageNode = LeftToRight.Instantiate<HBoxContainer>();
					FillData(messageNode, message);
					MessagesContainer.AddChild(messageNode);
				}
				else
				{
					HBoxContainer messageNode = RightToLeft.Instantiate<HBoxContainer>();
					FillData(messageNode, message);
					MessagesContainer.AddChild(messageNode);
				}
				
			}
			ScrollDown();
		}
	}
	private async void ScrollDown()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		var scrollBar = GetNode<ScrollContainer>("VBoxContainer/MessagesContainer").GetVScrollBar();
		scrollBar.Value = scrollBar.MaxValue; 
	}
	private void FillData(HBoxContainer node, Message data)
	{
		Label username = node.GetNode<Label>("VBoxContainer/MarginContainer/Label");
		username.Text = data.Username;
		RichTextLabel content = node.GetNode<RichTextLabel>("VBoxContainer/TextContainer/PanelContainer/MarginContainer/RichTextLabel");
		content.Text = data.Content;
	}

	private void FillData(HBoxContainer node, string name, string textContent)
	{
		Label username = node.GetNode<Label>("VBoxContainer/MarginContainer/Label");
		username.Text = name;
		RichTextLabel content = node.GetNode<RichTextLabel>("VBoxContainer/TextContainer/PanelContainer/MarginContainer/RichTextLabel");
		content.Text = textContent;
	}

	private void ClearContainer()
	{
		foreach (var node in MessagesContainer.GetChildren())
		{
			if (node is Node) node.QueueFree();
		}
	}
}
public class PMRender
{
	public int ChatId { get; set; }
}
