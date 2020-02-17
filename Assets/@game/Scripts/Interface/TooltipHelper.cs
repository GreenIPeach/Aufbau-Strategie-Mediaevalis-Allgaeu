using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHelper : MonoBehaviour
{
	private bool active = false;
	private Vector3 offsetUpper = new Vector3(-75,45);
	private Vector3 offsetLower = new Vector3(75,-25);
	[SerializeField]
	private TextMeshProUGUI infoText;

	private void Update()
	{
		if (active)
		{
			if(Input.mousePosition.y >= (Screen.height/3)*2)
			{
				gameObject.transform.position = Input.mousePosition - offsetUpper ;
			}
			else
			{
				gameObject.transform.position = Input.mousePosition - offsetLower ;
			}
		}
	}

	public void ShowTooltip(string text)
	{
		active = true;
		infoText.text = text;
		gameObject.SetActive(active);
	}

	public void HideTooltip()
	{
		active = false;
		gameObject.SetActive(active);
	}
}
