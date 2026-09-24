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
    [Export] public float coyoteTime = 0.1f; // <-- NEW: Time in seconds allowed for a late jump


    public bool CanCoyoteJump { get; set; } = false;
    public double ActiveCoyoteTimer { get; set; } = 0.0;

    public float JumpVelocity { get; private set; }
    public float JumpGravity { get; private set; }
    public float FallGravity { get; private set; }

    public int FacingDirection { get; set; } = 1; 
    public bool IsIdle { get; set; } = false;
    public bool IsGroundMove { get; set; } = false;

    [ExportCategory("Visuals")]
    [Export] public NESSprite playerSprite;
    [Export] public AnimationPlayer playerAnim;
    // [Export] public AnimationPlayer playerGoalAnim;

    [ExportCategory("Debug")]
    [Export] private CanvasLayer PlayerDebugScreen;
    [Export] private Node2D JumpArc;
    [Export] private Label PlayerStateLabel;
    [Export] private Label PlayerXVelocityLabel;
    [Export] private Label PlayerYVelocityLabel;
    [Export] private Label PlayerCoyoteTimerLabel;

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

        if (JumpArc != null)
        {
            JumpArc.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("DebugPlayer"))
        {
            if (PlayerDebugScreen != null)
            {
                PlayerDebugScreen.Visible = !PlayerDebugScreen.Visible;
            }

            if (JumpArc != null)
            {
                JumpArc.Visible = !JumpArc.Visible;
            }
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
            JumpArc.Scale = new Vector2(1f, 1f);
        }
        else if (inputX < 0)
        {
            FacingDirection = -1;
            playerSprite.FlipH = true;
            JumpArc.Scale = new Vector2(-1f, 1f);
        }
    }

    public void PlayAnimation(string animName)
    {
        if (playerAnim != null && playerAnim.HasAnimation(animName))
        {
            playerAnim.Play(animName);
        }
    }

    // public void PlayGoalAnimation()
    // {
    //     playerGoalAnim.Play("Goal");
    // }

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

        // NEW: Update the Coyote Timer label, formatting to 2 decimal places
        if (PlayerCoyoteTimerLabel != null)
            PlayerCoyoteTimerLabel.Text = $"COYOTE= {ActiveCoyoteTimer:F3}";
    }
}