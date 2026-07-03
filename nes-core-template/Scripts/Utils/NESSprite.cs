using Godot;

public partial class NESSprite : Sprite2D
{
    public override void _EnterTree()
    {
        // Register this sprite with the manager as soon as it enters the scene tree
        NESSpriteManager.Instance?.RegisterSprite(this);
    }

    public override void _ExitTree()
    {
        // Unregister to prevent memory leaks or null references when destroyed
        NESSpriteManager.Instance?.UnregisterSprite(this);
    }
}