using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class EventHandler: Node
{
	public static EventHandler Instance { get; private set; }
	private Dictionary<string, List<Action<object>>> Events { get; set; } = new();
	public override void _Ready()
	{
		Events = new Dictionary<string, List<Action<object>>>();
		GD.Print("piiiittt");

		if(Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
	}

	public void AddListener(string eventName, Action<object> action)
	{
		if (!Events.ContainsKey(eventName))
			Events[eventName] = new List<Action<object>>();
	
		Events[eventName].Add(action);
	}

	public void CallEvent(string eventName, object param)
	{
		if (Events.ContainsKey(eventName)) Events[eventName].ForEach(action => { 
			action(param);
		});
	}
}
