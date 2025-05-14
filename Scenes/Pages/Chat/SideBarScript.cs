using Godot;

using System;

using System.Threading.Tasks;


public partial class SideBarScript : PanelContainer

{

	[Export]

	public float SlideDistancePx { get; set; }


	public SlideSide CurrentSide = SlideSide.Right;

	public Vector2 PreviousWindowSize;

	public override void _Ready()

	{

		Vector2 windowSize = DisplayServer.WindowGetSize();

		PreviousWindowSize = windowSize;


		this.Size = new Vector2(300, Size.Y);

		this.Position = new Vector2(windowSize.X, 0);




		Window mainWindow = GetViewport().GetWindow();

		if (mainWindow != null)

		{

			mainWindow.SizeChanged += MainWindow_SizeChanged;

		}


		DelayMethod();


	}


	public async Task Slide(SlideSide side)

	{

		switch (side)

		{

			case SlideSide.Left:

				while (Position.X >= DisplayServer.WindowGetSize().X - Size.X)

				{

					GD.Print($"Position.X {Position.X}, Window.X {DisplayServer.WindowGetSize().X - Size.X}");

					await Task.Delay(TimeSpan.FromMilliseconds(10));



					Position = new Vector2(Position.X - SlideDistancePx, 0);

				}

				break;

			case SlideSide.Right:

				while (Position.X <= DisplayServer.WindowGetSize().X)

				{

					GD.Print($"Position.X {Position.X}, Window.X {DisplayServer.WindowGetSize().X - Size.X}");

					await Task.Delay(TimeSpan.FromMilliseconds(10));

					GD.Print(Position.X, Position.Y);

					Position = new Vector2(Position.X + SlideDistancePx, 0);

				}

				break;

		}


	}


	private async void DelayMethod()

	{

		while (true)

		{

			await Task.Delay(TimeSpan.FromMilliseconds(3000));


			if (CurrentSide == SlideSide.Right) CurrentSide = SlideSide.Left;

			else CurrentSide = SlideSide.Right;

			await Slide(CurrentSide);


		}


	}


	private void MainWindow_SizeChanged()

	{

		Vector2 windowSize = DisplayServer.WindowGetSize();


		this.Position = new Vector2(Position.X + (PreviousWindowSize.X - windowSize.X) * -1, Position.Y + (PreviousWindowSize.Y - windowSize.Y) * -1);

		this.Size = new Vector2(Size.X + (PreviousWindowSize.X - windowSize.X) * -1, Size.Y + (PreviousWindowSize.Y - windowSize.Y) * -1);

		PreviousWindowSize = windowSize;

	}

}


public enum SlideSide

{

	Right, Left

}
