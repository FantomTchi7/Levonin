using Godot;
using Levonin.Scripts;
using System;
using System.Collections.Generic;

public partial class Profile : PanelContainer
{
	public Button LoginButton { get; set; }
	public Button SignUpButton { get; set; }

	[ExportGroup("Buttons")]
	[Export]
	public PackedScene ButtonTemplate { get; set;}
	[Export]
	public int ButtonsStartPosition { get; set; }
	[Export]
	public string UnAuthorizedName { get; set; }

	public Label NameLabel { get; set; }

	public List<Control> AddictsButtons { get; set; } = new List<Control>();


	public override void _Ready()
	{
		EventHandler.Instance.AddListener("loggedIn", LoggedIn);
		NameLabel = GetNode<Label>("VBoxContainer/ProfileContainer/VBoxContainer/NameLabel");
		GenerateUnAuthorized();
	}
	private void GenerateUnAuthorized()
	{
		NameLabel.Text = UnAuthorizedName;
		Control login = ButtonTemplate.Instantiate<Control>();
		var loginButton = login.GetNode<Button>("Button");
		loginButton.Text = "Login";
		loginButton.Pressed += () =>
		{
			Controller.Instance.CurrentPageName = "login";

		};

		Control signup = ButtonTemplate.Instantiate<Control>();
		var signupButton =signup.GetNode<Button>("Button");
		signupButton.Text = "Sign Up";
		signupButton.Pressed += () =>
		{
			Controller.Instance.CurrentPageName = "signup";
		};

		var container = GetNode("VBoxContainer");

		AddictsButtons.AddRange([login, signup]);
		AddictsButtons.ForEach(control =>
		{
			GD.Print(control.GetNode<Button>("Button").Text);
			container.AddChild(control);
			container.MoveChild(control, ButtonsStartPosition - 1);
		});
	}


	private void GenerateAuthorizedButtons()
	{
		NameLabel.Text = Session.Instance.Name;
		Control logout = ButtonTemplate.Instantiate<Control>();
		var logoutButton = logout.GetNode<Button>("Button");
		logoutButton.Text = "Logout";
		logoutButton.Pressed += () =>
		{
			LoginHandler.Instance.Logout();
		};
		var container = GetNode("VBoxContainer");
		container.AddChild(logout);
		container.MoveChild(logout, ButtonsStartPosition);

		AddictsButtons.Add(logout);
	}

	private void ClearAddictsButtons()
	{
		var container = GetNode("VBoxContainer");
		AddictsButtons.ForEach(control =>
		{
			container.RemoveChild(control);
			control.QueueFree();
		});
		AddictsButtons.Clear();
	}
	private void LoggedIn(object prop)
	{
		if (prop is bool success)
		{
			if (success)
			{
				ClearAddictsButtons();
				GenerateAuthorizedButtons();
			}
			else
			{
				ClearAddictsButtons();
				GenerateUnAuthorized();
			}
		}
	}
}
