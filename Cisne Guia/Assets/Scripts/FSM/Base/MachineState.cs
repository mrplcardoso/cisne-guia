using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State of FSM Class.
/// </summary>
public abstract class MachineState : MonoBehaviour
{
	/// <summary>
	/// StateMachine owner.
	/// </summary>
	protected StateMachine machine;

	/// <summary>
	/// Get FSM that own this state.
	/// Set FSM when there is no FSM owner.
	/// </summary>
	public StateMachine stateMachine
	{
		get { return machine; }
		set { if (machine == null) machine = value; }
	}

	/// <summary>
	/// Execute a routine when machine enter in this state.
	/// </summary>
	public virtual void OnEnter()
	{ }

	/// <summary>
	/// Execute a routine when machine leave this state.
	/// </summary>
	public virtual void OnExit()
	{ }
}
