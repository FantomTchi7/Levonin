using Godot;
using System.Collections.Generic;

public partial class Send : PanelContainer
{
	[ExportGroup("Button background styles")]
	[Export] public StyleBox styleBoxFocus, styleBoxDisabled, styleBoxHoverPressed, styleBoxHover, styleBoxPressed, styleBoxNormal;

	[ExportGroup("Button text textures")]
	[Export] public Texture2D textureFocus, textureDisabled, textureHoverPressed, textureHover, texturePressed, textureNormal;

	private Button _button;
	private TextureRect _textureRect;

	private enum VisualState { Normal, Hover, Pressed, HoverPressed, Focus, Disabled }
	private VisualState _currentVisualState = VisualState.Normal;

	private Dictionary<VisualState, (StyleBox Style, Texture2D Texture)> _visualsMap;

	public override void _Ready()
	{
		if (GetChildCount() > 0 && GetChild(0) is Button buttonNode)
		{
			_button = buttonNode;
			if (_button.GetChildCount() > 0 && _button.GetChild(0) is TextureRect textureRectNode)
			{
				_textureRect = textureRectNode;
			}
		}
		else
		{
			SetProcess(false);
			return;
		}

		InitializeVisualsMap();
		UpdateAppearance();
	}

	private void InitializeVisualsMap()
	{
		_visualsMap = new Dictionary<VisualState, (StyleBox Style, Texture2D Texture)>
		{
			{ VisualState.Normal, (styleBoxNormal, textureNormal) },
			{ VisualState.Hover, (styleBoxHover, textureHover) },
			{ VisualState.Pressed,(styleBoxPressed, texturePressed) },
			{ VisualState.HoverPressed, (styleBoxHoverPressed ?? styleBoxPressed ?? styleBoxHover, textureHoverPressed ?? texturePressed ?? textureHover) },
			{ VisualState.Focus, (styleBoxFocus, textureFocus) },
			{ VisualState.Disabled, (styleBoxDisabled, textureDisabled) }
		};
	}

	public override void _Process(double delta)
	{
		VisualState newVisualState;

		if (_button.Disabled) newVisualState = VisualState.Disabled;
		else if (_button.ButtonPressed && _button.IsHovered()) newVisualState = VisualState.HoverPressed;
		else if (_button.ButtonPressed) newVisualState = VisualState.Normal;
		else if (_button.IsHovered()) newVisualState = VisualState.Hover;
		else if (_button.HasFocus()) newVisualState = VisualState.Focus;
		else newVisualState = VisualState.Normal;
		
		if (newVisualState != _currentVisualState)
		{
			_currentVisualState = newVisualState;
			UpdateAppearance();
		}
	}

	private void UpdateAppearance()
	{
		_visualsMap.TryGetValue(_currentVisualState, out var assets);

		AddThemeStyleboxOverride("panel", assets.Style ?? styleBoxNormal);

		if (_textureRect != null)
		{
			_textureRect.Texture = assets.Texture ?? textureNormal;
		}
	}
}
