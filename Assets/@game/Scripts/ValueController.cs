using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BarState
{
	ZeroBars,
	OneBars,
	TwoBars,
	ThreeBars,
	FourBars,
	FiveBars,
	SixBars,
	SevenBars,
	EightBars,
	NineBars,
	TenBars,
}

public class ValueController : MonoBehaviour
{
	[SerializeField]
	private BarState barState = BarState.EightBars;

	[SerializeField]
	private List<GameObject> barGos;

	private bool minLock = false;
	private bool maxLock = false;

	public void IncreaseValue()
	{
		if (!maxLock)
		{
			barState++;
			minLock = false;
			UpdateBars();
		}

		if (barState == BarState.TenBars)
		{
			maxLock = true;
		}
	}

	public void ReduceValue()
	{
		if (!minLock)
		{
			barState--;
			maxLock = false;
			UpdateBars();
		}

		if (barState == BarState.ZeroBars)
		{
			minLock = true;
		}
	}

	public BarState GetBarState()
	{
		return barState;
	}

	public void SetBarState(BarState state)
	{
		barState = state;
	}

	public void UpdateBars()
	{
		// First deactivate all bars
		foreach (var go in barGos)
		{
			go.GetComponent<Image>().color = new Color32(136, 164, 199, 0);
		}

		if (barState != BarState.ZeroBars)
		{
			// Then activate the right number of bars
			for (int i = 0; i < (int)barState; i++)
			{
				barGos[i].GetComponent<Image>().color = new Color32(136, 164, 199, 255);
			}
		}
	}
}
