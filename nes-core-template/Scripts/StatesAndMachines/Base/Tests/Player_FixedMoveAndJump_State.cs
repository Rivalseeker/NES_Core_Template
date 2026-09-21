using Godot;
using System;

public partial class Player_FixedMoveAndJump_State : Node
{
    [Signal]
    public delegate void TransitionedEventHandler(Player_FixedMoveAndJump_State state, StringName newStateName);

    // The StateMachine will populate this automatically on _Ready!
    public PlayerFixedMoveAndFixedJump playerFMFJ;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }
    public virtual void PhysicsUpdate(double delta) { }
}
