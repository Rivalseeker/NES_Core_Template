using Godot;

public partial class PauseScreen : ColorRect
{
    public override void _Ready()
    {
        // Ensure it starts hidden
        Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Ensure you have an action named "Pause" mapped in Project Settings -> Input Map
        if (@event.IsActionPressed("Pause")) 
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }
    }

    public void TogglePause()
    {
        bool newPauseState = !GetTree().Paused;
        GetTree().Paused = newPauseState;
        Visible = newPauseState;

        if (newPauseState)
        {
            // Optional: Grab focus for menu items if pause screen has buttons
            // GetNode<Button>("VBoxContainer/ResumeButton")?.GrabFocus();
        }
    }
    
    // NEW: Example method for when a player clicks "Quit to Main Menu" from the pause screen
    public void QuitToMainMenu(string mainMenuPath)
    {
        // CRITICAL: You must unpause the game before leaving, otherwise the Main Menu will be frozen!
        GetTree().Paused = false; 
        
        SceneManager.Instance.LoadSceneAsync(mainMenuPath);
    }
}