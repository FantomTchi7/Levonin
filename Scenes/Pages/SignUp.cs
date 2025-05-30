using Godot;
using Levonin.Scripts.Handlers;
using Levonin.Scripts.Models.API;
using System;

public partial class SignUp : PanelContainer
{
	[ExportCategory("Buttons")]
	[Export]
	public Button SubmitButton { get; set; }

	[ExportCategory("Inputs")]
	[Export]
	public LineEdit UsernameInput { get; set; }
	[Export]
	public LineEdit PasswordInput { get; set; }

	public override void _Ready()
	{
		if (SubmitButton == null) GD.Print("SUBMIT BUTTON IS NULL [SIGNUP]");
		SubmitButton.Pressed += async () =>
		{
			if (UsernameInput == null || PasswordInput == null) GD.Print("INPUTS IS NULL [SIGNUP]");
			if(string.IsNullOrEmpty(UsernameInput.Text) || string.IsNullOrEmpty(PasswordInput.Text))
			{
				EventHandler.Instance.CallEvent("failureNotification", "Some inputs are empty.");
				return;
			}
			ApiMessage message = await ApiHandler.Instance.SignUp(UsernameInput.Text, PasswordInput.Text, "NULL");
			if (message.Success)
			{
				EventHandler.Instance.CallEvent("successNotification", "Succefly! Login again!");
				GD.Print("page swap");
				Controller.Instance.CurrentPageEnum = Levonin.Resources.Scripts.GUI.Page.Profile;
			}
			else
			{
				GD.Print($"{message.ErrorMessage} [SIGNUP]");
				EventHandler.Instance.CallEvent("failureNotification", "Failed.");
				return;
			}
		};
	}
}
