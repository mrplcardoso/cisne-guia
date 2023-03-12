using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageElement : UIElement
{
	public Image imageComponent
	{ get; private set; }

	private void Awake()
	{
		imageComponent = GetComponent<Image>();
	}
}
