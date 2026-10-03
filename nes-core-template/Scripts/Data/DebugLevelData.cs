using Godot;

[GlobalClass]
public partial class DebugLevelData : Resource
{
    [Export] public string LevelName { get; set; } = "New Level";
    [Export(PropertyHint.File, "*.tscn")] public string ScenePath { get; set; }
    
    // Checkpoint count (e.g., 2 means the level has Start, CP1, and CP2)
    [Export] public int CheckpointCount { get; set; } = 0; 
}