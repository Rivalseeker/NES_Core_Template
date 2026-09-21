using Godot;
using System;

public partial class PlayerFixedMoveAndFixedJump : CharacterBody2D
{
    [ExportGroup("Movement Stats")]
    [Export] public float moveSpeed = 64.0f;
    [Export] public float MaxFallSpeed = 300f;

    [ExportCategory("Jump Mechanics")]
    [Export] public float jumpHeight = 48.0f; 
    [Export] public float jumpTimeToPeak = 0.4f;
    [Export] public float jumpTimeToDescent = 0.3f; 

    public float JumpVelocity { get; private set; }
    public float JumpGravity { get; private set; }
    public float FallGravity { get; private set; }

    public int FacingDirection { get; set; } = 1; 
    public bool IsIdle { get; set; } = false;
    public bool IsGroundMove { get; set; } = false;

    [ExportCategory("Visuals")]
    [Export] public NESSprite playerSprite;
    [Export] public AnimationPlayer playerAnim;

    [ExportCategory("Debug")]
    [Export] private CanvasLayer PlayerDebugScreen;
    [Export] private Label PlayerStateLabel;
    [Export] private Label PlayerXVelocityLabel;
    [Export] private Label PlayerYVelocityLabel;

    [Export] public Node PlayerStateMachine { get; set; }

    public override void _Ready()
    {
        CalculateJumpPhysics(); // FIXED: Jump math is now initialized.

        if (PlayerStateMachine == null)
        {
            PlayerStateMachine = GetNode<Node>("PlayerStateMachine");
        }

        if (PlayerDebugScreen != null)
        {
            PlayerDebugScreen.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("DebugPlayer") && PlayerDebugScreen != null)
        {
            PlayerDebugScreen.Visible = !PlayerDebugScreen.Visible;
        }

        if (PlayerDebugScreen != null && PlayerDebugScreen.Visible)
        {
            UpdateDebugUI();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Centralized gravity and slide execution
        if (!IsOnFloor())
        {
            float currentGravity = velocity.Y < 0 ? JumpGravity : FallGravity;
            velocity.Y += currentGravity * (float)delta;
            velocity.Y = Mathf.Min(velocity.Y, MaxFallSpeed); // FIXED: Capped fall speed centrally
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void CalculateJumpPhysics()
    {
        JumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f;
        JumpGravity = (2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak);
        FallGravity = (2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent);
    }

    public void UpdateFacingDirection(float inputX)
    {
        if (inputX > 0)
        {
            FacingDirection = 1;
            playerSprite.FlipH = false;
        }
        else if (inputX < 0)
        {
            FacingDirection = -1;
            playerSprite.FlipH = true;
        }
    }

    public void PlayAnimation(string animName)
    {
        if (playerAnim != null && playerAnim.HasAnimation(animName))
        {
            playerAnim.Play(animName);
        }
    }

    private void UpdateDebugUI()
    {
        if (PlayerXVelocityLabel != null)
            PlayerXVelocityLabel.Text = $"X= {Velocity.X:F2}";
        
        if (PlayerYVelocityLabel != null)
            PlayerYVelocityLabel.Text = $"Y= {Velocity.Y:F2}";

        if (PlayerStateLabel != null && PlayerStateMachine != null)
        {
            var currentState = PlayerStateMachine.Get("_playerFixedMoveAndFixedJumpState").As<Node>();
            if (currentState != null)
            {
                PlayerStateLabel.Text = currentState.Name.ToString().ToUpper(); 
            }
        }
    }
}






// using Godot;
// using System;

// public partial class PlayerFixedMoveAndFixedJump : CharacterBody2D
// {
//     // ==========================================
//     // MOVEMENT PARAMETERS
//     // ==========================================
//     [ExportGroup("Movement Stats")]
//     [Export] public float moveSpeed = 120.0f;
//     [Export] public float MaxFallSpeed = 300f;

//     // ==========================================
//     // KINEMATIC JUMP PARAMETERS
//     // ==========================================
//     [ExportCategory("Jump Mechanics")]
//     [Export] public float jumpHeight = 48.0f; 
//     [Export] public float jumpTimeToPeak = 0.4f;
//     [Export] public float jumpTimeToDescent = 0.3f; 

//     public float JumpVelocity { get; private set; }
//     public float JumpGravity { get; private set; }
//     public float FallGravity { get; private set; }

//     // ==========================================
//     // PUBLIC STATE VARIABLES
//     // ==========================================
//     public int FacingDirection { get; set; } = 1; 
//     public bool IsIdle { get; set; } = false;
//     public bool IsGroundMove { get; set; } = false;

//     // ==========================================
//     // NODE REFERENCES
//     // ==========================================
//     [ExportCategory("Visuals")]
//     [Export] public NESSprite playerSprite;
//     [Export] public AnimationPlayer playerAnim;

//     [ExportCategory("Debug")]
//     [Export] private CanvasLayer PlayerDebugScreen;
//     [Export] private Label PlayerStateLabel;
//     [Export] private Label PlayerXVelocityLabel;
//     [Export] private Label PlayerYVelocityLabel;
//     // [Export] private Label PlayerCoyoteTimerLabel;

//     [Export] public Node PlayerStateMachine { get; set; }




//     public override void _Ready()
//     {
//         // If not assigned in the inspector, fetch the state machine automatically
//         if (PlayerStateMachine == null)
//         {
//             PlayerStateMachine = GetNode<Node>("PlayerStateMachine");
//         }

//         // 2. Ensure the debug screen is hidden on load
//         if (PlayerDebugScreen != null)
//         {
//             PlayerDebugScreen.Visible = false;
//         }
//     }


//     public override void _Process(double delta)
//     {
//         if (Input.IsActionJustPressed("DebugPlayer"))
//         {
//             if (PlayerDebugScreen != null)
//             {
//                 PlayerDebugScreen.Visible = !PlayerDebugScreen.Visible;
//             }
//         }

//         // 2. Update the UI only if the screen is currently visible
//         if (PlayerDebugScreen != null && PlayerDebugScreen.Visible)
//         {
//             UpdateDebugUI();
//         }
//     }


//     public override void _PhysicsProcess(double delta)
//     {
//         Vector2 velocity = Velocity;

//         if (!IsOnFloor())
//         {
//             float currentGravity = velocity.Y < 0 ? JumpGravity : FallGravity;
//             velocity.Y += currentGravity * (float)delta;
//         }

//         Velocity = velocity;
//         MoveAndSlide();
//     }




//     private void CalculateJumpPhysics()
//     {
//         JumpVelocity = ((2.0f * jumpHeight) / jumpTimeToPeak) * -1.0f;
//         JumpGravity = (2.0f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak);
//         FallGravity = (2.0f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent);
//     }


//     public void UpdateFacingDirection(float inputX)
//     {
//         if (inputX > 0)
//         {
//             FacingDirection = 1;
//             playerSprite.FlipH = false;
//         }
//         else if (inputX < 0)
//         {
//             FacingDirection = -1;
//             playerSprite.FlipH = true;
//         }
//     }


//     // public void ApplyHorizontalMovement(float direction)
//     // {
//     //     Vector2 vel = Velocity;
//     //     vel.X = direction * moveSpeed;
//     //     Velocity = vel;

//     //     if (direction != 0)
//     //     {
//     //         UpdateSpriteFacing(direction < 0);
//     //     }
//     // }


//     public void PlayAnimation(string animName)
//     {
//         if (playerAnim != null && playerAnim.HasAnimation(animName))
//         {
//             playerAnim.Play(animName);
//         }
//     }


//     private void UpdateDebugUI()
//     {
//         // Format the velocity to 2 decimal places to prevent text jitter
//         if (PlayerXVelocityLabel != null)
//             PlayerXVelocityLabel.Text = $"X= {Velocity.X:F2}";
        
//         if (PlayerYVelocityLabel != null)
//             PlayerYVelocityLabel.Text = $"Y= {Velocity.Y:F2}";
        
//         // // NEW: Update the Coyote Timer label, formatting to 2 decimal places
//         // if (PlayerCoyoteTimerLabel != null)
//         //     PlayerCoyoteTimerLabel.Text = $"COYOTE= {ActiveCoyoteTimer:F3}";

//         // Retrieve the current state name from your state machine
//         if (PlayerStateLabel != null && PlayerStateMachine != null)
//         {
//             // Assumes your PlayerStateMachine script has a public property named 'CurrentState'
//             // We use dynamic/Variant retrieval here in case we don't know the exact class name
//             var currentState = PlayerStateMachine.Get("_playerFixedMoveAndFixedJumpState").As<Node>();
            
//             if (currentState != null)
//             {
//                 // ToUpper() matches the "IDLE" styling in your screenshot
//                 PlayerStateLabel.Text = currentState.Name.ToString().ToUpper(); 
//             }
//         }
//     }
// }
