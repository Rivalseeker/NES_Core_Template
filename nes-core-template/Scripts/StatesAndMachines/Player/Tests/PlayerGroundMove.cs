using Godot;

public partial class PlayerGroundMove : Player_FixedMoveAndJump_State
{
    public override void Enter()
    {
        playerFMFJ.PlayAnimation("GroundMove");
        playerFMFJ.IsGroundMove = true;
    }

    public override void Exit()
    {
        playerFMFJ.IsGroundMove = false;
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

        if (!playerFMFJ.IsOnFloor())
        {
            playerFMFJ.CanCoyoteJump = true;

            EmitSignal(SignalName.Transitioned, this, "Fall");
            return;
        }

        if (Input.IsActionJustPressed("Jump"))
        {
            EmitSignal(SignalName.Transitioned, this, "Jump");
            return;
        }

        float direction = Input.GetAxis("Left", "Right");
        if (direction == 0)
        {
            EmitSignal(SignalName.Transitioned, this, "Idle");
            return;
        }

        // FIXED: Applies actual horizontal velocity during GroundMove
        playerFMFJ.UpdateFacingDirection(direction);
        playerFMFJ.Velocity = new Vector2(direction * playerFMFJ.moveSpeed, playerFMFJ.Velocity.Y);
    }
}






// using Godot;
// using System;

// public partial class PlayerGroundMove : Player_FixedMoveAndJump_State
// {
//     public override void Enter()
//     {
//         playerFMFJ.PlayAnimation("GroundMove");

//         playerFMFJ.IsGroundMove = true;
//     }

//     public override void Exit()
//     {
//         playerFMFJ.IsGroundMove = false;
//     }

//     public override void PhysicsUpdate(double delta)
//     {
//         if (playerFMFJ == null) return;

//         // float inputDir = Input.GetAxis("Left", "Right");

//         // playerFMFJ.

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

//         if (direction == 0)
//         {
//             EmitSignal(SignalName.Transitioned, this, "Idle");
//             return;
//         }
//     }
// }
