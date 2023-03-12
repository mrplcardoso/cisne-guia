using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditMenuState : MainMenuState
{
	private void Start()
	{
		for (int i = 0; i < cutedParts.Count; ++i)
		{
			cutedParts[i].image.imageComponent.rectTransform.localPosition = cutedParts[i].image.outScenePosition;

			if (cutedParts[i].button == null) continue;
			cutedParts[i].button.buttonComponent.transform.localPosition = cutedParts[i].button.outScenePosition;

			switch ((int)cutedParts[i].button.buttonType)
			{
				case 2: //Back
				{
					Button.ButtonClickedEvent click = new Button.ButtonClickedEvent();
					click.AddListener(delegate ()
					{
						mainMenuMachine.ChangeStateImmediate<StartMenuState>();
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

			if (cutedParts[i].button == null) continue;
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
		while (t <= 1.01f)
		{
			for (int i = 0; i < cutedParts.Count; ++i)
			{
				cutedParts[i].image.imageComponent.rectTransform.localPosition =
					Vector2.Lerp(cutedParts[i].image.inScenePosition, cutedParts[i].image.outScenePosition, t);

				if (cutedParts[i].button != null)
				{
					cutedParts[i].button.buttonComponent.transform.localPosition =
					Vector2.Lerp(cutedParts[i].button.inScenePosition, cutedParts[i].button.outScenePosition, t);
				}

				t += 0.5f * Time.deltaTime;
			}
			yield return null;
		}
	}
}
