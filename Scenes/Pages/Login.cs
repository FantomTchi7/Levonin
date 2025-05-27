using Godot;
using System;

public partial class Login : PanelContainer
{
	private Button _loginButton;
	private LineEdit _usernameInput;
	private LineEdit _passwordInput;
	public override void _Ready()
	{
		GD.Print("unluck");
		string loginButtonPath ="VBoxContainer/LoginButton";
		_loginButton = GetNode<Button>(loginButtonPath);
		string usernameInputPath = "VBoxContainer/VBoxContainer/UsernameInput";
		_usernameInput = GetNode<LineEdit>(usernameInputPath);
		string passwordInputPath = "VBoxContainer/VBoxContainer2/PasswordInput";
		 _passwordInput = GetNode<LineEdit>(passwordInputPath);

		_loginButton.Pressed += _loginButton_Pressed;
	}

	private async void _loginButton_Pressed()
	{
		GD.Print("Pressed");
		GD.Print(_usernameInput.Text);
		GD.Print(_passwordInput.Text);
		if(!string.IsNullOrEmpty(_usernameInput.Text) && !string.IsNullOrEmpty(_passwordInput.Text))
		{
			await LoginHandler.Instance.Login(_usernameInput.Text, _passwordInput.Text);
		}
	}
}
