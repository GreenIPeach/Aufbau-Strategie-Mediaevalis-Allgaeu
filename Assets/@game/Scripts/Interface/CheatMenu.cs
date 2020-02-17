using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
	[SerializeField]
	private KeyCode openCheatMenu = KeyCode.C;

	[Header("Bindings")] 
	[SerializeField] 
	private GameObject profiler;
	
	
	private GameObject cheatMenuCanvas;
	private void Awake()
	{
		cheatMenuCanvas = gameObject.transform.GetChild(0).gameObject;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(openCheatMenu))
		{
			cheatMenuCanvas.SetActive(!cheatMenuCanvas.activeSelf);
		}
	}

	public void CheatWood(TMP_InputField input)
	{
		ResourceManager.instance.IncreaseResources(GameResources.Wood,int.Parse(input.text));
		input.text = "";
	}
	public void CheatStone(TMP_InputField input)
	{
		ResourceManager.instance.IncreaseResources(GameResources.Stone,int.Parse(input.text));
		input.text = "";
	}
	public void CheatIron(TMP_InputField input)
	{
		ResourceManager.instance.IncreaseResources(GameResources.Iron,int.Parse(input.text));
		input.text = "";
	}	
	public void CheatCoal(TMP_InputField input)
	{
		ResourceManager.instance.IncreaseResources(GameResources.Coal,int.Parse(input.text));
		input.text = "";
	}
	public void CheatGold(TMP_InputField input)
	{
		ResourceManager.instance.IncreaseResources(GameResources.Gold,int.Parse(input.text));
		input.text = "";
	}

	public void CheatWeapons(TMP_InputField input)
	{
		ResourceManager.instance.IncreaseResources(GameResources.Weapons,int.Parse(input.text));
		input.text = "";
	}

	public void CheatEverything()
	{
		ResourceManager.instance.IncreaseResources(GameResources.Wood,9999);
		ResourceManager.instance.IncreaseResources(GameResources.Stone,9999);
		ResourceManager.instance.IncreaseResources(GameResources.Iron,9999);
		ResourceManager.instance.IncreaseResources(GameResources.Coal,9999);
		ResourceManager.instance.IncreaseResources(GameResources.Gold,9999);
		ResourceManager.instance.IncreaseResources(GameResources.Weapons, 9999);
	}

	public void DisplayProfiler(bool display)
	{
		profiler.gameObject.SetActive(display);
	}
	
}

