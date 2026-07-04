using Godot;

public partial class ShmupShip : Area2D
{
    [Export]
    public float Speed { get; set; } = 300.0f;

    [ExportCategory("Level Scrolling")]
    [Export] 
    public float AutoScrollSpeed { get; set; } = 60.0f;

    // Adds a buffer so the sprite doesn't bleed off the edge
    [Export]
    public float Padding { get; set; } = 8.0f;

    public override void _Process(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("Right")) velocity.X += 1;
        if (Input.IsActionPressed("Left")) velocity.X -= 1;
        if (Input.IsActionPressed("Down")) velocity.Y += 1;
        if (Input.IsActionPressed("Up")) velocity.Y -= 1;

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
        }

        // Add the baseline upward movement so the ship glides with the camera
        velocity.Y -= AutoScrollSpeed;

        // Apply movement using GlobalPosition
        GlobalPosition += velocity * (float)delta;

        // Constrain the player to the camera view
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        // Get the visible rectangle in world space
        Transform2D canvasTransform = GetCanvasTransform();
        Rect2 visibleRect = canvasTransform.AffineInverse() * GetViewportRect();

        // Clamp the X and Y positions within the visible rect bounds
        float minX = visibleRect.Position.X + Padding;
        float maxX = visibleRect.Position.X + visibleRect.Size.X - Padding;
        float minY = visibleRect.Position.Y + Padding + (Padding / 2);
        float maxY = visibleRect.Position.Y + visibleRect.Size.Y;

        float clampedX = Mathf.Clamp(GlobalPosition.X, minX, maxX);
        float clampedY = Mathf.Clamp(GlobalPosition.Y, minY, maxY);

        // Apply the clamped coordinates back to the player
        GlobalPosition = new Vector2(clampedX, clampedY);
    }
}