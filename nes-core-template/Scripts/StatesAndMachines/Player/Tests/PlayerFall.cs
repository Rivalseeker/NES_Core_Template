using Godot;

public partial class PlayerFall : Player_FixedMoveAndJump_State
{
    public override void Enter()
    {
        // GD.Print("FALLFIRST");

        // Setup Coyote Timer using the player's public property
        if (playerFMFJ.CanCoyoteJump)
        {
            playerFMFJ.ActiveCoyoteTimer = playerFMFJ.coyoteTime;
            playerFMFJ.CanCoyoteJump = false; // Reset the flag so it isn't reused 
        }
        else
        {
            playerFMFJ.ActiveCoyoteTimer = 0.0;
        }

        playerFMFJ.PlayAnimation("Fall");
    }

    public override void PhysicsUpdate(double delta)
    {
        if (playerFMFJ == null) return;

        // Coyote Time Logic: Count down and check for jump input
        if (playerFMFJ.ActiveCoyoteTimer > 0)
        {
            playerFMFJ.ActiveCoyoteTimer -= delta;

            // Prevent the timer from displaying negative numbers on the UI frame
            if (playerFMFJ.ActiveCoyoteTimer < 0)
                playerFMFJ.ActiveCoyoteTimer = 0;

            if (Input.IsActionJustPressed("Jump"))
            {
                playerFMFJ.ActiveCoyoteTimer = 0.0; // Consume the timer immediately
                EmitSignal(SignalName.Transitioned, this, "Jump");
                return;
            }
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
