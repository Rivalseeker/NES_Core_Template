using Godot;
using System;

public partial class CheckpointFlag : Area2D
{
    [Export] public int CheckpointId { get; set; } = 1;
    [Export] private AnimationPlayer animPlayer;

    public static event Action<int> CheckpointTriggered;

    private bool _isActive = false;




    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        CheckpointTriggered += OnGlobalCheckpointTriggered;

        // if (animPlayer != null)
        // {
        //     animPlayer.AnimationFinished += OnAnimationFinished;
        // }
    }




    // Called by the level when it boots up to set the initial visual state
    public void InitializeState(bool makeActive)
    {
        _isActive = makeActive;
        animPlayer?.Play(makeActive ? "Active" : "Inactive");
    }


    private void OnBodyEntered(Node2D body)
    {
        // If it's already active, ignore the player completely
        if (_isActive) return;

        if (body is PlayerFixedMoveAndFixedJump)
        {
            _isActive = true;
            SessionState.ActiveCheckpointId = CheckpointId;
            
            // THE NEW LOGIC: Secure any items collected between the start/last checkpoint and now!
            // SessionState.CommitCollectedItems();

            animPlayer?.Play("Active");

            // Shout out to all other flags in the level to deactivate themselves
            CheckpointTriggered?.Invoke(CheckpointId);

            // GD.Print("Checkpoint!");
        }
    }

    private void OnGlobalCheckpointTriggered(int newCheckpointId)
    {
        // If another flag was just touched, and WE are currently active, turn ourselves off
        if (newCheckpointId != CheckpointId && _isActive)
        {
            _isActive = false;
            animPlayer?.Play("Inactive");
        }
    }

    public override void _ExitTree()
    {
        // Always unsubscribe from static events to prevent memory leaks!
        CheckpointTriggered -= OnGlobalCheckpointTriggered;
    }
}
