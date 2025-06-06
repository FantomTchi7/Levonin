using Godot;
using Levonin.Resources.Scripts.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Resources.Scripts.Models
{
	public class PageDefinition
	{
		[Export]
		public Page Page { get; set; }
		[Export]
		public Container Container { get; set; }
		[Export]
		public Button Button { get; set; }
		[Export]
		public string Name { get; set; }
		public PageDefinition(Page page, Container container, Button button, string name)
		{
			Page = page;
			Container = container;
			Button = button;
			Name = name;
		}
	}
}
