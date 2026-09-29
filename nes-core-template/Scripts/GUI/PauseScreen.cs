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
        // if (@event.IsActionPressed("Pause")) // Or your dedicated "pause" action
        // {
        //     TogglePause();
        //     GetViewport().SetInputAsHandled();
        // }
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
}