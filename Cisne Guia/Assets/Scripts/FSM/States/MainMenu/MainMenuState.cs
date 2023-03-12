using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MainMenuState : MachineState
{
	public MainMenuMachine mainMenuMachine
	{ get { return (MainMenuMachine)stateMachine; } }

	public List<ImageButtonPair> cutedParts;
}
