using Godot;
using System;

public partial class PlayerFixedMoveAndFixedJump : CharacterBody2D
{
    // ==========================================
    // MOVEMENT PARAMETERS
    // ==========================================
    [ExportGroup("Movement Mechanics")]
    [Export] public float MoveSpeed = 120.0f;
}
