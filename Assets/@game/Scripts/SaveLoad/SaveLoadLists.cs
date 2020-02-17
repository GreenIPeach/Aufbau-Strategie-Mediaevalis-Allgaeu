using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class contains 
public class SaveLoadLists : MonoBehaviour
{
	public static SaveLoadLists instance;

	// TODO: 2 List for trees:
	// Those, which are already placed at game start
	// and those, which are planted by a forest ranger

	// Game Objects
	public List<GameObject> buildings;
	public List<GameObject> trees;
	public List<GameObject> citizen;

	// Gameplay
	// Unlocked Buildings
	int[] unlockedBuildings = new int[8];

	private void Awake()
	{
		if (instance != null) 
		{
			Debug.LogError("More than one SaveLoadLists in scene!");
			return;
		}
		instance = this;
	}

	private void Start()
	{
		// Initialize Unlocked buildings
		// TODO: Do once!
		// Always unlocked buildings are not saved:
		// Town Hall 0, Castle 0, Forest ranger, Quarry, House
		// 0 equals locked, except Castle and Town Hall
		unlockedBuildings[0] = 0; // Town Hall upgrades: 3
		unlockedBuildings[1] = 0; // Castle upgrades: 3
		unlockedBuildings[2] = 0; // Mine upgrades: 1
		unlockedBuildings[3] = 0; // Smelter upgrades: 1
		unlockedBuildings[4] = 0; // Mill: upgrades: 0
		unlockedBuildings[5] = 0; // Bakery upgrades: 0
		unlockedBuildings[6] = 1; // Farm: upgrades: 1
		unlockedBuildings[7] = 0; // Pest house: upgrades: 0
	}

	/// <summary>
	/// There are 8 buildings + their tiers to unlock: <para/>
	/// Town Hall: 3,
	/// Castle: 3,
	/// Mine: 1,
	/// Smelter: 1,
	/// Mill: 0,
	/// Bakery: 0,
	/// Farm: 1,
	/// Pest house: 0
	/// </summary>
	/// <param name="index">Building to alter.</param>
	/// <param name="tier">New tier.</param>
	public void UnlockBuilding(int index, int tier)
	{
		if (index >= 0 && index <= 8)
		{
			unlockedBuildings[index] = tier;
		}
	}

	/// <summary>
	/// Add a citizen to a list to save it's location and rotation.
	/// </summary>
	public void AddCitizenToList(GameObject citizenToAdd)
	{
		if (citizenToAdd != null)
		{
			citizen.Add(citizenToAdd);
		}
	}

	/// <summary>
	/// Add a building to a list to save it's location and rotation.
	/// </summary>
	public void AddBuildingToList(GameObject buildingToAdd)
	{
		if (buildingToAdd != null)
		{
			buildings.Add(buildingToAdd);
		}
	}

	/// <summary>
	/// Add a tree to a list to save it's location
	/// </summary>
	public void AddTreeToList(GameObject treeToAdd)
	{
		if (treeToAdd != null)
		{
			trees.Add(treeToAdd);
		}
	}
}
