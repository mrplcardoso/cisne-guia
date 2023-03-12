using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMachine : StateMachine
{
	private void Start()
	{
		ChangeStateIntervaled<StartMenuState>(0.02f);
	}
}
