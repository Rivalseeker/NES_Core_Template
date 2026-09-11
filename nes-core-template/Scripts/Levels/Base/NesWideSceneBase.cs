using Godot;
using System;

public partial class NesWideSceneBase : Node2D
{
    [Export] private Color sceneClearColor = new Color(0, 0, 0, 1);

    // References to our nodes
    [ExportCategory("Node References")]
    [Export] private Camera2D sceneCamera;
    [Export] private Sprite2D sceneCameraBorder;

    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(sceneClearColor);

        // Turn on Scene Camera Border
        sceneCamera.Visible = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Ensure both nodes exist to prevent null reference errors
        if (sceneCamera != null && sceneCameraBorder != null)
        {
            Vector2 currentZoom = sceneCamera.Zoom;

            // Prevent division by zero
            if (currentZoom.X != 0 && currentZoom.Y != 0)
            {
                // Inverse relationship: as Zoom decreases, Scale increases (and vice versa)
                sceneCameraBorder.Scale = new Vector2(
                    1.0f / currentZoom.X,
                    1.0f / currentZoom.Y
                );
            }
        }
    }

    // Function to change the active viewport's clear color
    private void ApplyClearColor(Color newColor)
    {
        // This updates the actual rendering server immediately
        RenderingServer.SetDefaultClearColor(newColor);
    }
}