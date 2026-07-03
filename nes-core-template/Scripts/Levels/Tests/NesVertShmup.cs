using Godot;
using System;

public partial class NesVertShmup : Node2D
{
    [Export] private Color sceneClearColor = new Color(0, 0, 0, 1);




    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(sceneClearColor);
    }




    // Function to change the active viewport's clear color
    private void ApplyClearColor(Color newColor)
    {
        // This updates the actual rendering server immediately
        RenderingServer.SetDefaultClearColor(newColor);
        
        // Optional: If you need to read from ProjectSettings later in the session, 
        // you can also update the setting itself, though it's not required for the visual change.
        // ProjectSettings.SetSetting("rendering/environment/defaults/default_clear_color", newColor);
    }
}
