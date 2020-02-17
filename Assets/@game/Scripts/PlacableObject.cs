using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableObject : MonoBehaviour{
	[SerializeField]
	private int iD = -1;
	[SerializeField]
	private int upgradeLevel = 0;

    public bool isPlaced = false;

	[SerializeField] 
	private BuldingCategory category;

	public int GetID()
	{
		return iD;
	}

	public int GetUpgradeLevel()
	{
		return upgradeLevel;
	}
	
	public void SetUpgradeLevel(int upgrade)
	{
		 upgradeLevel = upgrade;
	}
	
	
}
