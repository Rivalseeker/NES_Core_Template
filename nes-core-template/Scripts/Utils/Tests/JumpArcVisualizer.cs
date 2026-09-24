using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class JumpArcVisualizer : Node2D
{
    [ExportCategory("Arc Properties")]
    [Export] public bool ShowInEditor { get; set; } = true;
    [Export] public Color ArcColor { get; set; } = new Color(0.2f, 0.8f, 1.0f, 0.85f);
    [Export] public float LineWidth { get; set; } = 2.0f;
    [Export] public int ResolutionSteps { get; set; } = 30;
    [Export] public bool MirrorBothDirections { get; set; } = true;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            // Hide the arc when entering runtime
            Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            Visible = ShowInEditor;
            QueueRedraw();
        }
        else if (Visible)
        {
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        DrawJumpTrajectory(1);  // Right arc
        if (MirrorBothDirections)
        {
            DrawJumpTrajectory(-1); // Left arc
        }
    }

    private void DrawJumpTrajectory(int direction)
    {
        float moveSpeed = GetParentFloat("moveSpeed", 64.0f);
        float jumpHeight = GetParentFloat("jumpHeight", 48.0f);
        float tPeak = GetParentFloat("jumpTimeToPeak", 0.4f);
        float tDescent = GetParentFloat("jumpTimeToDescent", 0.3f);

        if (tPeak <= 0 || tDescent <= 0) return;

        // Kinematic calculations matching PlayerFixedMoveAndFixedJump
        float initialJumpVelocity = ((2.0f * jumpHeight) / tPeak) * -1.0f;
        float jumpGravity = (2.0f * jumpHeight) / (tPeak * tPeak);
        float fallGravity = (2.0f * jumpHeight) / (tDescent * tDescent);

        float totalAirTime = tPeak + tDescent;
        int steps = Math.Max(10, ResolutionSteps);

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i <= steps; i++)
        {
            float t = (totalAirTime * i) / steps;
            float x = direction * moveSpeed * t;
            float y;

            if (t <= tPeak)
            {
                // Ascent phase
                y = (initialJumpVelocity * t) + (0.5f * jumpGravity * t * t);
            }
            else
            {
                // Descent phase
                float tFall = t - tPeak;
                y = -jumpHeight + (0.5f * fallGravity * tFall * tFall);
            }

            points.Add(new Vector2(x, y));
        }

        // Draw trajectory arc segments
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(points[i], points[i + 1], ArcColor, LineWidth, true);
        }

        // Draw circles at Peak Apex and Floor Landing point
        if (points.Count > 0)
        {
            float xApex = direction * moveSpeed * tPeak;
            DrawCircle(new Vector2(xApex, -jumpHeight), LineWidth * 1.5f, ArcColor);
            DrawCircle(points[points.Count - 1], LineWidth * 1.5f, ArcColor);
        }
    }

    private float GetParentFloat(string propertyName, float fallbackValue)
    {
        Node parent = GetParent();
        if (parent == null) return fallbackValue;

        // Strong typing path used at runtime
        if (parent is PlayerFixedMoveAndFixedJump player)
        {
            switch (propertyName)
            {
                case "moveSpeed": return player.moveSpeed;
                case "jumpHeight": return player.jumpHeight;
                case "jumpTimeToPeak": return player.jumpTimeToPeak;
                case "jumpTimeToDescent": return player.jumpTimeToDescent;
            }
        }

        // Editor path: Non-tool parent nodes are generic CharacterBody2D instances.
        // Retrieve exported values dynamically via Godot's Variant API.
        Variant val = parent.Get(propertyName);
        if (val.VariantType != Variant.Type.Nil)
        {
            return val.AsSingle();
        }

        return fallbackValue;
    }
}