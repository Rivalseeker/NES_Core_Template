using Godot;

public partial class Player : CharacterBody2D
{
    [ExportCategory("Movement Physics")]
    [Export] public float Speed = 120.0f;
    [Export] public float JumpVelocity = -280.0f;
    
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    [ExportCategory("Node References")]
    [Export] public Sprite2D Sprite { get; private set; }
    [Export] public AnimationPlayer AnimPlayer { get; private set; }

    public override void _PhysicsProcess(double delta)
    {
        // The StateMachine node processes right before this and updates Velocity.
        // The Player just applies it to the physics engine.
        MoveAndSlide();
    }
}