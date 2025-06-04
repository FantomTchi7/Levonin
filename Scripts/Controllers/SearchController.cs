using Godot;
using Levonin.Scripts.Handlers;
using Levonin.Scripts.Models.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

public partial class SearchController : PanelContainer
{
	[ExportCategory("Other")]
	[Export]
	public PackedScene UserTemplate { get; set; }
	public TextEdit UsernameInput { get; set; }
	public Button SubmitButton { get; set; }
	public PanelContainer Container { get; set; }
	public override void _Ready()
	{
		UsernameInput = GetNode<TextEdit>("VBoxContainer/TextBoxContainer/MarginContainer/HBoxContainer/TextContainer/PanelContainer/MarginContainer/TextEdit");
		SubmitButton = GetNode<PanelContainer>("VBoxContainer/TextBoxContainer/MarginContainer/HBoxContainer/SendButtonContainer/AspectRatioContainer/SendButton").GetNode<Button>("Button");

		SubmitButton.Pressed += SubmitButton_Pressed;
		Container = GetNode<PanelContainer>("VBoxContainer/PanelContainer");
	}

	private void SubmitButton_Pressed()
	{
		Render();
	}

	private async Task Render()
	{
		if (!String.IsNullOrEmpty(UsernameInput.Text))
		{
			ApiMessage message = await ApiHandler.Instance.GetUsers(UsernameInput.Text);
			if(message.Response is List<ApiUser> users)
			{
				foreach(ApiUser user in users)
				{
					PanelContainer userContainer = UserTemplate.Instantiate<PanelContainer>();
					userContainer.GetNode("HBoxContainer/PmTemplate").GetNode<Label>("MarginContainer/HBoxContainer/InfoContainer/Name").Text = user.Username;
					userContainer.GetNode("HBoxContainer/PmTemplate").GetNode<Label>("MarginContainer/HBoxContainer/InfoContainer/Status").Text = "";
					Button button = userContainer.GetNode<Button>("HBoxContainer/MarginContainer/AspectRatioContainer/Button");

					Container.AddChild(userContainer);
					button.Pressed += async () =>
					{
						int UserID = user.UserID;
						await ApiHandler.Instance.CreateChannel(UserID);
						EventHandler.Instance.CallEvent("reRenderChannels", null);
						string json = JsonConvert.SerializeObject(new ReRenderChannels { userId = UserID });
						await WebSocketHandler.Instance.Send(json);
					};

				}
			}
		}
	}
}
public class ReRenderChannels
{
	public string header { get; set; } = "ReRenderChannels";
	public int userId { get; set; }
}
