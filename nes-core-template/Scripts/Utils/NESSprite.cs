using Godot;

[GlobalClass] // <-- This registers it as an official Godot Node type!
public partial class NESSprite : Sprite2D
{
    public override void _Ready()
    {
        // Automatically load and apply the nes_clip shader if no material is set
        if (Material == null)
        {
            Shader nesShader = GD.Load<Shader>("res://Scripts/Shaders/nes_clip_and_colors.gdshader");
            ShaderMaterial mat = new ShaderMaterial { Shader = nesShader };
            Material = mat;
        }

        NESSpriteManager.Instance?.RegisterSprite(this);
    }

    public override void _ExitTree()
    {
        NESSpriteManager.Instance?.UnregisterSprite(this);
    }
}






// using Godot;
// [GlobalClass] // <-- This registers it as an official Godot Node type!

// public partial class NESSprite : Sprite2D
// {
//     public override void _EnterTree()
//     {
//         // Register this sprite with the manager as soon as it enters the scene tree
//         NESSpriteManager.Instance?.RegisterSprite(this);
//     }

//     public override void _ExitTree()
//     {
//         // Unregister to prevent memory leaks or null references when destroyed
//         NESSpriteManager.Instance?.UnregisterSprite(this);
//     }
// }