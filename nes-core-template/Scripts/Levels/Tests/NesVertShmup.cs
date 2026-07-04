using Godot;
using System;

public partial class NesVertShmup : Node2D
{
    [Export] private Color sceneClearColor = new Color(0, 0, 0, 1);

    [ExportCategory("Level Scrolling")]
    [Export] public Camera2D SceneCamera { get; set; }
    [Export] public float ScrollSpeed { get; set; } = 60.0f; // Pixels per second

    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(sceneClearColor);
    }

    public override void _Process(double delta)
    {
        if (SceneCamera != null)
        {
            // Godot's Y axis is positive downwards, so subtract to move up
            Vector2 cameraMovement = new Vector2(0, ScrollSpeed * (float)delta);
            SceneCamera.GlobalPosition -= cameraMovement;
        }
    }

    // Function to change the active viewport's clear color
    private void ApplyClearColor(Color newColor)
    {
        // This updates the actual rendering server immediately
        RenderingServer.SetDefaultClearColor(newColor);
    }
}