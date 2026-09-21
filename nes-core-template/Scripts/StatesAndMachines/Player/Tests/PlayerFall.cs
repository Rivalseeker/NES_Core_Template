using Godot;

public partial class PlayerFall : Player_FixedMoveAndJump_State
{
    public override void Enter()
    {
        playerFMFJ.PlayAnimation("Fall");
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

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

// public partial class PlayerFall : Player_FixedMoveAndJump_State
// {
//     public override void Enter()
//     {
//         playerFMFJ.PlayAnimation("Fall");
//     }

//     public override void PhysicsUpdate(double delta)
//     {
//         if (playerFMFJ == null) return;

//         Vector2 vel = playerFMFJ.Velocity;
//         vel.Y += playerFMFJ.FallGravity * (float)delta;

//         vel.Y = Mathf.Min(vel.Y, playerFMFJ.MaxFallSpeed);

//         playerFMFJ.Velocity = vel;

//         playerFMFJ.MoveAndSlide();

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
