using Godot;

public partial class DebugLevelSelect : Control
{
    [Export] private Godot.Collections.Array<DebugLevelData> _levels;
    [Export] private VBoxContainer _levelListContainer;
    [Export] private VBoxContainer _checkpointListContainer;

    [Export] private PackedScene _buttonPrefab;

    private Button _lastSelectedLevelButton;

    private static readonly Color LightBlueFocusColor = new Color(0.0f, 0.89f, 0.98f, 1.0f); // #00E4FC

    public override void _Ready()
    {
        PopulateLevelList();
        
        if (_levelListContainer.GetChildCount() > 0)
        {
            (_levelListContainer.GetChild(0) as Button)?.GrabFocus();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Cancel"))
        {
            AcceptEvent();
            Control focusedNode = GetViewport().GuiGetFocusOwner();

            if (focusedNode != null && _checkpointListContainer.IsAncestorOf(focusedNode))
            {
                ClearCheckpoints();
                _lastSelectedLevelButton?.GrabFocus();
            }
            else
            {
                SceneManager.Instance.CloseMenuOverlay();
            }
        }
    }

    private void PopulateLevelList()
    {
        if (_levels == null) return;

        foreach (DebugLevelData levelData in _levels)
        {
            Button levelBtn = _buttonPrefab?.Instantiate<Button>();
            if (levelBtn == null) return;

            levelBtn.Text = levelData.LevelName;
            levelBtn.Alignment = HorizontalAlignment.Left;

            levelBtn.AddThemeColorOverride("font_focus_color", LightBlueFocusColor);
            levelBtn.AddThemeColorOverride("font_hover_color", LightBlueFocusColor);

            levelBtn.Pressed += () => OnLevelSelected(levelBtn, levelData);
            _levelListContainer.AddChild(levelBtn);
        }

        // Apply wrap-around focus for the level list
        SetupFocusNeighbors(_levelListContainer);
    }

    private void OnLevelSelected(Button levelBtn, DebugLevelData levelData)
    {
        _lastSelectedLevelButton = levelBtn;
        ClearCheckpoints();

        // Generate "Start" (ID: 0)
        CreateCheckpointButton("Start", 0, levelData.ScenePath);

        // Generate sequential checkpoints (ID: 1, 2, 3...)
        for (int i = 1; i <= levelData.CheckpointCount; i++)
        {
            CreateCheckpointButton($"CP {i}", i, levelData.ScenePath);
        }

        // Apply wrap-around focus for the checkpoint list
        SetupFocusNeighbors(_checkpointListContainer);

        // Shift focus to the first item (Start) in the checkpoint list
        if (_checkpointListContainer.GetChildCount() > 0)
        {
            (_checkpointListContainer.GetChild(0) as Button)?.GrabFocus();
        }
    }

    private void CreateCheckpointButton(string label, int checkpointId, string scenePath)
    {
        Button cpBtn = _buttonPrefab?.Instantiate<Button>();
        if (cpBtn == null) return;

        cpBtn.Text = label;
        cpBtn.Alignment = HorizontalAlignment.Left;

        cpBtn.AddThemeColorOverride("font_focus_color", LightBlueFocusColor);
        cpBtn.AddThemeColorOverride("font_hover_color", LightBlueFocusColor);

        cpBtn.Pressed += () => OnCheckpointSelected(checkpointId, scenePath);
        _checkpointListContainer.AddChild(cpBtn);
    }

    private void OnCheckpointSelected(int checkpointId, string scenePath)
    {
        SessionState.ActiveCheckpointId = checkpointId; 
        SceneManager.Instance.CloseMenuOverlay();
        SceneManager.Instance.LoadSceneAsync(scenePath);
    }

    private void ClearCheckpoints()
    {
        foreach (Node child in _checkpointListContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    /// <summary>
    /// Links the top item's UP neighbor to the bottom item, 
    /// and the bottom item's DOWN neighbor to the top item.
    /// </summary>
    private void SetupFocusNeighbors(VBoxContainer container)
    {
        int count = container.GetChildCount();
        if (count <= 1) return;

        Button firstBtn = container.GetChild<Button>(0);
        Button lastBtn = container.GetChild<Button>(count - 1);

        if (firstBtn != null && lastBtn != null)
        {
            firstBtn.FocusNeighborTop = lastBtn.GetPath();
            lastBtn.FocusNeighborBottom = firstBtn.GetPath();
        }
    }
}