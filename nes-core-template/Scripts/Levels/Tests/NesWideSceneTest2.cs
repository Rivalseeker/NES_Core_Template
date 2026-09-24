using Godot;
using System;
using Godot.Collections;

public partial class NesWideSceneTest2 : NesWideSceneBase
{
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

        SpawnPlayer();
    }


    public override void _PhysicsProcess(double delta)
    {
        // NEW: Make the scene camera track the spawned player's position
        if (sceneCamera != null && spawnedPlayer != null)
        {
            sceneCamera.GlobalPosition = spawnedPlayer.GlobalPosition;
        }

        // Ensure both nodes exist to prevent null reference errors
        if (sceneCamera != null && sceneCameraBorder != null)
        {
            Vector2 currentZoom = sceneCamera.Zoom;

            // Prevent division by zero
            if (currentZoom.X != 0 && currentZoom.Y != 0)
            {
                // Inverse relationship: as Zoom decreases, Scale increases (and vice versa)
                sceneCameraBorder.Scale = new Vector2(
                    1.0f / currentZoom.X,
                    1.0f / currentZoom.Y
                );
            }
        }
    }


    public override void SpawnPlayer()
    {
        // Check if a PackedScene was assigned in the Inspector; if not, try loading it automatically
        playerScene ??= GD.Load<PackedScene>("res://Scenes/Characters/Players/Tests/player_fixed_move_and_fixed_jump.tscn");

        if (playerScene == null)
        {
            GD.PrintErr("NesPlatformerScene2: Player scene could not be found or loaded.");
            return;
        }

        // Fetch references to the container and spawner nodes based on scene structure
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

        // Instantiate the player character and store the reference
        spawnedPlayer = playerScene.Instantiate<Node2D>();

        // Set the position to match the Marker2D's global position
        spawnedPlayer.GlobalPosition = playerSpawner.GlobalPosition;

        // Add the player instance as a child of the Players Node2D container
        playersContainer.AddChild(spawnedPlayer);

        // Fetch the RemoteTransform2D from the newly spawned player
        RemoteTransform2D remoteTransform = spawnedPlayer.GetNode<RemoteTransform2D>("CameraRemote");
        
        // Assign the scene camera to the remote transform
        if (remoteTransform != null && sceneCamera != null)
        {
            remoteTransform.RemotePath = remoteTransform.GetPathTo(sceneCamera);
        }
    }
}
