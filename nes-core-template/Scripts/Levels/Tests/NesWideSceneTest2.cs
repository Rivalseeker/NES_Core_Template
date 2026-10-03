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
        ApplyClearColor(sceneClearColor);

        sceneCamera.Visible = true;
        debugRuler.Visible = false;

        if (playerSpawnerGroup != null)
        {
            foreach (Node child in playerSpawnerGroup.GetChildren())
            {
                child.QueueFree();
            }
        }

        goalArea ??= GetNodeOrNull<Area2D>("GoalArea");

        if (goalArea != null)
        {
            goalArea.BodyEntered += OnGoalAreaBodyEntered;
        }

        SessionState.CurrentLevelPath = this.SceneFilePath;

        // Default spawn position set to playerSpawner's GlobalPosition
        currentSpawnPosition = playerSpawner != null ? playerSpawner.GlobalPosition : Vector2.Zero;

        if (checkpointContainer != null)
        {
            foreach (Node child in checkpointContainer.GetChildren())
            {
                if (child is CheckpointFlag flag)
                {
                    if (SessionState.ActiveCheckpointId != 0 && flag.CheckpointId == SessionState.ActiveCheckpointId)
                    {
                        flag.InitializeState(true);
                        // Store the flag's world position for spawning
                        currentSpawnPosition = flag.GlobalPosition;
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
        if (sceneCamera != null && spawnedPlayer != null)
        {
            sceneCamera.GlobalPosition = spawnedPlayer.GlobalPosition;
        }

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

        if (playersContainer == null)
        {
            GD.PrintErr("NesPlatformerScene2: 'Entities/Players' node not found.");
            return;
        }

        spawnedPlayer = playerScene.Instantiate<Node2D>();
        
        // Spawn player directly at currentSpawnPosition (Checkpoint or Spawner GlobalPosition)
        spawnedPlayer.GlobalPosition = currentSpawnPosition;
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
            player.GlobalPosition = goalArea.GlobalPosition;

            if (player.PlayerStateMachine is Player_FixedMoveAndFixedJump_StateMachine stateMachine)
            {
                stateMachine.ForceTransition("Goal");
                goalAnim.Play("Goal");
            }
        }
    }

    public void LoadNextScene()
    {
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