using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuState : MainMenuState
{
	private void Start()
	{
		for(int i = 0; i < cutedParts.Count; ++i)
		{
			cutedParts[i].image.imageComponent.rectTransform.localPosition = cutedParts[i].image.outScenePosition;

			if (cutedParts[i].button == null) continue;
			cutedParts[i].button.buttonComponent.transform.localPosition = cutedParts[i].button.outScenePosition;

			switch ((int)cutedParts[i].button.buttonType)
			{
				case 0: //Start
				{
					Button.ButtonClickedEvent click = new Button.ButtonClickedEvent();
					click.AddListener(delegate ()
					{
						StartCoroutine(ChangeScene("Game"));
					});
					cutedParts[i].button.buttonComponent.onClick = click;
					break;
				}
				case 1: //Credit
				{
					Button.ButtonClickedEvent click = new Button.ButtonClickedEvent();
					click.AddListener(delegate ()
					{
						mainMenuMachine.ChangeStateImmediate<CreditMenuState>();
						//StartCoroutine(CutOut());
					});
					cutedParts[i].button.buttonComponent.onClick = click;
					break;
				}
			}
		}
	}

	public override void OnEnter()
	{
		for (int i = 0; i < cutedParts.Count; ++i)
		{
			cutedParts[i].image.imageComponent.rectTransform.localPosition = cutedParts[i].image.inScenePosition;
			cutedParts[i].button.buttonComponent.transform.localPosition = cutedParts[i].button.inScenePosition;
		}
	}

	public override void OnExit()
	{
		for (int i = 0; i < cutedParts.Count; ++i)
		{
			cutedParts[i].image.imageComponent.rectTransform.SetAsLastSibling();
		}
		for (int i = 0; i < cutedParts.Count; ++i)
		{
			if (cutedParts[i].button == null) continue;
			cutedParts[i].button.buttonComponent.transform.SetAsLastSibling();
		}
		StartCoroutine(CutOut());
	}

	IEnumerator CutOut()
	{
		yield return null;

		float t = 0;
		while(t <= 1.01f)
		{
			for(int i = 0; i < cutedParts.Count; ++i)
			{
				cutedParts[i].image.imageComponent.rectTransform.localPosition =
					Vector2.Lerp(cutedParts[i].image.inScenePosition, cutedParts[i].image.outScenePosition, t);
				cutedParts[i].button.buttonComponent.transform.localPosition =
					Vector2.Lerp(cutedParts[i].button.inScenePosition, cutedParts[i].button.outScenePosition, t);
				t += 0.5f * Time.deltaTime;
			}
			yield return null;
		}
	}

	IEnumerator ChangeScene(string name)
	{
		yield return CutOut();
		SceneManager.LoadScene(name);
	}
}
