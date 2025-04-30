using Godot;
using System;

public partial class Maxwell : Node3D
{
	private float RotationSpeedRadians = Mathf.DegToRad(200f);
	
	public override void _Process(double delta)
	{
		float radiansToRotate = -RotationSpeedRadians * (float)delta;

		RotateY(radiansToRotate);
	}
}
