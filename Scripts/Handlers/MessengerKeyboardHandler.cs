using Godot;
using System;

public partial class MessengerKeyboardHandler : VBoxContainer
{
	private TextEdit _textEdit;
	private Control _keyboardSpacer; // Can be an HSeparator or any Control

	[Export]
	private NodePath _textEditNodePath = "TextBoxContainer/MarginContainer/HBoxContainer/TextContainer/PanelContainer/MarginContainer/TextEdit";

	[Export]
	private NodePath _keyboardSpacerNodePath;

	private float _initialSpacerMinHeight = 0f;
	private float _screenScale = 1.0f;
	private float _scaledBottomSafeAreaInset = 0f;

	public override void _Ready()
	{
		_screenScale = DisplayServer.ScreenGetScale(); // For the main screen (screen 0)
		if (_screenScale <= 0)
		{
			GD.PrintErr("Screen scale was reported as zero or negative. Defaulting to 1.0.");
			_screenScale = 1.0f;
		}
		GD.Print($"Screen scale detected: {_screenScale}");

		// Calculate the scaled bottom safe area inset (e.g., Android navigation bar height)
		// This is done once in _Ready, assuming it doesn't change dynamically often.
		// If it can change (e.g., immersive mode toggled), this might need to be updated more frequently.
		Rect2I safeArea = DisplayServer.GetDisplaySafeArea(); // Physical pixels
		Vector2I screenSize = DisplayServer.ScreenGetSize();  // Physical pixels for the main screen
		
		// Bottom inset in physical pixels
		int physicalBottomInset = screenSize.Y - (safeArea.Position.Y + safeArea.Size.Y);
		if (physicalBottomInset < 0) physicalBottomInset = 0; // Ensure it's not negative

		_scaledBottomSafeAreaInset = physicalBottomInset / _screenScale;
		GD.Print($"Physical screen height: {screenSize.Y}, Safe area Y: {safeArea.Position.Y}, Safe area height: {safeArea.Size.Y}");
		GD.Print($"Physical bottom safe area inset (system nav bar): {physicalBottomInset}px");
		GD.Print($"Scaled bottom safe area inset: {_scaledBottomSafeAreaInset} UI units");


		_textEdit = GetNodeOrNull<TextEdit>(_textEditNodePath);
		if (_textEdit == null)
		{
			GD.PrintErr($"TextEdit node not found at path: '{_textEditNodePath}'. Please check the path and assign it in the Inspector.");
			return;
		}

		if (_keyboardSpacerNodePath == null || _keyboardSpacerNodePath.IsEmpty)
		{
			GD.PrintErr("'Keyboard Spacer Node Path' is not set in the Inspector. Please assign your HSeparator or spacer Control.");
			return;
		}

		_keyboardSpacer = GetNodeOrNull<Control>(_keyboardSpacerNodePath);
		if (_keyboardSpacer == null)
		{
			GD.PrintErr($"KeyboardSpacer node not found at path: '{_keyboardSpacerNodePath}'. Please ensure the path is correct.");
			return;
		}

		_initialSpacerMinHeight = _keyboardSpacer.CustomMinimumSize.Y;
		GD.Print($"Initial spacer min height (in Godot UI units): {_initialSpacerMinHeight}");

		_keyboardSpacer.MouseFilter = MouseFilterEnum.Ignore;

		_textEdit.FocusEntered += OnTextEditFocusEntered;
		_textEdit.FocusExited += OnTextEditFocusExited;

		if (!DisplayServer.HasFeature(DisplayServer.Feature.VirtualKeyboard))
		{
			GD.Print("Warning: This platform might not have virtual keyboard support.");
		}
	}

	private void AdjustSpacerHeight()
	{
		if (_textEdit == null || _keyboardSpacer == null || !_textEdit.HasFocus())
		{
			if (_keyboardSpacer != null && Math.Abs(_keyboardSpacer.CustomMinimumSize.Y - _initialSpacerMinHeight) > 0.01f && (_textEdit == null || !_textEdit.HasFocus()))
			{
				_keyboardSpacer.CustomMinimumSize = new Vector2(_keyboardSpacer.CustomMinimumSize.X, _initialSpacerMinHeight);
			}
			return;
		}

		int physicalKeyboardHeight = DisplayServer.VirtualKeyboardGetHeight(); // In physical pixels

		if (physicalKeyboardHeight > 0)
		{
			float scaledKeyboardHeight = physicalKeyboardHeight / _screenScale; // Convert to Godot UI units
			float halvedScaledKeyboardHeight = scaledKeyboardHeight / 2.0f;

			// Subtract the scaled bottom safe area inset from the halved keyboard height
			float targetSpacerHeight = halvedScaledKeyboardHeight - _scaledBottomSafeAreaInset;

			// Ensure the spacer height is not negative
			targetSpacerHeight = Mathf.Max(0, targetSpacerHeight);

			if (Math.Abs(_keyboardSpacer.CustomMinimumSize.Y - targetSpacerHeight) > 0.1f)
			{
				// GD.Print($"PhysicalKbdH: {physicalKeyboardHeight}, ScaledKbdH: {scaledKeyboardHeight}, HalvedKbdH: {halvedScaledKeyboardHeight}, BottomInset: {_scaledBottomSafeAreaInset}, TargetSpacerH: {targetSpacerHeight}");
				_keyboardSpacer.CustomMinimumSize = new Vector2(_keyboardSpacer.CustomMinimumSize.X, targetSpacerHeight);
			}
		}
		else
		{
			// Keyboard is not visible (or height is 0), revert to initial spacer height
			if (Math.Abs(_keyboardSpacer.CustomMinimumSize.Y - _initialSpacerMinHeight) > 0.1f)
			{
				// GD.Print($"Keyboard hidden. Reverting spacer to initial height: {_initialSpacerMinHeight}.");
				_keyboardSpacer.CustomMinimumSize = new Vector2(_keyboardSpacer.CustomMinimumSize.X, _initialSpacerMinHeight);
			}
		}
	}

	public override void _Process(double delta)
	{
		AdjustSpacerHeight();
	}

	private void OnTextEditFocusEntered()
	{
		if (_textEdit == null || _keyboardSpacer == null) return;
		GD.Print("TextEdit focus entered.");
		// Initial adjustment when focus is gained. _Process will handle continuous updates.
		AdjustSpacerHeight();
	}

	private void OnTextEditFocusExited()
	{
		if (_textEdit == null || _keyboardSpacer == null) return;
		GD.Print("TextEdit focus exited.");
		// Reset spacer size to its original minimum height when TextEdit loses focus
		_keyboardSpacer.CustomMinimumSize = new Vector2(_keyboardSpacer.CustomMinimumSize.X, _initialSpacerMinHeight);
	}
}
