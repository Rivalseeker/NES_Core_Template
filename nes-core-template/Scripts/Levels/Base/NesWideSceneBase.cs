using Godot;
using System;

public partial class NesWideSceneBase : Node2D
{
    [Export] public Color sceneClearColor = new Color(0, 0, 0, 1);

    [ExportGroup("Spawning Options")]
    [Export] 
    public PackedScene playerScene;

    public Node2D spawnedPlayer;

    // References to our nodes
    [ExportCategory("Node References")]
    [Export] public Camera2D sceneCamera;
    [Export] public Sprite2D sceneCameraBorder;
    [Export] public ColorRect debugRuler;




    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(sceneClearColor);

        // Turn on Scene Camera Border
        sceneCamera.Visible = true;

        debugRuler.Visible = false;

        SpawnPlayer();
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("DebugLevelRuler") && debugRuler != null)
        {
            debugRuler.Visible = !debugRuler.Visible;
        }
    }


    public override void _PhysicsProcess(double delta)
    {
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

    


    // Function to change the active viewport's clear color
    private void ApplyClearColor(Color newColor)
    {
        // This updates the actual rendering server immediately
        RenderingServer.SetDefaultClearColor(newColor);
    }


    public virtual void SpawnPlayer()
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
        Marker2D playerSpawner = GetNodeOrNull<Marker2D>("Spawners/PlayerSpawner");

        if (playersContainer == null)
        {
            GD.PrintErr("NesPlatformerScene2: 'Entities/Players' node not found.");
            return;
        }

        if (playerSpawner == null)
        {
            GD.PrintErr("NesPlatformerScene2: 'Spawners/PlayerSpawner' node not found.");
            return;
        }

        // Instantiate the player character and store the reference
        spawnedPlayer = playerScene.Instantiate<Node2D>();

        // Set the position to match the Marker2D's global position
        spawnedPlayer.GlobalPosition = playerSpawner.GlobalPosition;

        // Add the player instance as a child of the Players Node2D container
        playersContainer.AddChild(spawnedPlayer);
    }
}