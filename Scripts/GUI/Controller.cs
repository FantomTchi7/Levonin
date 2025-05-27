using Godot;
using Levonin.Resources.Scripts.Data;
using Levonin.Resources.Scripts.GUI;
using Levonin.Resources.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

public delegate void ExportedFieldsAction(object target, PropertyInfo propertyInfo);

public partial class Controller : Control
{
	[ExportGroup("Pages")]
	[ExportSubgroup("Main")]
	[Export]
	public Button MainPageButton { get; set; }
	[Export]
	public Container MainPageContainer { get; set; }
	[ExportSubgroup("Messenger")]
	[Export]
	public Button MessengerPageButton { get; set; }
	[Export]
	public Container MessengerPageContainer { get; set; }
	[ExportSubgroup("Profile")]
	[Export]
	public Button ProfilePageButton { get; set; }
	[Export]
	public Container ProfilePageContainer { get; set; }

	[ExportGroup("Pages containers test")]
	[Export]
	public Godot.Collections.Dictionary<Page, Godot.Collections.Dictionary<ControlType, Control>> gg { get; set; }

	public List<PageDefinition> PageDefinitions { get; set; }

	private Page? _currentPage = null;
	public static Controller Instance { get; set; }
	public Page? CurrentPage
	{
		get { return this._currentPage; }
		set
		{
			if (this._currentPage != value)
			{
				if (value.HasValue) ChangePage(value.Value);
				this._currentPage = value;
			}
		}
	}
	public override void _Ready()
	{
		CheckExportedFields(this, new ExportedFieldsAction[] { CheckExportedField, InitializeButtonEvent });
		PageDefinitions = new List<PageDefinition>()
		{
			new PageDefinition(Page.Home, MainPageContainer, MainPageButton),
			new PageDefinition(Page.Messenger, MessengerPageContainer, MessengerPageButton),
			new PageDefinition(Page.Profile, ProfilePageContainer, ProfilePageButton)
		};
		if(Instance != null && Instance != this)
			{
				QueueFree();
				return;
			}
		Instance = this;
	}
	public void ChangePage(Page page)
	{
		Func<Page, PageDefinition> pageDefinition = (containerPage) => PageDefinitions.Find(pageDefinition => pageDefinition.Page == containerPage);
		if(_currentPage != null) pageDefinition(_currentPage.Value).Container.Visible = false;
		pageDefinition(page).Container.Visible = true;
	}
	private void CheckExportedFields(object targetObject, ExportedFieldsAction[] acts)
	{
		Type type = targetObject.GetType();
		PropertyInfo[] propertiesInfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		foreach (PropertyInfo propertyInfo in propertiesInfo)
		{
			foreach(ExportedFieldsAction act in acts)
			act(targetObject, propertyInfo);

		}
	}
	private void CheckExportedField(object targetObject, PropertyInfo propertyInfo)
	{
		if (propertyInfo.GetCustomAttribute<ExportAttribute>() != null)
		{
			object value = propertyInfo.GetValue(targetObject);
			if (value == null)
			{
				GD.Print("gg, wp");
			}
		}
	}
	private void InitializeButtonEvent(object targetObject, PropertyInfo propertyInfo)
	{
		if (propertyInfo.GetCustomAttribute<ExportAttribute>() != null)
		{
			object value = propertyInfo.GetValue(targetObject);
			if (value.GetType() == typeof(Button))
			{
				Button button = value as Button;
				button.Pressed += () => Button_Pressed(button);
			}
		}
	}
	private void Button_Pressed(Button button)
	{
		PageDefinition pageDefinition = PageDefinitions.Find(pageDefinition => pageDefinition.Button == button);
		CurrentPage = pageDefinition.Page;
	}
}
