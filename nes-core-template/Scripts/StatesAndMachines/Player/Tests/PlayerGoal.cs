using Godot;

public partial class PlayerGoal : Player_FixedMoveAndJump_State
{
    public override void Enter()
    {
        if (playerFMFJ == null) return;

        // Zero velocity immediately upon reaching the goal
        playerFMFJ.Velocity = Vector2.Zero;
        playerFMFJ.PlayAnimation("Idle");
        // playerFMFJ.PlayGoalAnimation();
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

        // Lock horizontal and vertical velocity to prevent any movement
        playerFMFJ.Velocity = Vector2.Zero;
    }
}