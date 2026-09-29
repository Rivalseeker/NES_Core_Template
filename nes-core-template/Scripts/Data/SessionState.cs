using Godot;
using System;

public partial class SessionState : Node
{
    // Remembers the specific ID of the flag touched
    public static int ActiveCheckpointId { get; set; } = 0;

    // Remembers the path to the level currently being played
    public static string CurrentLevelPath { get; set; } = "";
}
