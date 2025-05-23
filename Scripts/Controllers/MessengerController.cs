using Godot;
using System;
using Levonin.Resources.Scripts.Models;
using System.Collections.Generic;

public partial class MessengerController : PanelContainer
{
	[Export]
	public PackedScene PMTemplate { get; set; }

	[ExportGroup("Statuses")]
	[Export]
	public StyleBoxFlat OfflineStyleBox { get; set; }
	[Export]
	public StyleBoxFlat OnlineStyleBox { get; set; }
	
	public List<PMTemplate> Templates { get; set; }
	public VBoxContainer PMContainer { get; set; }
	
	public override void _Ready()
	{
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
		foreach (PMTemplate template in Templates)
		{
			if (PMTemplate != null)
			{
				Node componentInstance = PMTemplate.Instantiate();

				MarginContainer marginContainer = componentInstance.GetNode<MarginContainer>("MarginContainer");
				HBoxContainer hboxContainer = marginContainer.GetNode<HBoxContainer>("HBoxContainer");
				AspectRatioContainer aspectRatioContainer = hboxContainer.GetNode<AspectRatioContainer>("AspectRatioContainer");
				MarginContainer infoContainer = hboxContainer.GetNode<MarginContainer>("InfoContainer");
				infoContainer.GetNode<Label>("Name").Text = template.Name;
				infoContainer.GetNode<Label>("Status").Text = template.Status.ToString();

				PanelContainer panelContainer = aspectRatioContainer.GetNode<PanelContainer>("UserImagePanel");
				Panel statusPanel = panelContainer.GetNode<Panel>("StatusPanel");
				StyleBoxFlat styleBoxFlat = (StyleBoxFlat)statusPanel.GetThemeStylebox("panel");
				if (styleBoxFlat != null)
				{
					switch (template.Status)
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
}
