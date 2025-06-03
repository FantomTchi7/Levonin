using Godot;

public partial class MessengerKeyboardHandler : Node
{
	[Export] private NodePath _hSeparatorPath = "HSeparator";
	private HSeparator _hSeparatorNode;

	[Export(PropertyHint.Range, "1.0, 30.0")] private float _animationSpeed = 15.0f;

	private float _baseSeparatorHeightUiUnits;
	private float _baseSafeAreaBottomInsetUiUnits;

	private float _projectBaseUiHeight;

	private float _currentAnimatedSeparatorHeightUiUnits;
	private float _currentTargetSeparatorHeightUiUnits;

	private bool _isMobilePlatform;

	public override void _Ready()
	{
		_isMobilePlatform = OS.HasFeature("mobile");

		if (ProjectSettings.HasSetting("display/window/size/viewport_height"))
		{
			_projectBaseUiHeight = (float)ProjectSettings.GetSetting("display/window/size/viewport_height");
		}
		else
		{
			GD.PushError("Project setting 'display/window/size/viewport_height' not found. Disabling script.");
			SetProcess(false);
			return;
		}

		if (_projectBaseUiHeight < 1.0f)
		{
			GD.PushError("Project's base viewport height is not a valid positive value. Disabling script.");
			SetProcess(false);
			return;
		}

		if (GetNodeOrNull(_hSeparatorPath) is HSeparator separator)
		{
			_hSeparatorNode = separator;
		}
		else
		{
			GD.PushError($"HSeparator node not found at path: '{_hSeparatorPath}'. Disabling script.");
			SetProcess(false);
			return;
		}

		_baseSeparatorHeightUiUnits = _hSeparatorNode.Size.Y;

		float physicalScreenHeight = DisplayServer.ScreenGetSize().Y;
		if (physicalScreenHeight < 1.0f)
		{
			GD.PushError("Could not get valid physical screen height in _Ready. Disabling script.");
			SetProcess(false);
			return;
		}

		float physicalToUiScaleFactorY = _projectBaseUiHeight / physicalScreenHeight;

		Rect2I initialSafeAreaPx = DisplayServer.GetDisplaySafeArea();
		float initialBottomInsetPx = physicalScreenHeight - (initialSafeAreaPx.Position.Y + initialSafeAreaPx.Size.Y);
		_baseSafeAreaBottomInsetUiUnits = initialBottomInsetPx * physicalToUiScaleFactorY;

		_currentAnimatedSeparatorHeightUiUnits = _baseSeparatorHeightUiUnits;
		_currentTargetSeparatorHeightUiUnits = _baseSeparatorHeightUiUnits;

		_hSeparatorNode.CustomMinimumSize = new Vector2(_hSeparatorNode.CustomMinimumSize.X, _currentAnimatedSeparatorHeightUiUnits);
	}

	public override void _Process(double delta)
	{
		if (_hSeparatorNode == null || !_hSeparatorNode.IsInsideTree())
		{
			GD.PushWarning("HSeparator node is null or not in tree. Disabling MessengerKeyboardHandler process.");
			SetProcess(false); 
			return;
		}

		float physicalScreenHeight = DisplayServer.ScreenGetSize().Y;
		if (physicalScreenHeight < 1.0f)
		{
			return;
		}

		float physicalToUiScaleFactorY = _projectBaseUiHeight / physicalScreenHeight;

		// 1. Get keyboard height information from DisplaySafeArea
		Rect2I currentSafeAreaPx = DisplayServer.GetDisplaySafeArea();
		float currentTotalBottomInsetPx = physicalScreenHeight - (currentSafeAreaPx.Position.Y + currentSafeAreaPx.Size.Y);
		float currentTotalBottomInsetUiUnits = currentTotalBottomInsetPx * physicalToUiScaleFactorY;
		float purelyKeyboardHeightFromSafeAreaUiUnits = Mathf.Max(0f, currentTotalBottomInsetUiUnits - _baseSafeAreaBottomInsetUiUnits);

		// 2. Get real-time virtual keyboard height
		float realtimeKeyboardHeightUiUnits = 0f;

		if (_isMobilePlatform)
		{
			float virtualKeyboardHeightPx = DisplayServer.VirtualKeyboardGetHeight();
			realtimeKeyboardHeightUiUnits = virtualKeyboardHeightPx * physicalToUiScaleFactorY;
		}

		// 3. Determine new target separator height
		float newTargetHeightUiUnits;
		if (realtimeKeyboardHeightUiUnits > 0.1f) 
		{
			newTargetHeightUiUnits = _baseSeparatorHeightUiUnits + purelyKeyboardHeightFromSafeAreaUiUnits;
		}
		else
		{
			newTargetHeightUiUnits = _baseSeparatorHeightUiUnits;
		}
		
		_currentTargetSeparatorHeightUiUnits = newTargetHeightUiUnits;

		// 4. Animate current height towards the target height
		if (!Mathf.IsEqualApprox(_currentAnimatedSeparatorHeightUiUnits, _currentTargetSeparatorHeightUiUnits))
		{
			float interpolationFactor = 1.0f - Mathf.Exp(-_animationSpeed * (float)delta);
			_currentAnimatedSeparatorHeightUiUnits = Mathf.Lerp(
				_currentAnimatedSeparatorHeightUiUnits,
				_currentTargetSeparatorHeightUiUnits,
				interpolationFactor
			);

			if (Mathf.Abs(_currentAnimatedSeparatorHeightUiUnits - _currentTargetSeparatorHeightUiUnits) < 0.01f)
			{
				_currentAnimatedSeparatorHeightUiUnits = _currentTargetSeparatorHeightUiUnits;
			}

			_hSeparatorNode.CustomMinimumSize = new Vector2(
				_hSeparatorNode.CustomMinimumSize.X,
				_currentAnimatedSeparatorHeightUiUnits
			);
		}
	}
}
