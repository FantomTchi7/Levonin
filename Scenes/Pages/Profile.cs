using Godot;
using System;

public partial class Profile : PanelContainer
{
	public Button LoginButton { get; set; }
	public Button SignUpButton { get; set; }
	public override void _Ready()
	{
		EventHandler.Instance.AddListener("loggedIn", LoggedIn);
		LoginButton = GetNode<Button>("VBoxContainer/LoginButton");
		SignUpButton = GetNode<Button>("VBoxContainer/SignInButton");

		LoginButton.Pressed += () =>
		{
			Controller.Instance.CurrentPageName = "login";

		};
		SignUpButton.Pressed += () =>
		{
			Controller.Instance.CurrentPageName = "signup";
		};
	}

	private void LoggedIn(object prop)
	{
		if (prop is bool success && success)
		{
			return;
		}
	}
}
