using Godot;

public partial class PlayerIdle : Player_FixedMoveAndJump_State
{
    public override void Enter()
    {
        playerFMFJ.PlayAnimation("Idle");
        playerFMFJ.IsIdle = true;
        
        // Stop horizontal momentum immediately upon entering Idle
        playerFMFJ.Velocity = new Vector2(0, playerFMFJ.Velocity.Y);
    }

    public override void Exit()
    {
        playerFMFJ.IsIdle = false;
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

        if (!playerFMFJ.IsOnFloor())
        {
            EmitSignal(SignalName.Transitioned, this, "Fall");
            return;
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






// using Godot;

// public partial class PlayerIdle : Player_FixedMoveAndJump_State
// {
//     public override void Enter()
//     {
//         playerFMFJ.PlayAnimation("Idle");

//         playerFMFJ.IsIdle = true;

//         playerFMFJ.Velocity = new Vector2(0, playerFMFJ.Velocity.Y);
//     }

//     public override void Exit()
//     {
//         playerFMFJ.IsIdle = false;
//     }

//     public override void PhysicsUpdate(double delta)
//     {
//         if (playerFMFJ == null) return;

//         if (!playerFMFJ.IsOnFloor())
//         {
//             EmitSignal(SignalName.Transitioned, this, "Fall");
//             // Player.CurrentJumps++;
//             return;
//         }

//         if (Input.IsActionJustPressed("Jump"))
//         {
//             EmitSignal(SignalName.Transitioned, this, "Jump");
//         }

//         float direction = Input.GetAxis("Left", "Right");
//         if (direction != 0)
//         {
//             EmitSignal(SignalName.Transitioned, this, "GroundMove");
//         }

//         playerFMFJ.UpdateFacingDirection(direction);
//         playerFMFJ.Velocity = new Vector2(direction * playerFMFJ.moveSpeed, playerFMFJ.Velocity.Y);
//     }
// }
