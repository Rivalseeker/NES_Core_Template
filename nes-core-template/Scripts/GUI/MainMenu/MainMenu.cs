using Godot;

public partial class MainMenu : Control
{
    [Export] private Color _sceneClearColor = new Color(0, 0, 0, 1);

    // UI Node References
    [ExportCategory("UI Nodes")]
    [Export] private Button _mainGameButton;
    [Export] private Button _optionsButton;
    [Export] private Button _creditsButton;
    [Export] private Button _quitButton;

    // Cursor References
    [ExportCategory("Cursor Settings")]
    [Export] private TextureRect _cursorIcon;
    [Export] private AnimationPlayer _cursorAnim;
    [Export] private Vector2 _cursorOffset = new Vector2(-72, 0); 

    // Scene Paths
    [ExportCategory("Scene Paths")]
    [Export(PropertyHint.File, "*.tscn")] private string _mainGamePath;
    [Export(PropertyHint.File, "*.tscn")] private string _optionsPath;
    [Export(PropertyHint.File, "*.tscn")] private string _creditsPath;

    // NEW: Variable to track the player's current position in the menu
    private Control _lastFocusedControl;

    public override void _Ready()
    {
        // Apply the background color immediately when the splash screen loads
        ApplyClearColor(_sceneClearColor);

        // Wire up the button press signals
        _mainGameButton.Pressed += OnMainGamePressed;
        _optionsButton.Pressed += OnOptionsPressed;
        _creditsButton.Pressed += OnCreditsPressed;
        _quitButton.Pressed += OnQuitPressed;

        // Wire up the focus signals to move our cursor
        _mainGameButton.FocusEntered += () => MoveCursorTo(_mainGameButton);
        _optionsButton.FocusEntered += () => MoveCursorTo(_optionsButton);
        _creditsButton.FocusEntered += () => MoveCursorTo(_creditsButton);
        _quitButton.FocusEntered += () => MoveCursorTo(_quitButton);

        // Focus Looping (Wrap-Around)
        _mainGameButton.FocusNeighborTop = _quitButton.GetPath();
        _quitButton.FocusNeighborBottom = _mainGameButton.GetPath();

        // NEW: Listen for when the SceneManager unhides this menu
        VisibilityChanged += OnVisibilityChanged;

        _mainGameButton.GrabFocus();
    }

    private void MoveCursorTo(Control targetButton)
    {
        if (_cursorIcon == null) return;

        // NEW: Save the button that just received focus
        _lastFocusedControl = targetButton;

        Vector2 targetPosition = targetButton.GlobalPosition;
        targetPosition.X += _cursorOffset.X;
        targetPosition.Y += (targetButton.Size.Y / 2.0f) - (_cursorIcon.Size.Y / 2.0f) + _cursorOffset.Y;

        _cursorIcon.GlobalPosition = targetPosition.Floor();

        if (_cursorAnim != null)
        {
            _cursorAnim.Stop();
            _cursorAnim.Play("Blink");
        }
    }

    // NEW: Restore focus exactly where the player left off
    private void OnVisibilityChanged()
    {
        // When SceneManager calls .Show(), this fires. If we have a saved control, grab focus.
        if (Visible && _lastFocusedControl != null)
        {
            _lastFocusedControl.GrabFocus();
        }
    }

    private void OnMainGamePressed()
    {
        // We no longer disable buttons since the menu just hides instead.
        if (!string.IsNullOrEmpty(_mainGamePath))
        {
            Visible = false;
            SceneManager.Instance.LoadSceneAsync(_mainGamePath);
        }
        
    }

    private void OnOptionsPressed()
    {
        if (!string.IsNullOrEmpty(_optionsPath)) SceneManager.Instance.OpenMenuOverlay(_optionsPath);
    }

    private void OnCreditsPressed()
    {
        if (!string.IsNullOrEmpty(_creditsPath)) SceneManager.Instance.OpenMenuOverlay(_creditsPath);
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
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