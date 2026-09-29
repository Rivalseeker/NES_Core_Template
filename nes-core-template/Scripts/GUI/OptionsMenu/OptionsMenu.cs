using Godot;
using System;

public partial class OptionsMenu : Control
{
    private enum MenuState { Main, Keyboard, Audio, Video }
    private MenuState _activeSubMenu = MenuState.Main;

    // ==========================================
    // NODE REFERENCES
    // ==========================================
    [ExportCategory("Menu Containers")]
    [Export] private VBoxContainer _optionsList;
    [Export] private VBoxContainer _keyboardList;
    [Export] private VBoxContainer _audioList;
    [Export] private VBoxContainer _videoList;

    [ExportCategory("Main Options Nodes")]
    [Export] private Button _keyboardButton;
    [Export] private Button _controllerButton;
    [Export] private Button _audioButton;
    [Export] private Button _videoButton;
    [Export] private Button _backButton;

    [ExportCategory("Keyboard Nodes")]
    [Export] private Control _upAction;
    [Export] private Button _keyboardDefaultsButton;
    [Export] private Button _keyboardBackButton;

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
    [Export] private RetroSelector _crtSelector;
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
    private int _previousWindowScaleIndex;

    // ==========================================
    // INITIALIZATION
    // ==========================================
    public override void _Ready()
    {
        GenerateResolutionOptions();

        // Ensure HBoxContainer can participate in UI Focus
        if (_upAction != null)
        {
            _upAction.FocusMode = FocusModeEnum.All;
        }

        // 1. Wire Button Presses
        _keyboardButton.Pressed += () => OpenSubMenu(MenuState.Keyboard);
        _audioButton.Pressed += () => OpenSubMenu(MenuState.Audio);
        _videoButton.Pressed += () => OpenSubMenu(MenuState.Video);
        _backButton.Pressed += CloseOptionsOverlay;

        _keyboardBackButton.Pressed += AttemptCloseSubMenu;
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

        // 3. Wire Cursor Tracking (Keyboard List)
        if (_upAction != null) _upAction.FocusEntered += () => MoveCursorTo(_upAction);
        if (_keyboardDefaultsButton != null) _keyboardDefaultsButton.FocusEntered += () => MoveCursorTo(_keyboardDefaultsButton);
        if (_keyboardBackButton != null) _keyboardBackButton.FocusEntered += () => MoveCursorTo(_keyboardBackButton);

        // 4. Wire Cursor Tracking (Audio List)
        _masterVolSlider.FocusEntered += () => MoveCursorTo(_masterVolSlider);
        _musicVolSlider.FocusEntered += () => MoveCursorTo(_musicVolSlider);
        _sfxVolSlider.FocusEntered += () => MoveCursorTo(_sfxVolSlider);
        _speakerSelector.FocusEntered += () => MoveCursorTo(_speakerSelector);
        _audioBackButton.FocusEntered += () => MoveCursorTo(_audioBackButton);

        // 5. Wire Cursor Tracking (Video List)
        _windowSizeSelector.FocusEntered += () => MoveCursorTo(_windowSizeSelector);
        _fullscreenSelector.FocusEntered += () => MoveCursorTo(_fullscreenSelector);
        _vsyncSelector.FocusEntered += () => MoveCursorTo(_vsyncSelector);
        _flickerSelector.FocusEntered += () => MoveCursorTo(_flickerSelector);
        _pixelMovementSelector.FocusEntered += () => MoveCursorTo(_pixelMovementSelector);
        _crtSelector.FocusEntered += () => MoveCursorTo(_crtSelector);
        _videoBackButton.FocusEntered += () => MoveCursorTo(_videoBackButton);

        // 6. Wire Cursor Tracking (Prompt)
        _promptYesButton.FocusEntered += () => MovePromptCursorTo(_promptYesButton);
        _promptNoButton.FocusEntered += () => MovePromptCursorTo(_promptNoButton);
        _promptCancelButton.FocusEntered += () => MovePromptCursorTo(_promptCancelButton);

        // 7. Live Video Event Listeners
        _windowSizeSelector.OnValueChanged += OnWindowSizeChanged;
        _fullscreenSelector.OnValueChanged += OnFullscreenChanged;
        _vsyncSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();
        _flickerSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();
        _pixelMovementSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();
        _crtSelector.OnValueChanged += (index) => ApplyVideoSettingsToServer();

        // 8. Focus Looping (Wrap-Around)
        _keyboardButton.FocusNeighborTop = _backButton.GetPath();
        _backButton.FocusNeighborBottom = _keyboardButton.GetPath();

        // Keyboard List Focus Neighbor Loop: UpAction <> KeyboardDefaultsButton <> KeyboardBackButton <> UpAction
        if (_upAction != null && _keyboardDefaultsButton != null && _keyboardBackButton != null)
        {
            _upAction.FocusNeighborTop = _keyboardBackButton.GetPath();
            _upAction.FocusNeighborBottom = _keyboardDefaultsButton.GetPath();

            _keyboardDefaultsButton.FocusNeighborTop = _upAction.GetPath();
            _keyboardDefaultsButton.FocusNeighborBottom = _keyboardBackButton.GetPath();

            _keyboardBackButton.FocusNeighborTop = _keyboardDefaultsButton.GetPath();
            _keyboardBackButton.FocusNeighborBottom = _upAction.GetPath();
        }

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
        _keyboardList.Hide();
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

        if (menu == MenuState.Keyboard)
        {
            _keyboardList.Show();
            _upAction.CallDeferred(Control.MethodName.GrabFocus);
        }
        else if (menu == MenuState.Audio)
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
        bool hasChanged = false;

        if (_activeSubMenu == MenuState.Audio) hasChanged = HasAudioChanged();
        else if (_activeSubMenu == MenuState.Video) hasChanged = HasVideoChanged();
        // Add keyboard key binding change check here in the future if applicable

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
        
        _keyboardList.Hide();
        _audioList.Hide();
        _videoList.Hide();
        _optionsList.Show();

        if (_activeSubMenu == MenuState.Keyboard) _keyboardButton.CallDeferred(Control.MethodName.GrabFocus);
        else if (_activeSubMenu == MenuState.Audio) _audioButton.CallDeferred(Control.MethodName.GrabFocus);
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
        _previousWindowScaleIndex = scaleIndex;
        
        _windowSizeSelector.SetValueNoNotify(scaleIndex);
        _fullscreenSelector.SetValueNoNotify(d.Fullscreen ? 1 : 0);
        _vsyncSelector.SetValueNoNotify(d.VSync ? 1 : 0);
        _flickerSelector.SetValueNoNotify(d.SpriteFlickerMode);
        _pixelMovementSelector.SetValueNoNotify(d.PixelMovement ? 1 : 0);
        _crtSelector.SetValueNoNotify(d.CRT_Filter ? 1 : 0);
    }

    private bool HasVideoChanged()
    {
        if (SaveManager.Instance == null) return false;
        GameData d = SaveManager.Instance.CurrentGameData;
        int targetScale = _windowSizeSelector.CurrentIndex + 1;
        bool isFullscreen = _fullscreenSelector.CurrentIndex == 1;
        bool isVsync = _vsyncSelector.CurrentIndex == 1;
        int currentFlicker = _flickerSelector.CurrentIndex;
        bool isPixelMovement = _pixelMovementSelector.CurrentIndex == 1;
        bool isCrt = _crtSelector.CurrentIndex == 1;

        return targetScale != d.WindowScaleMultiplier || isFullscreen != d.Fullscreen || isVsync != d.VSync || currentFlicker != d.SpriteFlickerMode || isPixelMovement != d.PixelMovement || isCrt != d.CRT_Filter;
    }

    private void SaveVideo()
    {
        if (SaveManager.Instance == null) return;
        GameData d = SaveManager.Instance.CurrentGameData;
        d.WindowScaleMultiplier = _windowSizeSelector.CurrentIndex + 1;
        d.Fullscreen = _fullscreenSelector.CurrentIndex == 1;
        d.VSync = _vsyncSelector.CurrentIndex == 1;

        d.SpriteFlickerMode = _flickerSelector.CurrentIndex;
        d.PixelMovement = _pixelMovementSelector.CurrentIndex == 1;
        d.CRT_Filter = _crtSelector.CurrentIndex == 1;
        
        SaveManager.Instance.SaveGameData();

        if (NESSpriteManager.Instance != null)
        {
            NESSpriteManager.Instance.CurrentFlickerMode = (NESSpriteManager.FlickerMode)_flickerSelector.CurrentIndex;
        }
    }

    private void OnWindowSizeChanged(int newIndex)
    {
        if (newIndex == _maxWindowScale - 1)
        {
            _fullscreenSelector.SetValueNoNotify(1);
        }
        else
        {
            _fullscreenSelector.SetValueNoNotify(0);
            _previousWindowScaleIndex = newIndex;
        }

        ApplyVideoSettingsToServer();
    }

    private void OnFullscreenChanged(int newIndex)
    {
        bool isFullscreen = newIndex == 1;

        if (isFullscreen)
        {
            if (_windowSizeSelector.CurrentIndex != _maxWindowScale - 1)
            {
                _previousWindowScaleIndex = _windowSizeSelector.CurrentIndex;
            }
            _windowSizeSelector.SetValueNoNotify(_maxWindowScale - 1);
        }
        else
        {
            int restoreIndex = _previousWindowScaleIndex;
            
            if (restoreIndex == _maxWindowScale - 1) 
            {
                restoreIndex = Mathf.Max(0, _maxWindowScale - 2); 
            }
            
            _windowSizeSelector.SetValueNoNotify(restoreIndex);
        }

        ApplyVideoSettingsToServer();
    }

    private void ApplyVideoSettingsToServer()
    {
        bool isFullscreen = _fullscreenSelector.CurrentIndex == 1;
        bool isVsync = _vsyncSelector.CurrentIndex == 1;
        int scale = _windowSizeSelector.CurrentIndex + 1;
        bool isPixelMovement = _pixelMovementSelector.CurrentIndex == 1;
        bool isCrt = _crtSelector.CurrentIndex == 1;

        if (NESSpriteManager.Instance != null)
        {
            NESSpriteManager.Instance.CurrentFlickerMode = (NESSpriteManager.FlickerMode)_flickerSelector.CurrentIndex;
        }

        DisplayServer.WindowSetVsyncMode(isVsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);

        Window mainWindow = GetTree().Root;
        if (mainWindow.IsEmbedded()) return;

        Viewport viewport = GetViewport();
        viewport.Snap2DTransformsToPixel = isPixelMovement;

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

        CanvasLayer crtFilter = GetTree().Root.FindChild("CRT_Layer", true, false) as CanvasLayer;
        
        if (crtFilter != null)
        {
            crtFilter.Visible = isCrt;
        }
        else
        {
            GD.PrintErr("OptionsMenu: CRTLayer could not be found in the active Scene Tree.");
        }
    }

    private void OnWindowResized()
    {
        CallDeferred(MethodName.UpdateCursorPosition);
    }

    private void UpdateCursorPosition()
    {
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
        }
        else if (_activeSubMenu == MenuState.Video)
        {
            LoadSavedVideoToUI();
            ApplyVideoSettingsToServer();
        }
        ForceCloseSubMenu();
    }

    private void OnPromptCancel()
    {
        _promptCursor.Hide();
        _confirmPrompt.Hide();
        _cursorIcon.Show();

        if (_activeSubMenu == MenuState.Keyboard) _keyboardBackButton.CallDeferred(Control.MethodName.GrabFocus);
        else if (_activeSubMenu == MenuState.Audio) _audioBackButton.CallDeferred(Control.MethodName.GrabFocus);
        else if (_activeSubMenu == MenuState.Video) _videoBackButton.CallDeferred(Control.MethodName.GrabFocus);
    }

    private void OnControllerPressed() { GD.Print("Controller Settings Not Implemented"); }
}