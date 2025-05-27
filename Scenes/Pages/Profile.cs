using Godot;
using System;

public partial class Profile : PanelContainer
{
	public override void _Ready()
	{
		EventHandler.Instance.AddListener("loggedIn", LoggedIn);
	}

	private void LoggedIn(object prop)
	{
		if (prop is bool success && success)
		{
			return;
		}
	}
}
