using Godot;
using Godot.Collections;

public partial class Player_FixedMoveAndFixedJump_StateMachine : Node
{
    [Export] private NodePath _initialStatePath;
    
    // NEW: We need a reference to the body we are controlling
    [Export] private PlayerFixedMoveAndFixedJump _playerFixedMoveAndFixedJump; 

    private Player_FixedMoveAndJump_State _playerFixedMoveAndFixedJumpState;
    private Dictionary<StringName, Player_FixedMoveAndJump_State> _states = new Dictionary<StringName, Player_FixedMoveAndJump_State>();

    // NEW: Public getter so other scripts can safely read the active state without modifying it
    public Player_FixedMoveAndJump_State CurrentFixedMoveAndJumpState => _playerFixedMoveAndFixedJumpState;

    public override void _Ready()
    {
        // Auto-fetch the parent if we forgot to assign it in the Inspector
        if (_playerFixedMoveAndFixedJump == null)
        {
            _playerFixedMoveAndFixedJump = GetParent<PlayerFixedMoveAndFixedJump>();
        }

        // Loop through all child nodes to find our States
        foreach (Node child in GetChildren())
        {
            if (child is Player_FixedMoveAndJump_State _playerFMFJ_State)
            {
                // Store the state in a dictionary using its node name
                _states[_playerFMFJ_State.Name] = _playerFMFJ_State;
                
                // NEW: Give the state a reference to the player body!
                _playerFMFJ_State.playerFMFJ = _playerFixedMoveAndFixedJump;
                
                // Connect the transition signal
                _playerFMFJ_State.Transitioned += OnChildTransitioned;
            }
        }

        // Initialize the starting state
        if (_initialStatePath != null)
        {
            _playerFixedMoveAndFixedJumpState = GetNode<Player_FixedMoveAndJump_State>(_initialStatePath);
            _playerFixedMoveAndFixedJumpState.Enter();
        }
        else if (GetChildCount() > 0 && GetChild(0) is Player_FixedMoveAndJump_State firstState)
        {
            // Fallback: If no initial state is set, just grab the first child
            _playerFixedMoveAndFixedJumpState = firstState;
            _playerFixedMoveAndFixedJumpState.Enter();
        }
    }

    public override void _Process(double delta)
    {
        _playerFixedMoveAndFixedJumpState?.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _playerFixedMoveAndFixedJumpState?.PhysicsUpdate(delta);
    }

    private void OnChildTransitioned(Player_FixedMoveAndJump_State state, StringName newStateName)
    {
        // Ignore transitions from states that aren't currently active
        if (state != _playerFixedMoveAndFixedJumpState) return;
        
        // SAFETY NET: If the state doesn't exist, print a massive error to the console!
        if (!_states.TryGetValue(newStateName, out Player_FixedMoveAndJump_State newState)) 
        {
            GD.PrintErr($"[StateMachine Error] '{_playerFixedMoveAndFixedJumpState.Name}' tried to transition to '{newStateName}', but no Node with that exact name exists in the StateMachine!");
            return;
        }

        _playerFixedMoveAndFixedJumpState.Exit();
        newState.Enter();
        _playerFixedMoveAndFixedJumpState = newState;
    }

    // NEW: Allows external scripts (like BasePlayer) to hijack the active state!
    public void ForceTransition(StringName newStateName)
    {
        if (!_states.TryGetValue(newStateName, out Player_FixedMoveAndJump_State newState)) 
        {
            GD.PrintErr($"[StateMachine Error] Tried to force transition to '{newStateName}', but no Node with that exact name exists!");
            return;
        }

        _playerFixedMoveAndFixedJumpState?.Exit();
        newState.Enter();
        _playerFixedMoveAndFixedJumpState = newState;
    }
}
