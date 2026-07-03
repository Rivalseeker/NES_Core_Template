using Godot;
using System;
using System.Collections.Generic;

public partial class NESSpriteManager : Node
{
    // Static instance allowing individual sprites to easily register themselves
    public static NESSpriteManager Instance { get; private set; }

    [Export]
    public bool FlickerEnabled { get; set; } = true;

    // Adjusted for your project's specific vertical viewport height
    private const int TargetHeight = 216; 
    private const int MaxSpritesPerScanline = 8;

    private readonly List<Sprite2D> _activeSprites = new();

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        // On boot, pull the loaded settings profile directly from SaveManager
        if (SaveManager.Instance != null)
        {
            FlickerEnabled = SaveManager.Instance.CurrentGameData.SpriteFlicker;
        }
    }

    public void RegisterSprite(Sprite2D sprite)
    {
        if (!_activeSprites.Contains(sprite))
        {
            _activeSprites.Add(sprite);
        }
    }

    public void UnregisterSprite(Sprite2D sprite)
    {
        _activeSprites.Remove(sprite);
    }

    public override void _Process(double delta)
    {
        // If the feature is toggled off, ensure everything stays visible and exit
        if (!FlickerEnabled)
        {
            foreach (var sprite in _activeSprites)
            {
                if (GodotObject.IsInstanceValid(sprite))
                {
                    sprite.Visible = true;
                }
            }
            return;
        }

        // 1. Simulate OAM priority cycling by shuffling evaluation order every frame
        ShuffleSprites(_activeSprites);

        // 2. Setup an array tracking sprite counts for each horizontal row of your 216 height
        int[] scanlineCounts = new int[TargetHeight];

        // 3. Evaluate line constraints based on the randomized priority list
        foreach (var sprite in _activeSprites)
        {
            if (!GodotObject.IsInstanceValid(sprite)) continue;

            // 1. Grab the full Canvas Transform (which contains both position and zoom scale)
            Transform2D canvasTransform = sprite.GetGlobalTransformWithCanvas();
            int spriteY = (int)canvasTransform.Origin.Y;

            // 2. Get the raw image height
            int rawHeight = sprite.Texture != null ? (int)sprite.Texture.GetSize().Y : 8;

            // 3. Multiply it by the camera zoom scale so it matches physical screen pixels!
            int textureHeight = Mathf.RoundToInt(rawHeight * canvasTransform.Scale.Y);

            int top = spriteY - (sprite.Centered ? textureHeight / 2 : 0);
            int bottom = top + textureHeight;

            bool shouldHide = false;

            // Scan through all horizontal rows this sprite covers
            for (int y = top; y < bottom; y++)
            {
                if (y >= 0 && y < TargetHeight)
                {
                    if (scanlineCounts[y] >= MaxSpritesPerScanline)
                    {
                        shouldHide = true;
                        break; // Drop limit reached for this scanline!
                    }
                }
            }

            if (shouldHide)
            {
                sprite.Visible = false;
            }
            else
            {
                sprite.Visible = true;
                // Accumulate this sprite's presence across its occupied scanlines
                for (int y = top; y < bottom; y++)
                {
                    if (y >= 0 && y < TargetHeight)
                    {
                        scanlineCounts[y]++;
                    }
                }
            }
        }
    }

    // High-performance Fisher-Yates shuffle method for OAM cycling simulation
    private void ShuffleSprites(List<Sprite2D> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            Sprite2D value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}




// using Godot;
// using System;
// using System.Collections.Generic;

// public partial class NESSpriteManager : Node
// {
//     // Static instance allowing individual sprites to easily register themselves
//     public static NESSpriteManager Instance { get; private set; }

//     [Export]
//     public bool FlickerEnabled { get; set; } = true;

//     // Adjusted for your project's specific vertical viewport height
//     private const int TargetHeight = 216; 
//     private const int MaxSpritesPerScanline = 8;

//     private readonly List<Sprite2D> _activeSprites = new();

//     public override void _EnterTree()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//         }
//         else
//         {
//             QueueFree();
//         }
//     }

//     public override void _Ready()
//     {
//         // On boot, pull the loaded settings profile directly from SaveManager
//         if (SaveManager.Instance != null)
//         {
//             FlickerEnabled = SaveManager.Instance.CurrentGameData.SpriteFlicker;
//         }
//     }

//     public void RegisterSprite(Sprite2D sprite)
//     {
//         if (!_activeSprites.Contains(sprite))
//         {
//             _activeSprites.Add(sprite);
//         }
//     }

//     public void UnregisterSprite(Sprite2D sprite)
//     {
//         _activeSprites.Remove(sprite);
//     }

//     public override void _Process(double delta)
//     {
//         // // Slows the calculation down so you can see exactly which cursor is being dropped frame-by-frame
//         // Engine.MaxFps = 10;

//         // If the feature is toggled off, ensure everything stays visible and exit
//         if (!FlickerEnabled)
//         {
//             foreach (var sprite in _activeSprites)
//             {
//                 if (GodotObject.IsInstanceValid(sprite))
//                 {
//                     sprite.Visible = true;
//                 }
//             }
//             return;
//         }

//         // 1. Simulate OAM priority cycling by shuffling evaluation order every frame
//         ShuffleSprites(_activeSprites);

//         // 2. Setup an array tracking sprite counts for each horizontal row of your 216 height
//         int[] scanlineCounts = new int[TargetHeight];

//         // 3. Evaluate line constraints based on the randomized priority list
//         foreach (var sprite in _activeSprites)
//         {
//             if (!GodotObject.IsInstanceValid(sprite)) continue;

//             // Pixel-snapping guarantees GlobalPosition safely translates to clean integers
//             // Convert the sprite's world position into Camera/Screen Space coordinates
//             Vector2 screenPos = sprite.GetGlobalTransformWithCanvas().Origin;
//             int spriteY = (int)screenPos.Y;
//             int textureHeight = sprite.Texture != null ? (int)sprite.Texture.GetSize().Y : 8;

//             int top = spriteY - (sprite.Centered ? textureHeight / 2 : 0);
//             int bottom = top + textureHeight;

//             bool shouldHide = false;

//             // Scan through all horizontal rows this sprite covers
//             for (int y = top; y < bottom; y++)
//             {
//                 if (y >= 0 && y < TargetHeight)
//                 {
//                     if (scanlineCounts[y] >= MaxSpritesPerScanline)
//                     {
//                         shouldHide = true;
//                         break; // Drop limit reached for this scanline!
//                     }
//                 }
//             }

//             if (shouldHide)
//             {
//                 sprite.Visible = false;
//             }
//             else
//             {
//                 sprite.Visible = true;
//                 // Accumulate this sprite's presence across its occupied scanlines
//                 for (int y = top; y < bottom; y++)
//                 {
//                     if (y >= 0 && y < TargetHeight)
//                     {
//                         scanlineCounts[y]++;
//                     }
//                 }
//             }
//         }
//     }

//     // High-performance Fisher-Yates shuffle method for OAM cycling simulation
//     private void ShuffleSprites(List<Sprite2D> list)
//     {
//         int n = list.Count;
//         while (n > 1)
//         {
//             n--;
//             int k = Random.Shared.Next(n + 1);
//             Sprite2D value = list[k];
//             list[k] = list[n];
//             list[n] = value;
//         }
//     }
// }