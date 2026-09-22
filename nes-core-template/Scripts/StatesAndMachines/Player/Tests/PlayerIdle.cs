using Godot;

public partial class PlayerIdle : Player_FixedMoveAndJump_State
{
    private int _physicsFrames = 0; // Tracks frames spent in the Idle state

    public override void Enter()
    {
        // GD.Print("IDLE");

        playerFMFJ.PlayAnimation("Idle");
        playerFMFJ.IsIdle = true;
        
        // Stop horizontal momentum immediately upon entering Idle
        playerFMFJ.Velocity = new Vector2(0, playerFMFJ.Velocity.Y);
        
        _physicsFrames = 0; // Reset counter every time Idle is entered
    }

    public override void Exit()
    {
        playerFMFJ.IsIdle = false;
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

        // Delay the floor check by 1 frame so MoveAndSlide() can populate IsOnFloor()
        if (_physicsFrames > 0 && !playerFMFJ.IsOnFloor())
        {
            playerFMFJ.CanCoyoteJump = true;

            EmitSignal(SignalName.Transitioned, this, "Fall");
            return;
        }
        
        // Increment the frame counter after the initial bypass check
        if (_physicsFrames == 0)
        {
            _physicsFrames++;
        }

        if (Input.IsActionJustPressed("Jump"))
        {
            EmitSignal(SignalName.Transitioned, this, "Jump");
            return;
        }

        if (Input.GetAxis("Left", "Right") != 0)
        {
            EmitSignal(SignalName.Transitioned, this, "GroundMove");
        }
    }
}