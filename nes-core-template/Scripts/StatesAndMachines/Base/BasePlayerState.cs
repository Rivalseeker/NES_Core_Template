using Godot;

// Base class for all player states (Idle, GroundMove, Jump, Fall, ...).
// Attach a script that inherits this to each state node.
public partial class BasePlayerState : Node
{
    // [Signal]
    // public delegate void TransitionedEventHandler(RunJumpShootPlayerState state, StringName newStateName);

    // // The StateMachine will populate this automatically on _Ready!
    // public BasePlayer basePlayer;

    // public virtual void Enter() { }
    // public virtual void Exit() { }
    // public virtual void Update(double delta) { }
    // public virtual void PhysicsUpdate(double delta) { }
}