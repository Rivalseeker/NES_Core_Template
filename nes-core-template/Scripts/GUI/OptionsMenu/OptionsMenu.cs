using Godot;
using System;

public partial class OptionsMenu : Control
{
    private enum MenuState { Main, Audio, Video }
    private MenuState _activeSubMenu = MenuState.Main;

    // ==========================================
    // NODE REFERENCES
    // ==========================================
    [ExportCategory("Menu Containers")]
    [Export] private VBoxContainer _optionsList;
    [Export] private VBoxContainer _audioList;
    [Export] private VBoxContainer _videoList;

    [ExportCategory("Main Options Nodes")]
    [Export] private Button _keyboardButton;
    [Export] private Button _controllerButton;
    [Export] private Button _audioButton;
    [Export] private Button _videoButton;
    [Export] private Button _backButton;

    [ExportCategory("Audio Nodes")]
    [Export] private RetroSlider _masterVolSlider;
    [Export] private RetroSlider _musicVolSlider;
    [Export] private RetroSlider _sfxVolSlider;
    [Export] private RetroSelector _speakerSelector;
    [Export] private Button _audioBackButton;

    [ExportCategory("Video Nodes")]
    [Export] private RetroSelector _windowSizeSelector;
    [Export] private RetroSelector _fullscreenSelector;
    [Export] private RetroSelector _vsyncSelector;
    [Export] private RetroSelector _flickerSelector;
    [Export] private RetroSelector _pixelMovementSelector;
    [Export] private Button _videoBackButton;

    [ExportCategory("Confirm Prompt Nodes")]
    [Export] private Control _confirmPrompt;
    [Export] private Button _promptYesButton;
    [Export] private Button _promptNoButton;
    [Export] private Button _promptCancelButton;

    // ==========================================
    // CURSOR SETTINGS
    // ==========================================
    [ExportCategory("Main Cursor Settings")]
    [Export] private TextureRect _cursorIcon;
    [Export] private AnimationPlayer _cursorAnim;
    [Export] private Vector2 _cursorOffset = new Vector2(-72, 0);

    [ExportCategory("Prompt Cursor Settings")]
    [Export] private TextureRect _promptCursor;
    [Export] private AnimationPlayer _promptCursorAnim;
    [Export] private Vector2 _promptCursorOffset = new Vector2(-32, 0);

    // ==========================================
    // VIDEO SETTINGS STATE
    // ==========================================
    private readonly Vector2I _baseResolution = new Vector2I(384, 216);
    private int _maxWindowScale;
    private int _previousWindowScaleIndex; // NEW: Remembers the size before going fullscreen

    // ==========================================
    // INITIALIZATION
    // ==========================================
    public override void _Ready()
    {
        GenerateResolutionOptions();

        // 1. Wire Button Presses
        _audioButton.Pressed += () => OpenSubMenu(MenuState.Audio);
        _videoButton.Pressed += () => OpenSubMenu(MenuState.Video);
        _backButton.Pressed += CloseOptionsOverlay;

        _audioBackButton.Pressed += AttemptCloseSubMenu;
        _videoBackButton.Pressed += AttemptCloseSubMenu;

        _promptYesButton.Pressed += OnPromptYes;
        _promptNoButton.Pressed += OnPromptNo;
        _promptCancelButton.Pressed += OnPromptCancel;

        // 2. Wire Cursor Tracking (Main List)
        _keyboardButton.FocusEntered += () => MoveCursorTo(_keyboardButton);
        _controllerButton.FocusEntered += () => MoveCursorTo(_controllerButton);
        _audioButton.FocusEntered += () => MoveCursorTo(_audioButton);
        _videoButton.FocusEntered += () => MoveCursorTo(_videoButton);
        _backButton.FocusEntered += () => MoveCursorTo(_backButton);

        // 3. Wire Cursor Tracking (Audio List)
        _masterVolSlider.FocusEntered += () => MoveCursorTo(_masterVolSlider);
        _musicVolSlider.FocusEntered += () => MoveCursorTo(_musicVolSlider);
        _sfxVolSlider.FocusEntered += () => MoveCursorTo(_sfxVolSlider);
        _speakerSelector.FocusEntered += () => MoveCursorTo(_speakerSelector);
        _audioBackButton.FocusEntered += () => MoveCursorTo(_audioBackButton);

        // 4. Wire Cursor Tracking (Video List)
        _windowSizeSelector.FocusEntered += () => MoveCursorTo(_windowSizeSelector);
        _fullscreenSelector.FocusEntered += () => MoveCursorTo(_fullscreenSelector);
        _vsyncSelector.FocusEntered += () => MoveCursorTo(_vsyncSelector);
        _flickerSelector.FocusEntered += () => MoveCursorTo(_flickerSelector);
        _pixelMovementSelector.FocusEntered += () => MoveCursorTo(_pixelMovementSelector);
        _videoBackButton.FocusEntered += () => MoveCursorTo(_videoBackButton);

        // 5. Wire Cursor Tracking (Prompt)
        _promptYesButton.FocusEntered += () => MovePromptCursorTo(_promptYesButton);
        _promptNoButton.FocusEntered += () => MovePromptCursorTo(_promptNoButton);
        _promptCancelButton.FocusEntered += () => MovePromptCursorTo(_promptCancelButton);

        // 6. Live Video Event Listeners
        _windowSizeSelector.OnValueChanged += OnWindowSizeChanged;
        _fullscreenSelector.OnValueChanged += OnFullscreenChanged;
        _vsyncSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();

        // THE FIX: Make sure this line exists so the menu talks to the manager in real-time!
        _flickerSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();
        _pixelMovementSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();

        // 7. Focus Looping (Wrap-Around)
        _keyboardButton.FocusNeighborTop = _backButton.GetPath();
        _backButton.FocusNeighborBottom = _keyboardButton.GetPath();

        _masterVolSlider.FocusNeighborTop = _audioBackButton.GetPath();
        _audioBackButton.FocusNeighborBottom = _masterVolSlider.GetPath();

        _windowSizeSelector.FocusNeighborTop = _videoBackButton.GetPath();
        _videoBackButton.FocusNeighborBottom = _windowSizeSelector.GetPath();

        _promptYesButton.FocusNeighborLeft = _promptCancelButton.GetPath();
        _promptYesButton.FocusNeighborRight = _promptNoButton.GetPath();
        _promptNoButton.FocusNeighborLeft = _promptYesButton.GetPath();
        _promptNoButton.FocusNeighborRight = _promptCancelButton.GetPath();
        _promptCancelButton.FocusNeighborLeft = _promptNoButton.GetPath();
        _promptCancelButton.FocusNeighborRight = _promptYesButton.GetPath();

        // Trap Prompt Focus
        _promptYesButton.FocusNeighborTop = _promptYesButton.GetPath();
        _promptYesButton.FocusNeighborBottom = _promptYesButton.GetPath();
        _promptNoButton.FocusNeighborTop = _promptNoButton.GetPath();
        _promptNoButton.FocusNeighborBottom = _promptNoButton.GetPath();
        _promptCancelButton.FocusNeighborTop = _promptCancelButton.GetPath();
        _promptCancelButton.FocusNeighborBottom = _promptCancelButton.GetPath();

        // Initial State
        _audioList.Hide();
        _videoList.Hide();
        _confirmPrompt.Hide();
        _promptCursor.Hide();
        _optionsList.Show();
        _keyboardButton.GrabFocus();

        GetTree().Root.SizeChanged += OnWindowResized;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            AcceptEvent();
            if (_confirmPrompt.Visible) OnPromptCancel();
            else if (_activeSubMenu != MenuState.Main) AttemptCloseSubMenu();
            else CloseOptionsOverlay();
        }
    }

    // ==========================================
    // DYNAMIC RESOLUTION CALCULATION
    // ==========================================
    private void GenerateResolutionOptions()
    {
        // Find the absolute maximum integer scale that fits on the current monitor
        Vector2I screenSize = DisplayServer.ScreenGetSize();
        int maxScaleX = screenSize.X / _baseResolution.X;
        int maxScaleY = screenSize.Y / _baseResolution.Y;
        _maxWindowScale = Mathf.Min(maxScaleX, maxScaleY);

        string[] resOptions = new string[_maxWindowScale];
        for (int i = 0; i < _maxWindowScale; i++)
        {
            int scale = i + 1;
            resOptions[i] = $"{scale}X ({_baseResolution.X * scale}x{_baseResolution.Y * scale})";
        }
        
        _windowSizeSelector.Options = resOptions;
    }

    // ==========================================
    // CURSOR MOVEMENT
    // ==========================================
    private void MoveCursorTo(Control target)
    {
        if (_cursorIcon == null || !_cursorIcon.Visible) return;
        Vector2 pos = target.GlobalPosition;
        pos.X += _cursorOffset.X;
        pos.Y += (target.Size.Y / 2.0f) - (_cursorIcon.Size.Y / 2.0f) + _cursorOffset.Y;
        _cursorIcon.GlobalPosition = pos.Floor();
        _cursorAnim?.Stop();
        _cursorAnim?.Play("Blink");
    }

    private void MovePromptCursorTo(Control target)
    {
        if (_promptCursor == null || !_promptCursor.Visible) return;
        Vector2 pos = target.GlobalPosition;
        pos.X += _promptCursorOffset.X;
        pos.Y += (target.Size.Y / 2.0f) - (_promptCursor.Size.Y / 2.0f) + _promptCursorOffset.Y;
        _promptCursor.GlobalPosition = pos.Floor();
        _promptCursorAnim?.Stop();
        _promptCursorAnim?.Play("Blink");
    }

    // ==========================================
    // MENU NAVIGATION
    // ==========================================
    private void OpenSubMenu(MenuState menu)
    {
        _activeSubMenu = menu;
        _optionsList.Hide();

        if (menu == MenuState.Audio)
        {
            LoadSavedAudioToUI();
            _audioList.Show();
            _masterVolSlider.CallDeferred(Control.MethodName.GrabFocus);
        }
        else if (menu == MenuState.Video)
        {
            LoadSavedVideoToUI();
            _videoList.Show();
            _windowSizeSelector.CallDeferred(Control.MethodName.GrabFocus);
        }
    }

    private void AttemptCloseSubMenu()
    {
        bool hasChanged = _activeSubMenu == MenuState.Audio ? HasAudioChanged() : HasVideoChanged();

        if (hasChanged)
        {
            _cursorIcon.Hide();
            _promptCursor.Show();
            _confirmPrompt.Show();
            _promptYesButton.CallDeferred(Control.MethodName.GrabFocus);
        }
        else
        {
            ForceCloseSubMenu();
        }
    }

    private void ForceCloseSubMenu()
    {
        _promptCursor.Hide();
        _cursorIcon.Show();
        _confirmPrompt.Hide();
        
        _audioList.Hide();
        _videoList.Hide();
        _optionsList.Show();

        if (_activeSubMenu == MenuState.Audio) _audioButton.CallDeferred(Control.MethodName.GrabFocus);
        else if (_activeSubMenu == MenuState.Video) _videoButton.CallDeferred(Control.MethodName.GrabFocus);

        _activeSubMenu = MenuState.Main;
    }

    private void CloseOptionsOverlay()
    {
        SceneManager.Instance.CloseMenuOverlay();
    }

    // ==========================================
    // AUDIO LOGIC
    // ==========================================
    private void LoadSavedAudioToUI()
    {
        if (SaveManager.Instance == null) return;
        GameData d = SaveManager.Instance.CurrentGameData;
        _masterVolSlider.CurrentValue = d.MasterVolume;
        _musicVolSlider.CurrentValue = d.MusicVolume;
        _sfxVolSlider.CurrentValue = d.SfxVolume;
        _speakerSelector.SetValueNoNotify(d.SpeakerModeIndex);
    }

    private bool HasAudioChanged()
    {
        if (SaveManager.Instance == null) return false;
        GameData d = SaveManager.Instance.CurrentGameData;
        return _masterVolSlider.CurrentValue != d.MasterVolume ||
               _musicVolSlider.CurrentValue != d.MusicVolume ||
               _sfxVolSlider.CurrentValue != d.SfxVolume ||
               _speakerSelector.CurrentIndex != d.SpeakerModeIndex;
    }

    private void SaveAudio()
    {
        if (SaveManager.Instance == null) return;
        GameData d = SaveManager.Instance.CurrentGameData;
        d.MasterVolume = _masterVolSlider.CurrentValue;
        d.MusicVolume = _musicVolSlider.CurrentValue;
        d.SfxVolume = _sfxVolSlider.CurrentValue;
        d.SpeakerModeIndex = _speakerSelector.CurrentIndex;
        SaveManager.Instance.SaveGameData();
    }

    // ==========================================
    // VIDEO LOGIC
    // ==========================================
    private void LoadSavedVideoToUI()
    {
        if (SaveManager.Instance == null) return;
        GameData d = SaveManager.Instance.CurrentGameData;
        
        int scaleIndex = Mathf.Clamp(d.WindowScaleMultiplier - 1, 0, _maxWindowScale - 1);
        _previousWindowScaleIndex = scaleIndex; // Initialize the tracker
        
        _windowSizeSelector.SetValueNoNotify(scaleIndex);
        _fullscreenSelector.SetValueNoNotify(d.Fullscreen ? 1 : 0);
        _vsyncSelector.SetValueNoNotify(d.VSync ? 1 : 0);
        // _flickerSelector.SetValueNoNotify(d.SpriteFlicker ? 1 : 0);
        _flickerSelector.SetValueNoNotify(d.SpriteFlickerMode);
        _pixelMovementSelector.SetValueNoNotify(d.PixelMovement ? 1 : 0);
    }

    private bool HasVideoChanged()
    {
        if (SaveManager.Instance == null) return false;
        GameData d = SaveManager.Instance.CurrentGameData;
        int targetScale = _windowSizeSelector.CurrentIndex + 1;
        bool isFullscreen = _fullscreenSelector.CurrentIndex == 1;
        bool isVsync = _vsyncSelector.CurrentIndex == 1;
        // bool isFlicker = _flickerSelector.CurrentIndex == 1;
        int currentFlicker = _flickerSelector.CurrentIndex;
        bool isPixelMovement = _pixelMovementSelector.CurrentIndex == 1;

        // return targetScale != d.WindowScaleMultiplier || isFullscreen != d.Fullscreen || isVsync != d.VSync || isFlicker != d.SpriteFlicker || isPixelMovement != d.PixelMovement;
        return targetScale != d.WindowScaleMultiplier || isFullscreen != d.Fullscreen || isVsync != d.VSync || currentFlicker != d.SpriteFlickerMode || isPixelMovement != d.PixelMovement;
    }

    private void SaveVideo()
    {
        if (SaveManager.Instance == null) return;
        GameData d = SaveManager.Instance.CurrentGameData;
        d.WindowScaleMultiplier = _windowSizeSelector.CurrentIndex + 1;
        d.Fullscreen = _fullscreenSelector.CurrentIndex == 1;
        d.VSync = _vsyncSelector.CurrentIndex == 1;

        // 1. Assign to the data model
        // d.SpriteFlicker = _flickerSelector.CurrentIndex == 1;
        d.SpriteFlickerMode = _flickerSelector.CurrentIndex;
        d.PixelMovement = _pixelMovementSelector.CurrentIndex == 1;
        
        // 2. Write to disk
        SaveManager.Instance.SaveGameData();

        // 3. THE FIX: Push the saved state directly to the live Runtime memory
        if (NESSpriteManager.Instance != null)
        {
            // NESSpriteManager.Instance.FlickerEnabled = CurrentGameData.SpriteFlicker;
            NESSpriteManager.Instance.CurrentFlickerMode = (NESSpriteManager.FlickerMode)_flickerSelector.CurrentIndex;
        }
    }

    // private void ApplyLiveVideoSettings()
    // {
    //     bool isFullscreen = _fullscreenSelector.CurrentIndex == 1;
    //     bool isVsync = _vsyncSelector.CurrentIndex == 1;
    //     int scale = _windowSizeSelector.CurrentIndex + 1;

    //     // --- Interdependence Logic ---
    //     // If they pushed the resolution up to the max monitor limit, toggle fullscreen ON
    //     if (scale == _maxWindowScale && !isFullscreen)
    //     {
    //         _fullscreenSelector.SetValueNoNotify(1);
    //         isFullscreen = true;
    //     }
    //     // If they turned Fullscreen ON, force the window size UI to show the Max resolution
    //     else if (isFullscreen && scale < _maxWindowScale)
    //     {
    //         _windowSizeSelector.SetValueNoNotify(_maxWindowScale - 1);
    //         scale = _maxWindowScale;
    //     }

    //     // --- Apply to DisplayServer ---
    //     DisplayServer.WindowSetVsyncMode(isVsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);

    //     if (isFullscreen)
    //     {
    //         DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
    //     }
    //     else
    //     {
    //         DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            
    //         // Calculate and set the exact pixel size
    //         Vector2I newSize = new Vector2I(_baseResolution.X * scale, _baseResolution.Y * scale);
    //         DisplayServer.WindowSetSize(newSize);

    //         // Re-center the window on the screen so it doesn't get stuck in the top left
    //         Vector2I screenPos = DisplayServer.ScreenGetPosition();
    //         Vector2I screenSize = DisplayServer.ScreenGetSize();
    //         Vector2I centerPos = screenPos + (screenSize / 2) - (newSize / 2);
    //         DisplayServer.WindowSetPosition(centerPos);
    //     }
    // }

    // Triggered ONLY when the user clicks left/right on the Window Size selector
    private void OnWindowSizeChanged(int newIndex)
    {
        // If the user manually picked the absolute max monitor size, auto-trigger Fullscreen
        if (newIndex == _maxWindowScale - 1)
        {
            _fullscreenSelector.SetValueNoNotify(1);
        }
        else
        {
            // If they picked a lower size, ensure Fullscreen is OFF and remember this choice
            _fullscreenSelector.SetValueNoNotify(0);
            _previousWindowScaleIndex = newIndex;
        }

        ApplyVideoSettingsToServer();
    }

    // Triggered ONLY when the user clicks left/right on the Fullscreen selector
    private void OnFullscreenChanged(int newIndex)
    {
        bool isFullscreen = newIndex == 1;

        if (isFullscreen)
        {
            // Remember the current scale before forcing max, so we can return to it later
            if (_windowSizeSelector.CurrentIndex != _maxWindowScale - 1)
            {
                _previousWindowScaleIndex = _windowSizeSelector.CurrentIndex;
            }
            _windowSizeSelector.SetValueNoNotify(_maxWindowScale - 1);
        }
        else
        {
            // Exiting fullscreen: Restore the previous windowed scale
            int restoreIndex = _previousWindowScaleIndex;
            
            // Safety fallback: if the previous size was somehow max, bump it down by 1 so it isn't stuck
            if (restoreIndex == _maxWindowScale - 1) 
            {
                restoreIndex = Mathf.Max(0, _maxWindowScale - 2); 
            }
            
            _windowSizeSelector.SetValueNoNotify(restoreIndex);
        }

        ApplyVideoSettingsToServer();
    }

    // The "Dumb" applier: It simply looks at the UI and executes it on the OS Window.
    private void ApplyVideoSettingsToServer()
    {
        bool isFullscreen = _fullscreenSelector.CurrentIndex == 1;
        bool isVsync = _vsyncSelector.CurrentIndex == 1;
        int scale = _windowSizeSelector.CurrentIndex + 1;
        bool isFlicker = _flickerSelector.CurrentIndex == 1;
        bool isPixelMovement = _pixelMovementSelector.CurrentIndex == 1;

        // Live pass down to the Autoload Simulation Manager
        if (NESSpriteManager.Instance != null)
        {
            // NESSpriteManager.Instance.FlickerEnabled = isFlicker;
            NESSpriteManager.Instance.CurrentFlickerMode = (NESSpriteManager.FlickerMode)_flickerSelector.CurrentIndex;
        }

        DisplayServer.WindowSetVsyncMode(isVsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);

        Window mainWindow = GetTree().Root;
        if (mainWindow.IsEmbedded()) return;

        Viewport viewport = GetViewport();
        viewport.Snap2DTransformsToPixel = isPixelMovement ? viewport.Snap2DTransformsToPixel = true : viewport.Snap2DTransformsToPixel = false;

        if (isFullscreen)
        {
            mainWindow.Mode = Window.ModeEnum.ExclusiveFullscreen;
        }
        else
        {
            mainWindow.Mode = Window.ModeEnum.Windowed;
            
            Vector2I newSize = new Vector2I(_baseResolution.X * scale, _baseResolution.Y * scale);
            mainWindow.Size = newSize;

            Vector2I screenPos = DisplayServer.ScreenGetPosition();
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            Vector2I centerPos = screenPos + (screenSize / 2) - (newSize / 2);
            centerPos.Y = Mathf.Max(0, centerPos.Y);
            mainWindow.Position = centerPos;
        }
    }

    private void OnWindowResized()
    {
        // When the window resizes, the UI takes a split second to calculate the new centered layout.
        // We use CallDeferred to wait until that math is done before snapping the cursor.
        CallDeferred(MethodName.UpdateCursorPosition);
    }

    private void UpdateCursorPosition()
    {
        // Find whichever control the player is currently highlighting
        Control focusedNode = GetViewport().GuiGetFocusOwner();
        
        if (focusedNode != null)
        {
            if (_confirmPrompt.Visible)
            {
                MovePromptCursorTo(focusedNode);
            }
            else
            {
                MoveCursorTo(focusedNode);
            }
        }
    }

    // ==========================================
    // PROMPT RESPONSES
    // ==========================================
    private void OnPromptYes()
    {
        if (_activeSubMenu == MenuState.Audio) SaveAudio();
        else if (_activeSubMenu == MenuState.Video) SaveVideo();
        ForceCloseSubMenu();
    }

    private void OnPromptNo()
    {
        if (_activeSubMenu == MenuState.Audio)
        {
            LoadSavedAudioToUI();
            // Sliders automatically re-apply live volume in their property setter
        }
        else if (_activeSubMenu == MenuState.Video)
        {
            LoadSavedVideoToUI();
            ApplyVideoSettingsToServer(); // Revert screen back to saved state
        }
        ForceCloseSubMenu();
    }

    private void OnPromptCancel()
    {
        _promptCursor.Hide();
        _confirmPrompt.Hide();
        _cursorIcon.Show();

        if (_activeSubMenu == MenuState.Audio) _audioBackButton.CallDeferred(Control.MethodName.GrabFocus);
        else if (_activeSubMenu == MenuState.Video) _videoBackButton.CallDeferred(Control.MethodName.GrabFocus);
    }

    private void OnKeyboardPressed() { GD.Print("Keyboard Settings Not Implemented"); }
    private void OnControllerPressed() { GD.Print("Controller Settings Not Implemented"); }
}