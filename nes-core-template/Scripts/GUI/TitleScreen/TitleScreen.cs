using Godot;

public partial class TitleScreen : Control
{
    [Export] private Color _sceneClearColor = new Color(0, 0, 0, 1);

    // Updated to point to the Main Menu
    [Export(PropertyHint.File, "*.tscn")] private string _mainMenuPath;

    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(_sceneClearColor);
    }

    public override void _Input(InputEvent @event)
    {
        // Check if the event is a key press or a joypad (controller) button press
        bool isAnyKeyPressed = @event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo;
        bool isAnyButtonPressed = @event is InputEventJoypadButton joypadEvent && joypadEvent.Pressed;

        if (isAnyKeyPressed || isAnyButtonPressed)
        {
            SetProcessInput(false); 
            
            if (!string.IsNullOrEmpty(_mainMenuPath))
            {
                Visible = false;
                SceneManager.Instance.LoadSceneAsync(_mainMenuPath);
            }
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