using Godot;
using System;
using Godot.Collections;

public partial class NesWideSceneTest2 : NesWideSceneBase
{
    [ExportCategory("Node References")]
    [Export] public Node checkpointContainer;
    [Export] public Marker2D playerSpawner;
    [Export] private Area2D goalArea;
    [Export] private AnimationPlayer goalAnim;

    [Export(PropertyHint.File, "*.tscn")] private string nextLevelPath;


    public Vector2 currentSpawnPosition;




    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(sceneClearColor);

        // Turn on Scene Camera Border
        sceneCamera.Visible = true;

        debugRuler.Visible = false;

        if (playerSpawnerGroup != null)
        {
            foreach (Node child in playerSpawnerGroup.GetChildren())
            {
                child.QueueFree();
            }
        }

        // Auto-fetch GoalArea node if not assigned in Inspector
        goalArea ??= GetNodeOrNull<Area2D>("GoalArea");

        // Connect the BodyEntered signal
        if (goalArea != null)
        {
            goalArea.BodyEntered += OnGoalAreaBodyEntered;
        }

        // Memorize this level so the Pause Screen knows what to reload
        SessionState.CurrentLevelPath = this.SceneFilePath;

        currentSpawnPosition = playerSpawner != null ? playerSpawner.Position : Vector2.Zero;

        if (checkpointContainer != null)
        {
            foreach (Node child in checkpointContainer.GetChildren())
            {
                if (child is CheckpointFlag flag)
                {
                    // If this flag matches our saved session memory, make it Active and spawn here!
                    if (SessionState.ActiveCheckpointId != 0 && flag.CheckpointId == SessionState.ActiveCheckpointId)
                    {
                        flag.InitializeState(true);
                        currentSpawnPosition = flag.Position;
                    }
                    else
                    {
                        flag.InitializeState(false);
                    }
                }
            }
        }

        SpawnPlayer();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Make the scene camera track the spawned player's position
        if (sceneCamera != null && spawnedPlayer != null)
        {
            sceneCamera.GlobalPosition = spawnedPlayer.GlobalPosition;
        }

        // Ensure both nodes exist to prevent null reference errors
        if (sceneCamera != null && sceneCameraBorder != null)
        {
            Vector2 currentZoom = sceneCamera.Zoom;

            if (currentZoom.X != 0 && currentZoom.Y != 0)
            {
                sceneCameraBorder.Scale = new Vector2(
                    1.0f / currentZoom.X,
                    1.0f / currentZoom.Y
                );
            }
        }
    }

    public override void SpawnPlayer()
    {
        playerScene ??= GD.Load<PackedScene>("res://Scenes/Characters/Players/Tests/player_fixed_move_and_fixed_jump.tscn");

        if (playerScene == null)
        {
            GD.PrintErr("NesPlatformerScene2: Player scene could not be found or loaded.");
            return;
        }

        Node2D playersContainer = GetNodeOrNull<Node2D>("Entities/Players");
        Marker2D playerSpawner = GetNodeOrNull<Marker2D>("Spawners/PlayerSpawnerGroup/PlayerSpawner");

        if (playersContainer == null)
        {
            GD.PrintErr("NesPlatformerScene2: 'Entities/Players' node not found.");
            return;
        }

        if (playerSpawner == null)
        {
            GD.PrintErr("NesPlatformerScene2: 'Spawners/PlayerSpawnerGroup/PlayerSpawner' node not found.");
            return;
        }

        spawnedPlayer = playerScene.Instantiate<Node2D>();
        spawnedPlayer.GlobalPosition = playerSpawner.GlobalPosition;
        playersContainer.AddChild(spawnedPlayer);

        RemoteTransform2D remoteTransform = spawnedPlayer.GetNode<RemoteTransform2D>("CameraRemote");
        
        if (remoteTransform != null && sceneCamera != null)
        {
            remoteTransform.RemotePath = remoteTransform.GetPathTo(sceneCamera);
        }
    }

    private void OnGoalAreaBodyEntered(Node2D body)
    {
        if (body is PlayerFixedMoveAndFixedJump player)
        {
            // 1. Snap player global position to GoalArea global position
            player.GlobalPosition = goalArea.GlobalPosition;

            // 2. Lock player state to "Goal" so no further inputs or movements occur
            if (player.PlayerStateMachine is Player_FixedMoveAndFixedJump_StateMachine stateMachine)
            {
                stateMachine.ForceTransition("Goal");

                goalAnim.Play("Goal");
            }
        }
    }

    /// <summary>
    /// Swaps the current scene out for a new Node2D level scene via SceneManager.
    /// Can be called directly by an AnimationPlayer Call Method track.
    /// </summary>
    public void LoadNextScene()
    {
        // string path = !string.IsNullOrEmpty(targetPath) ? targetPath : nextScenePath;

        // if (string.IsNullOrEmpty(path))
        // {
        //     GD.PrintErr("NesWideSceneTest2: No target scene path specified for scene transition.");
        //     return;
        // }

        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadSceneAsync(nextLevelPath);
        }
        else
        {
            GD.PrintErr("NesWideSceneTest2: SceneManager.Instance is null. Ensure SceneManager is initialized.");
        }
    }
}