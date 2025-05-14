using Godot;
using System;
using Levonin.Resources.Scripts.Data;

public partial class GameManager : Node
{
		public static UserDataResource UserData { get; set; }
		public static readonly string ResourcePath = "res://Scripts/Data/user_data.tres";
		public override void _Ready()
		{
			GD.Print("null123");
			UserData = ResourceLoader.Load<UserDataResource>(ResourcePath);
			GD.Print(UserData.IsRegistered);
			if(UserData == null)
			{
				
				UserData = new UserDataResource();
				UserData.IsRegistered = false;
				UserData.Token = null;
				UserData.UserName = null;

				ResourceSaver.Save(UserData, ResourcePath);
			}
		}
}
