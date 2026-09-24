using Godot;
using System;

[Tool]
public partial class ExampleCamera : Camera2D
{
    [ExportGroup("Screen Guide Settings")]
    [Export] public bool ShowInEditor { get; set; } = true;
    [Export] public bool ShowInGame { get; set; } = false;
    
    // Automatically reads Project Settings (384x216), or can be overridden here
    [Export] public Vector2I BaseResolution { get; set; } = new Vector2I(384, 216);
    [Export] public bool UseProjectSettingsResolution { get; set; } = true;

    [ExportGroup("Visuals")]
    [Export] public Color LineColor { get; set; } = new Color(0.9f, 0.2f, 0.85f, 0.85f);
    [Export] public float LineWidth { get; set; } = 2.0f;
    [Export] public bool DrawCenterCrosshair { get; set; } = true;

    public override void _Ready()
    {
        UpdateResolutionFromProjectSettings();
    }

    public override void _Process(double delta)
    {
        bool shouldDraw = Engine.IsEditorHint() ? ShowInEditor : ShowInGame;

        if (shouldDraw)
        {
            if (UseProjectSettingsResolution)
            {
                UpdateResolutionFromProjectSettings();
            }
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        bool shouldDraw = Engine.IsEditorHint() ? ShowInEditor : ShowInGame;
        if (!shouldDraw) return;

        Vector2 targetSize = (Vector2)BaseResolution;
        Vector2 effectiveZoom = Zoom != Vector2.Zero ? Zoom : Vector2.One;
        Vector2 visibleSize = targetSize / effectiveZoom;
        Vector2 halfSize = visibleSize / 2.0f;

        // Centered screen rectangle
        Rect2 screenBounds = new Rect2(-halfSize, visibleSize);

        // Draw outer bounding line
        DrawRect(screenBounds, LineColor, filled: false, width: LineWidth);

        // Draw center crosshair target
        if (DrawCenterCrosshair)
        {
            float crosshairSize = 6.0f;
            DrawLine(new Vector2(-crosshairSize, 0), new Vector2(crosshairSize, 0), LineColor, LineWidth);
            DrawLine(new Vector2(0, -crosshairSize), new Vector2(0, crosshairSize), LineColor, LineWidth);
        }
    }

    private void UpdateResolutionFromProjectSettings()
    {
        if (ProjectSettings.HasSetting("display/window/size/viewport_width") &&
            ProjectSettings.HasSetting("display/window/size/viewport_height"))
        {
            int w = ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
            int h = ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();

            if (w > 0 && h > 0)
            {
                BaseResolution = new Vector2I(w, h);
            }
        }
    }
}