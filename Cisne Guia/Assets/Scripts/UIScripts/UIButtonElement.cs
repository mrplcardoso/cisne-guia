using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonElement : UIElement
{
	public UIButtonType buttonType;
	public Button buttonComponent
	{ get; private set; }

	private void Awake()
	{
		buttonComponent = GetComponent<Button>();
	}
}
