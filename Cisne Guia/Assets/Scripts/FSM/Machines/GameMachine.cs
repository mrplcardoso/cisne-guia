using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMachine : StateMachine
{
	private void Start()
	{
		ChangeStateIntervaled<TutorialGameState>(0.05f);
	}
}
