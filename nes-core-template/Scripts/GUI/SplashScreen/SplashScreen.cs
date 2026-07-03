using Godot;

public partial class SplashScreen : Control
{
    // Updated to use a file path string instead of a PackedScene
    [Export(PropertyHint.File, "*.tscn")] private string _titleScreenPath; 
    
    // Exported color property for easy configuration in the inspector
    // Defaults to black, highly typical for classic boot screens
    [Export] private Color _bootClearColor = new Color(0, 0, 0, 1); 

    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(_bootClearColor);
    }

    public override void _Input(InputEvent @event)
    {
        // Check if the exact action was just pressed this frame
        if (@event.IsActionPressed("StartAccept"))
        {
            // Consume the input so it doesn't accidentally trigger anything on the next screen
            AcceptEvent(); 
            
            // Instantly trigger the scene load
            OnSplashTimeout(); 
        }
    }

    private void OnSplashTimeout()
    {
        // Tell the global manager to load the title screen asynchronously
        if (!string.IsNullOrEmpty(_titleScreenPath))
        {
            SceneManager.Instance.LoadSceneAsync(_titleScreenPath);
        }
    }

    // Function to change the active viewport's clear color
    private void ApplyClearColor(Color newColor)
    {
        // This updates the actual rendering server immediately
        RenderingServer.SetDefaultClearColor(newColor);
        
        // Optional: If you need to read from ProjectSettings later in the session, 
        // you can also update the setting itself, though it's not required for the visual change.
        // ProjectSettings.SetSetting("rendering/environment/defaults/default_clear_color", newColor);
    }
}