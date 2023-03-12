using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finite-State Machine Class. 
/// Maintain states as components through a child object.
/// </summary>
public abstract class StateMachine : MonoBehaviour
{
	/// <summary>
	/// List of current states available.
	/// </summary>
	[SerializeField]
	protected List<MachineState> states;

	/// <summary>
	/// GameObject that hold states as components.
	/// </summary>
	[SerializeField]
	protected GameObject statesHolder;

	/// <summary>
	/// Current state running.
	/// </summary>
	[SerializeField]
	protected MachineState currentState;

	/// <summary>
	/// Get current state executing.
	/// </summary>
	public virtual MachineState runningState
	{ get { return currentState; } }

	/// <summary>
	/// Indicates if machine is transitioning between states.
	/// </summary>
	public bool inTransition { get; private set; }

	protected virtual void Awake()
	{
		BuildList();
	}

	/// <summary>
	/// Create states' holder, if there is no child.
	/// Otherwise, get states' holder reference and build states' list.
	/// </summary>
	protected virtual void BuildList()
	{
		states = new List<MachineState>();
		if (GetComponentInChildren<MachineState>() != null)
		{
			statesHolder = GetComponentInChildren<MachineState>().gameObject;
			MachineState[] s = statesHolder.GetComponents<MachineState>();
			for (int i = 0; i < s.Length; ++i)
			{
				s[i].stateMachine = this;
				states.Add(s[i]);
			}
		}
		else
		{
			statesHolder = new GameObject();
			statesHolder.transform.parent = gameObject.transform;
		}
	}

	/// <summary>
	/// Add the given state as a component. Inserts in the list and holder.
	/// </summary>
	/// <typeparam name="T">Type of given state to add.</typeparam>
	/// <returns>If successful, return the state just added.</returns>
	public virtual T AddState<T>() where T : MachineState
	{
		if (statesHolder == null) { return null; }
		T s = statesHolder.GetComponent<T>();
		if (s == null)
		{
			s = statesHolder.AddComponent<T>();
			s.stateMachine = this;
			states.Add(s);
		}
		return s;
	}

	/// <summary>
	/// Remove a state from machine by removing from list and destroing the component.
	/// Only destroy states that aren't the current one running, and outside transitions.
	/// </summary>
	/// <typeparam name="T">State to destroy.</typeparam>
	/// <returns>True if state was destroied.</returns>
	public virtual bool RemoveState<T>() where T : MachineState
	{
		if (statesHolder == null) { return false; }
		if (inTransition) { return false; }

		T s = statesHolder.GetComponent<T>();
		if (s == null) { return false; }
		if (s == currentState) { return false; }

		if (states.Contains(s)) { states.Remove(s); }
		Destroy(s);
		return true;
	}

	/// <summary>
	/// Check if the machine has a given state. 
	/// If it has, returns the state as component, if not, return null.
	/// </summary>
	/// <typeparam name="T">State to search for.</typeparam>
	/// <param name="includeDisabled">If false, return only enabled state.</param>
	/// <returns>Return state as component, if it exists, or null, if not.</returns>
	public virtual T HasState<T>(bool includeDisabled = true) where T : MachineState
	{
		if (statesHolder == null) { return null; }
		T s = statesHolder.GetComponent<T>();
		if (!includeDisabled)
		{
			if (!s.enabled) { return null; }
		}
		return s;
	}

	/// <summary>
	/// Make transition between two states immediately.
	/// </summary>
	/// <typeparam name="T">\Next state to enter.</typeparam>
	/// <param name="addState">If true, when machine don't have the given state, add it as new. 
	/// If false, method understands the next state is already in list/holder.</param>
	/// <param name="setEnabled">If true, when "addState" is false, 
	/// machine changes to enabled and disabled states (activating it).
	///<br>If false, machine will change only to enabled states.</br></param>
	public virtual void ChangeStateImmediate<T>(bool addState = true, bool setEnabled = false) where T : MachineState
	{
		T s = (addState ? AddState<T>() : HasState<T>(setEnabled));
		if (s == null || s == currentState || inTransition)
		{ return; }

		if (setEnabled)
		{
			if (!states.Contains(s)) { states.Add(s); }
			s.enabled = true;
		}

		inTransition = true;
		if (currentState != null)
		{ currentState.OnExit(); }
		currentState = s;

		inTransition = false;
		if (currentState != null)
		{ currentState.OnEnter(); }
	}

	/// <summary>
	/// Make transition between two states with a interval time.
	/// </summary>
	/// <typeparam name="T">\Next state to enter.</typeparam>
	/// <param name="intervalTime">Interval seconds between previous state's OnExit() method and
	/// next state's OnEnter() method.</param>
	/// <param name="addState">If true, when machine don't have the given state, add it as new. 
	/// If false, method understands the next state is already in list/holder.</param>
	/// <param name="setEnabled">If true, when "addState" is false, 
	/// machine changes to enabled and disabled states (activating it).
	///<br>If false, machine will change only to enabled states.</br></param>
	public virtual void ChangeStateIntervaled<T>(float intervalTime = 0, bool addState = true, bool setEnabled = false) where T : MachineState
	{
		T s = (addState ? AddState<T>() : HasState<T>(setEnabled));
		if (s == null || s == currentState || inTransition)
		{ return; }

		if (setEnabled)
		{
			if (!states.Contains(s)) { states.Add(s); }
			s.enabled = true;
		}
		inTransition = true;

		if (currentState != null)
		{ currentState.OnExit(); }
		currentState = s;

		if (currentState != null)
		{ StartCoroutine(ChangeInterval(currentState, intervalTime)); }
		else { inTransition = false; }
	}

	/// <summary>
	/// Coroutine that runs the OnEnter() method of next state.
	/// </summary>
	/// <param name="nextState">Next state to enter.</param>
	/// <param name="intervalTime">Interval seconds between previous state's OnExit() method and
	/// next state's OnEnter() method.</param>
	/// <returns></returns>
	protected virtual IEnumerator ChangeInterval(MachineState nextState, float intervalTime)
	{
		if(intervalTime <= 0) { yield return null; }
		else { yield return new WaitForSeconds(intervalTime); }

		inTransition = false;
		nextState.OnEnter();
	}

	/// <summary>
	/// Disable a given state by removing it from list and disabling it component.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns>Returns the given state as a component.</returns>
	public virtual T DisableState<T>() where T : MachineState
	{
		T s = HasState<T>();
		if (s == null) return null;
		if (!states.Contains(s)) return null;
		states.Remove(s);
		s.enabled = false;
		return s;
	}

	/// <summary>
	/// Enable a given state by adding it into the list and enabling it component.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns>Returns the given state as a component.</returns>
	public virtual T EnableState<T>() where T : MachineState
	{
		T s = HasState<T>();
		if (s == null) return null;
		if (!states.Contains(s)) return null;
		states.Add(s);
		s.enabled = true;
		return s;
	}
}
