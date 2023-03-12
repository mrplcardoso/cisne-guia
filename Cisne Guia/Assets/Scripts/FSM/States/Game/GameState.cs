using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : MachineState
{
	public GameMachine gameMachine
	{ get { return (GameMachine)stateMachine; } }
}
