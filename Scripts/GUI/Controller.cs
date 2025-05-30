using Godot;
using Levonin.Resources.Scripts.Data;
using Levonin.Resources.Scripts.GUI;
using Levonin.Resources.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

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

	[ExportGroup("PagesContainersTest")]
	[Export]
	public Godot.Collections.Dictionary<string, NodePath> MultipleNodePaths { get; set; }

	public List<PageDefinition> PageDefinitions { get; set; }

	private PageDefinition? _currentPage = null;
	public static Controller Instance { get; set; }
	public PageDefinition? CurrentPage
	{
		get { return this._currentPage; }
		set
		{
			if (this._currentPage != value)
			{
				if (value != null) ChangePage(value);
				this._currentPage = value;
			}
		}
	}
	public Page? CurrentPageEnum
	{
		get { return this._currentPage.Page; }
		set
		{
			if (this._currentPage.Page != value)
			{
				if (value != null) ChangePage(value.Value);
				this._currentPage = PageDefinitions.Find(defin => defin.Page == value);
			}
		}
	}
	public string? CurrentPageName
	{
		get
		{
			return this._currentPage?.Name;
		}
		set
		{
			if (this._currentPage.Name != value)
			{
				GD.Print("gg wp wwp");
				if (value != null) ChangePage(value);
				this._currentPage = PageDefinitions.Find(defin => defin.Name == value);
			}
		}
	}
	public override void _Ready()
	{

		if (Instance != null && Instance != this)
			{
				QueueFree();
				return;
			}
		Instance = this;
		CheckExportedFields(this, new ExportedFieldsAction[] { CheckExportedField, InitializeButtonEvent });
		PageDefinitions = new List<PageDefinition>()
		{
			new PageDefinition(Page.Home, MainPageContainer, MainPageButton, "home"),
			new PageDefinition(Page.Messenger, MessengerPageContainer, MessengerPageButton, "messenger"),
			new PageDefinition(Page.Profile, ProfilePageContainer, ProfilePageButton, "profile")
		};
		int index = -1;
		foreach (var (name, containerPath) in MultipleNodePaths)
		{
			index++;
			PageDefinitions.Add(new PageDefinition(Page.Foreign, GetNode<Container>(containerPath), null, name));

		}

		EventHandler.Instance.AddListener("changedPage", (param) =>
		{
			if(param is PageDefinition pageDef)
			{
				GD.Print($"Page changed -> {pageDef.Name}");
			}
		});
	}
	private void ChangePage(Page page)
	{
		GD.Print("Page changing...");
		if(CurrentPage != null) GD.Print($"Previous page -> {CurrentPage.Name}");
		Func<Page, PageDefinition> pageDefinition = (containerPage) => PageDefinitions.Find(pageDefinition => pageDefinition.Page == containerPage);
		if(CurrentPage != null) _currentPage.Container.Visible = false;
		pageDefinition(page).Container.Visible = true;

		EventHandler.Instance.CallEvent("changedPage", pageDefinition);
	}
	private void ChangePage(PageDefinition page)
	{
		GD.Print("Page changing...");
		if (CurrentPage != null) GD.Print($"Previous page -> {CurrentPage.Name}");
		if (_currentPage != null) _currentPage.Container.Visible = false;
		page.Container.Visible = true;

		EventHandler.Instance.CallEvent("changedPage", page);
	}
	private void ChangePage(string name)
	{
		GD.Print("Page changing...");
		if (CurrentPage != null) GD.Print($"Previous page -> {CurrentPage.Name}");
		PageDefinition? pageDefinition = PageDefinitions.Find(pageDefinition => {
			GD.Print($"{pageDefinition.Name} == (STR) {name} ");
		
			return pageDefinition.Name == name;
			});
		if (pageDefinition != null)
		{
			if (_currentPage != null) _currentPage.Container.Visible = false;
			GD.Print($"container is null {pageDefinition.Container == null}");
			pageDefinition.Container.Visible = true;
		}
		else
		{
			GD.PrintErr($"Page with name {name} not found!");
		}

		EventHandler.Instance.CallEvent("changedPage", pageDefinition);
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
				GetTree().Quit();
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
		CurrentPage = pageDefinition;
	}
}
