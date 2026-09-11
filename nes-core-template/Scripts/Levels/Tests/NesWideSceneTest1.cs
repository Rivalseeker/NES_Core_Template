using Godot;
using System;

public partial class NesWideSceneTest1 : NesWideSceneBase
{
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        // Listen for the specific input action
        if (@event.IsActionPressed("DebugPixelSnap"))
        {
            // Get the current active viewport
            Viewport viewport = GetViewport();
            
            // Toggle the boolean value
            viewport.Snap2DTransformsToPixel = !viewport.Snap2DTransformsToPixel;
            
            // Print to the output console so you know it registered
            GD.Print($"[Debug] Snap 2D Transforms to Pixel is now: {viewport.Snap2DTransformsToPixel}");
        }
    }
}
