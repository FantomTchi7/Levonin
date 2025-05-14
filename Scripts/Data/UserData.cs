using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Levonin.Resources.Scripts.Data
{
	[GlobalClass]
	public partial class GameManager: Node
	{
		public override void _Ready()
		{
			GD.Print("ku");
		}
	}
}
