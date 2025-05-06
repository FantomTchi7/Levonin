using Godot;
using System;
using Levonin.Resources.Scripts.Models;
using System.Collections.Generic;

public partial class MessengerController : PanelContainer
{
	[Export]
	public PackedScene PMTemplate { get; set; }
	public List<PMTemplate> Templates { get; set; }
	public VBoxContainer PMContainer { get; set; }
	
	public MessengerController()
	{
		GD.Print("init");
	}
	
	public override void _Ready()
	{
		GD.Print("gg wp");
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
		RenderComponents();

	}

	public void RenderComponents()
	{
		GD.Print("gg wp");
		foreach (PMTemplate template in Templates)
		{
			
			if (PMTemplate != null)
			{
				GD.Print("not null");
				var componentInstance = PMTemplate.Instantiate();
				
				
				GD.Print("is control");
				MarginContainer container = componentInstance.GetNode<MarginContainer>("MarginContainer");
				container.GetNode<Label>("Name").Text = template.Name;
				container.GetNode<Label>("Status").Text = template.Status.ToString();
				GD.Print(template.Status.ToString());
				PMContainer.AddChild(componentInstance);
				
			}
		}
	}
}
