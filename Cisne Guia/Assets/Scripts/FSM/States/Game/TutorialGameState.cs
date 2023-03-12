using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGameState : GameState
{
	public Image tutorialScreen;
	public Button tutorialButton;
	Image buttonImage;
	Text buttonText;

	private void Awake()
	{
		tutorialScreen.color = Color.clear;
		
		ColorBlock c = tutorialButton.colors;
		c.normalColor = Color.clear;
		tutorialButton.colors = c;

		buttonImage = tutorialButton.GetComponent<Image>();
		buttonImage.color = Color.clear;

		buttonText = tutorialButton.GetComponentInChildren<Text>();
		buttonText.color = Color.clear;

		Button.ButtonClickedEvent click = new Button.ButtonClickedEvent();
		click.AddListener(delegate ()
		{
			StartCoroutine(DisposeTutorial());
		});
		tutorialButton.onClick = click;
	}

	public override void OnEnter()
	{
		StartCoroutine(ShowTutorial());
	}

	IEnumerator DisposeTutorial()
	{
		yield return HideTutorial();
		gameMachine.ChangeStateImmediate<UpdateGameState>();
	}

	IEnumerator ShowTutorial()
	{
		float t = 0;
		Color start = tutorialScreen.color;
		Color end = Color.white;

		ColorBlock c = ColorBlock.defaultColorBlock;

		while (t < 1.01f)
		{
			tutorialScreen.color = Color.Lerp(start, end, t);

			c.normalColor = Color.Lerp(start, end, t);
			tutorialButton.colors = c;

			buttonImage.color = Color.Lerp(start, end, t);

			buttonText.color = Color.Lerp(start, Color.black, t);

			t += 1f * Time.deltaTime;
			yield return null;
		}

		tutorialScreen.color = end;
		c.normalColor = end;
		tutorialButton.colors = c;
		buttonImage.color = end;
		buttonText.color = Color.black;
	}

	IEnumerator HideTutorial()
	{
		float t = 0;
		Color start = tutorialScreen.color;
		Color end = Color.clear;
		
		ColorBlock c = ColorBlock.defaultColorBlock;

		while (t < 1.01f)
		{
			tutorialScreen.color = Color.Lerp(start, end, t);
			c.normalColor = Color.Lerp(start, end, t);
			tutorialButton.colors = c;
			buttonImage.color = Color.Lerp(start, end, t);
			buttonText.color = Color.Lerp(start, end, t);

			t += 1f * Time.deltaTime;
			yield return null;
		}

		tutorialScreen.color = end;
		c.normalColor = end;
		tutorialButton.colors = c;
		buttonImage.color = end;
		buttonText.color = end;
	}
}
