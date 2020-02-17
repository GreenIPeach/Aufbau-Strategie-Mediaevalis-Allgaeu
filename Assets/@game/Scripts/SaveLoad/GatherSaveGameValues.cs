using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// This class gathers all savable information from the game.
/// </summary>
public class GatherSaveGameValues : MonoBehaviour
{
	public static GatherSaveGameValues instance;

	[SerializeField] private GameObject citizensRoot; // All citizen GameObjects are in this GO
	[SerializeField] private GameObject buildingsRoot; // All building GameObjects are in this GO

	private List<GameObject> buildingGos = new List<GameObject>();
	private List<GameObject> citizenGos = new List<GameObject>();

	private string saveGameName;
	// TODO: Singleton?
	private SaveLoadLists saveLoadLists = new SaveLoadLists();

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one GatherSaveGameValues in scene!");
			return;
		}

		instance = this;
	}

	public GatherSaveGameValues()
	{
	}

	// TODO: Rename (e.g. InitSaves) or delete
	/// <summary>
	/// Prepares the game to be saved by calling the method to all game objects in the scene hierarchy.
	/// Takes then a screenshot and calls the SaveLoad.Save(...) function.
	/// Saves the game with a user specified name and a timestamp.
	/// </summary>
	/// <param name="saveName">Save/ file name</param>
	public void SaveGame(string saveName)
	{
		buildingGos = FindBuildings();
		citizenGos = FindCitizen();
		saveGameName = saveName;

		SaveLoad.Save(this, saveGameName + "@" + GatherCurrentTimestamp().ToString());

		Debug.Log("Saved Game!");
	}

	/// <summary>
	/// This fills the list of game objects, gathered from a specific layer.
	/// At the moment the "buildings"- layer has an integer value of 9.
	/// </summary>
	/// <param name="layer">The layer the seareched game objects are on.</param>
	List<GameObject> FindGameObjectsInLayer(int layer)
	{
		GameObject[] goArray = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		List<GameObject> gameObjects = new List<GameObject>();

		foreach (var go in goArray)
		{
			if (go.layer == layer)
			{
				Debug.Log("Object found in layer " + go.layer);
			}
		}

		return gameObjects;
	}

	List<GameObject> FindBuildings()
	{
		List<GameObject> buildings = new List<GameObject>();

		for (int i = 0; i < buildingsRoot.transform.childCount; i++)
		{
			buildings.Add(buildingsRoot.transform.GetChild(i).gameObject);
		}

		return buildings;
	}

	List<GameObject> FindCitizen()
	{
		List<GameObject> citizens = new List<GameObject>();

		GameObject citizenRoot = GameObject.Find("CitizenManger");

		for (int i = 0; i < citizenRoot.transform.childCount; i++)
		{
			citizens.Add(citizenRoot.transform.GetChild(i).gameObject);
		}

		return citizens;
	}

	GameObject GetCameraGameObject()
	{
		return Camera.main.gameObject;
	}

	public Vector3 GatherCameraPosition()
	{
		return GetCameraGameObject().transform.position;
	}

	public Quaternion GatherCameraRotation()
	{
		return GetCameraGameObject().transform.rotation;
	}

	/// <summary>
	/// This method extracts the TRANSLATION vectors of the gathered game objects.
	/// </summary>
	/// <returns>A list of translation vectors.</returns>
	public List<Vector3> GatherBuildingTranslations()
	{
		List<Vector3> translationVectors = new List<Vector3>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				if (!IsConstructionSite(go.name))
				{
					translationVectors.Add(go.transform.position);
				}
			}
		}

		return translationVectors;
	}

	/// <summary>
	/// This method extracts the ROTATION of the gathered game objects.
	/// Since the buildings can only be rotated around the y- axis, we only need to save this axis.
	/// </summary>
	/// <returns>A list of floats.</returns>
	public List<float> GatherBuildingRotations()
	{
		List<float> rotationsYAxis = new List<float>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				if (!IsConstructionSite(go.name))
				{
					rotationsYAxis.Add(go.transform.rotation.eulerAngles.y);
				}
			}
		}

		return rotationsYAxis;
	}

	/// <summary>
	/// This method gets the building id of a game object.
	/// The building id determines the type of building.
	/// </summary>
	/// <returns></returns>
	public List<int> GatherBuildingIds()
	{
		List<int> buildingIds = new List<int>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				// TODO: Effect of game objects with two children? E.g. Farm
				// Since we're gathering only the root gamy objects of the hierarchy, we have to get the components of the children
				if (go.GetComponent<PlacableObject>() != null)
				{
					if (!IsConstructionSite(go.name))
					{
						buildingIds.Add(go.GetComponent<PlacableObject>().GetID());
					}
				}
			}
		}

		return buildingIds;
	}

	/// <summary>
	/// This method gets the building name of a game object.
	/// </summary>
	public List<string> GatherBuildingObjectNames()
	{
		List<string> buildingNames = new List<string>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				if (!IsConstructionSite(go.name))
				{
					buildingNames.Add(go.name);
				}
			}
		}

		return buildingNames;
	}

	/// <summary>
	/// Gathers a buildings upgrade level.
	/// </summary>
	public List<int> GatherBuildingUpgradeLevel()
	{
		List<int> lvl = new List<int>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				if (!IsConstructionSite(go.name))
				{
					lvl.Add(go.GetComponent<PlacableObject>().GetUpgradeLevel());
				}
			}
		}

		return lvl;
	}

	// TODO: Find a way to fill list of GameObjects
	/// <summary>
	/// Gathers the worker count a resource production building has.
	/// If a building does not produce anything, it will be flagged with -1.
	/// </summary>
	public List<int> GatherBuildingWorkers()
	{
		List<int> workers = new List<int>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				if (!IsConstructionSite(go.name))
				{
					BuildingWorkers bw = go.GetComponent<BuildingWorkers>();

					// Does this building even has this component?
					if (bw != null)
					{
						workers.Add(bw.workers.Count);
					}
					else
					{
						// Add -1 flag to keep the same list count as there are total buildings.
						workers.Add(-1);
					}
				}
			}
		}

		return workers;
	}

	/// <summary>
	/// Gathers the idle worker count a resource production building has.
	/// If a building does not produce anything, it will be flagged with -1.
	/// </summary>
	public List<int> GatherBuildingIdleWorkers()
	{
		List<int> idles = new List<int>();

		if (buildingGos != null)
		{
			foreach (var go in buildingGos)
			{
				if (!IsConstructionSite(go.name))
				{
					BuildingWorkers bw = go.GetComponent<BuildingWorkers>();

					// Does this building even has this component?
					if (bw != null)
					{
						idles.Add(bw.idleWorkers.Count);
					}
					else
					{
						idles.Add(-1);
					}
				}
			}
		}

		return idles;
	}

	/// <summary>
	/// This method returns the current unix timestamp in seconds.
	/// </summary>
	public int GatherCurrentTimestamp()
	{
		return (int)(DateTime.UtcNow.Subtract(
			new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
	}

	/// <summary>
	/// This method sets the name of the save game, as it was inputted by the user.
	/// </summary>
	public string GatherSaveGameName()
	{
		return saveGameName;
	}

	/// <summary>
	/// Retrieves the location of a citizen.
	/// </summary>
	public List<Vector3> GatherCitizenLocations()
	{
		List<Vector3> locations = new List<Vector3>();

		if (citizenGos.Count != 0)
		{
			foreach (var citizen in citizenGos)
			{
				locations.Add(citizen.transform.position);
			}
		}

		return locations;
	}

	/// <summary>
	/// Retrieves the rotation of a citizen.
	/// </summary>
	public List<float> GatherCitizenRotations()
	{
		List<float> rotations = new List<float>();

		foreach (var citizen in citizenGos)
		{
			rotations.Add(citizen.transform.rotation.y);
		}

		return rotations;
	}

	public List<int> GatherCitizenType()
	{
		List<int> type = new List<int>();

		foreach (var citizen in citizenGos)
		{
			if (citizen.gameObject.name.Contains("0")) // Siedler_0(Clone)
			{
				type.Add(0);
			}
			else if(citizen.gameObject.name.Contains("1")) // Siedler_1(Clone)
			{
				type.Add(1);
			}
			else // TODO: More than 2 types of settler models?
			{
				type.Add(0);
			}
			
		}

		return type;
	}

	public List<GameObject> GatherCitizenWorkplaces()
	{
		List<GameObject> workplaces = new List<GameObject>();

		foreach (var citizen in citizenGos)
		{
			workplaces.Add(citizen.GetComponent<WorkerController>().workplace);
		}

		return workplaces;
	}

	public List<int> GatherCitizenTarget() // TODO: Find a way to save targets!
	{
		List<int> targets = new List<int>();

		foreach (var citizen in citizenGos)
		{
			if (citizen.GetComponent<WorkerController>().target == null)
			{
				targets.Add(0);
			}
			else
			{
				targets.Add(1);
			}
		}

		return targets;
	}

	/// <summary>
	/// Gather resource/ citizen count. <para/>
	/// Order: wood, stone, iron, coal, gold, idle citizen, max. citizen.
	/// </summary>
	public List<int> GatherResourceAndCitizenCount()
	{
		// TODO: Save as an array instead of a list
		List<int> resources = new List<int>();

		if (ResourceManager.instance != null)
		{
			resources.Add(ResourceManager.instance.Wood);
			resources.Add(ResourceManager.instance.Stone);
			resources.Add(ResourceManager.instance.Iron);
			resources.Add(ResourceManager.instance.Coal);
			resources.Add(ResourceManager.instance.Gold);
			resources.Add(ResourceManager.instance.Citizen);
			resources.Add(ResourceManager.instance.MaxCitizen);
			//resources.Add(CitizenManager.citizenIdle.Count); // TODO: Save?
		}

		return resources;
	}

	/// <summary>
	/// Retrieves the location of the trees.
	/// </summary>
	public List<Vector3> GatherTreeLocations()
	{
		List<Vector3> locations = new List<Vector3>();
		List<GameObject> treeGOs = saveLoadLists.trees;

		if (treeGOs == null)
			return locations; // TODO: fix/ make nicer

		foreach (var tree in treeGOs)
		{
			if (tree != null)
			{
				locations.Add(tree.transform.position);
			}
		}

		return locations;
	}

	/// <summary>
	/// Checks if the game objects is a construction site.
	/// Those do not need to be saved.
	/// </summary>
	/// <param name="gameObjectName">Name of the game object to check.</param>
	/// <returns>True, if go is a construction site.</returns>
	private bool IsConstructionSite(string gameObjectName)
	{
		if (gameObjectName.Contains("Constructionsite"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}