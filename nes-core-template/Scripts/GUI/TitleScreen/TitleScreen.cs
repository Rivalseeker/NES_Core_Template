using Godot;

public partial class TitleScreen : Control
{
    [Export] private Color _sceneClearColor = new Color(0, 0, 0, 1);
    [Export(PropertyHint.File, "*.tscn")] private string _mainMenuPath;
    
    // NEW: Path to the debug menu scene
    [Export(PropertyHint.File, "*.tscn")] private string _debugLevelSelectPath;

    public override void _Ready()
    {
        ApplyClearColor(_sceneClearColor);
        
        // NEW: Re-enable input when returning from overlays
        VisibilityChanged += () => {
            if (Visible) SetProcessInput(true);
        };
    }

    public override void _Input(InputEvent @event)
    {
        // NEW: Check for Debug input first
        if (@event.IsActionPressed("DebugLevelSelect") && !string.IsNullOrEmpty(_debugLevelSelectPath))
        {
            AcceptEvent();
            SetProcessInput(false); // Stop title screen from reacting
            SceneManager.Instance.OpenMenuOverlay(_debugLevelSelectPath);
            return;
        }

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

    private void ApplyClearColor(Color newColor)
    {
        RenderingServer.SetDefaultClearColor(newColor);
    }
}