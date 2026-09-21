using Godot;

public partial class PlayerJump : Player_FixedMoveAndJump_State
{
    public override void Enter()
    {
        playerFMFJ.PlayAnimation("Jump");
        playerFMFJ.Velocity = new Vector2(playerFMFJ.Velocity.X, playerFMFJ.JumpVelocity);
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

        if (playerFMFJ.Velocity.Y > 0)
        {
            EmitSignal(SignalName.Transitioned, this, "Fall");
            return;
        }

        if (playerFMFJ.IsOnCeiling())
        {
            // FIXED: Zero out upward momentum, preserve horizontal momentum
            playerFMFJ.Velocity = new Vector2(playerFMFJ.Velocity.X, 0); 
            EmitSignal(SignalName.Transitioned, this, "Fall");
            return;
        }

        // ADDED: Air Control
        float direction = Input.GetAxis("Left", "Right");
        // if (direction != 0)
        // {
        //     playerFMFJ.UpdateFacingDirection(direction);
        // }
        // playerFMFJ.Velocity = new Vector2(direction * playerFMFJ.moveSpeed, playerFMFJ.Velocity.Y);

        if (playerFMFJ.IsOnFloor())
        {
            EmitSignal(SignalName.Transitioned, this, direction == 0 ? "Idle" : "GroundMove");
        }
    }
}






// using Godot;
// using System;

// public partial class PlayerJump : Player_FixedMoveAndJump_State
// {
//     public override void Enter()
//     {
//         playerFMFJ.PlayAnimation("Jump");
//         playerFMFJ.Velocity = new Vector2(playerFMFJ.Velocity.X, playerFMFJ.JumpVelocity);
//     }

//     public override void PhysicsUpdate(double delta)
//     {
//         if (playerFMFJ == null) return;

//         Vector2 vel = playerFMFJ.Velocity;
//         vel.Y += playerFMFJ.JumpGravity * (float)delta;
//         playerFMFJ.Velocity = vel;

//         playerFMFJ.MoveAndSlide();

//         if (playerFMFJ.Velocity.Y > 0)
//         {
//             EmitSignal(SignalName.Transitioned, this, "Fall");
//             return;
//         }

//         if (playerFMFJ.IsOnCeiling())
//         {
//             playerFMFJ.Velocity = new Vector2(0, playerFMFJ.Velocity.Y); 
//             EmitSignal(SignalName.Transitioned, this, "Fall");
//             return;
//         }

//         if (playerFMFJ.IsOnFloor())
//         {
//             float direction = Input.GetAxis("Left", "Right");

//             if (direction == 0)
//             {
//                 // playerFMFJ.PlayAnimation("Idle");
//                 EmitSignal(SignalName.Transitioned, this, "Idle");
//             }
//             else
//             {
//                 EmitSignal(SignalName.Transitioned, this, "GroundMove");
//             }
//             return;
//         }
//     }
// }
