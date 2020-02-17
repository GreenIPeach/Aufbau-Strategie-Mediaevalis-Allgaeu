using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
	public static BuildingManager instance;
	
	[Header("Placement")]
	[SerializeField] private GameObject placmentGrid;
	[SerializeField] private LayerMask buildingMask;
	[VisibleOnly] [SerializeField] private bool isTownHallPlaced = false;
	[SerializeField] private BuildingPlacment buildingPlacment;
	[SerializeField] private BuildingPlacment currentPlacement;
	[SerializeField] private MiningPlacement miningPlacement;

	[Header("Ui")]
	[SerializeField] private BuildingUI buildingUI;
	
	[Header("Effects")]
	[SerializeField] private GameObject newBuildingParticleGo;
	[SerializeField] private GameObject upgradeParticleStarsGo;
	[SerializeField] private GameObject destructionParticleGo;
	
	[Header("Victory Event")]
	[SerializeField] private GameObject victoryScreenGo;
	
	[Header("Buildable Buildings")]
	public List<ScriptableObjectBuilding> buildings = new List<ScriptableObjectBuilding>();
	
	private GameObject clickedGameObject;
	private PlacableObject clickedPlacable;
	private int cityHallLevel = -1;
	private Camera mainCamera;

	private void Awake()
	{
		//buildingPlacment = GetComponent<BuildingPlacment>();

			if (instance != null)
			{
				Debug.LogError("More than one BuildManager in scene!");
				return;
			}
			instance = this;
		//currentPlacement = buildingPlacment;
		
		mainCamera = Camera.main;
	}

	public int CityHallLevel
	{
		get { return cityHallLevel; }
		set { cityHallLevel = value; UserInterface.instance.UpdateCategory(BuldingCategory.Hauptgebäude);}
	}

	public BuildingPlacment BuildingPlacment
	{
		get { return buildingPlacment; }
		set { buildingPlacment = value; }
	}

	private void Update()
	{
		if (buildingPlacment.currentBuilding == null)
		{
			placmentGrid.SetActive(false);
		}
		else
		{
			placmentGrid.SetActive(true);
		}
	


		if (Input.GetMouseButton(0) && currentPlacement.currentBuilding == null && currentPlacement.beenPlaced)
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}

			RaycastHit hitInfo;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, buildingMask))
			{
				if (clickedPlacable != null)
				{
					buildingUI.Hide();
				}

				clickedGameObject = hitInfo.collider.gameObject;
				clickedPlacable = clickedGameObject.GetComponent<PlacableObject>();

				Debug.Log(clickedPlacable);
				buildingUI.PassBuildingInfo(clickedPlacable.GetID(),clickedPlacable.GetUpgradeLevel(),clickedGameObject);

				// Construction/ Upgrade/ Deconstruction in progress?
				if (clickedPlacable.isPlaced)
				{
					if (clickedPlacable.GetID() == 10)//Ist die Burg!!
					{
						//über die Burg setzem	
						buildingUI.SetPosition(hitInfo.collider.gameObject.transform.position + new Vector3(-5,6,-5));
					}
					else
					{
						buildingUI.SetPosition(hitInfo.collider.gameObject.transform.position);
					}

					// Also display building ui
					buildingUI.ShowBuildingUi();
				}

				// Always show description and worker altering
				buildingUI.ShowBuildingPanelUi();

				// Building select sound
				AudioMixerManager.instance.PlaySound(clickedGameObject.name);
			}
			else
			{
				if (clickedPlacable != null)
				{
					buildingUI.Hide();
				}
			}
		}	
	}

	public void BuildNewBuilding(ScriptableObjectBuilding scriptableObjectBuilding)
	{
		//Reduce resources
		ScriptableBuilding scriptableBuilding = scriptableObjectBuilding.building[0];
		
		ResourceManager.instance.ReduceResources(GameResources.Wood, scriptableBuilding.wood);
		ResourceManager.instance.ReduceResources(GameResources.Stone, scriptableBuilding.stone);
		ResourceManager.instance.ReduceResources(GameResources.Iron, scriptableBuilding.iron);
		ResourceManager.instance.ReduceResources(GameResources.Coal, scriptableBuilding.coal);
		ResourceManager.instance.ReduceResources(GameResources.Gold, scriptableBuilding.gold);
		ResourceManager.instance.ReduceResources(GameResources.Weapons, scriptableBuilding.weapons);

		if (scriptableObjectBuilding.buildingName.Contains("Rathaus"))
		{
			isTownHallPlaced = true;
		}
		
		// Play building sound
		AudioMixerManager.instance.PlaySound("Build");

		// Spawn building smoke effect
		float totalDuration = scriptableObjectBuilding.building[0].constructionTime;
		SpawnBuildingParticles(newBuildingParticleGo, currentPlacement.currentBuilding, totalDuration);
	}

	/// <summary>
	/// Subtracts build cost, unlocks upgrade dependent events and plays the specific upgrade sound.
	/// Also destroys the previous building and instantiates the upgraded version of it.
	/// </summary>
	public void UpgradeBuilding()
	{
		// Reduce resources
		int placeableId					= clickedPlacable.GetID();
		int placeableNextUpgradeLvl		= clickedPlacable.GetUpgradeLevel() + 1;
		ScriptableObjectBuilding sob	= buildings[placeableId];
		ScriptableBuilding buildingToUpgrade= sob.building[placeableNextUpgradeLvl];
		
		ResourceManager.instance.ReduceResources(GameResources.Wood, buildingToUpgrade.wood);
		ResourceManager.instance.ReduceResources(GameResources.Stone,buildingToUpgrade.stone);
		ResourceManager.instance.ReduceResources(GameResources.Iron, buildingToUpgrade.iron);
		ResourceManager.instance.ReduceResources(GameResources.Coal, buildingToUpgrade.coal);
		ResourceManager.instance.ReduceResources(GameResources.Gold, buildingToUpgrade.gold);
		ResourceManager.instance.ReduceResources(GameResources.Weapons, buildingToUpgrade.weapons);

		// If isPlaced is false, the building will not show the upgrade and destroy button (buildingUI)
		clickedPlacable.isPlaced = false;

		// Unlock events
		switch (placeableId)
		{
			case 10:
				BoundariesScript.instance.UnlockNext();
				break;
			case 6:
				CityHallLevel = CityHallLevel + 1;
				break;
		}
		
		// Play sounds
		if (buildingToUpgrade.name.Contains("Castle"))
		{
			switch (placeableNextUpgradeLvl)
			{
				case 1:
					AudioMixerManager.instance.PlaySound("Lvl_1");
					break;
				case 2:
					AudioMixerManager.instance.PlaySound("Lvl_2");
					break;
				case 3:
					AudioMixerManager.instance.PlaySound("Lvl_3");
					break;
				default:
					break;
			}
		}
		else
		{
			AudioMixerManager.instance.PlaySound("Upgrade");
		}
		
		// Create construction site
		BuildConstructionSite(sob, clickedGameObject.transform);

		// Upgrade building after construction time
		StartCoroutine(DelayUpgrade(buildingToUpgrade));
	}

	private IEnumerator DelayUpgrade(ScriptableBuilding scriptableBuilding)
	{
		// Cache clickedGameObject so it doesn't get overwritten while coroutine is running
		GameObject tempUpgradingGo = clickedGameObject;

		yield return new WaitForSeconds(scriptableBuilding.constructionTime);

		// Spawn particles
		SpawnBuildingParticles(upgradeParticleStarsGo, tempUpgradingGo.transform, GetParticleDuration(upgradeParticleStarsGo));

		// Activate victory screen when condition is met
		if (scriptableBuilding.name.Contains("Rathaus"))
		{
			// GetComponent<PlaceableObject>() is only needed when the upgraded building was a town hall
			PlacableObject placeableObject = tempUpgradingGo.GetComponent<PlacableObject>();

			if (placeableObject.GetUpgradeLevel() + 1 == 3)
			{
				victoryScreenGo.SetActive(true);
			}
		}

		// Creation of new, upgraded building
		var pos = tempUpgradingGo.transform.position;
		var rot = tempUpgradingGo.transform.rotation;
		var transformParent = tempUpgradingGo.transform.parent;

		// Destroy "old" building
		DestroyBuilding(tempUpgradingGo);

		// Instantiate upgraded building
		GameObject upgradedBuildingGo = Instantiate(scriptableBuilding.building, pos, rot, transformParent);

		PlacableObject upgradedBuildingPlaceable = upgradedBuildingGo.GetComponent<PlacableObject>();
		
		if (upgradedBuildingPlaceable != null)
		{
			// Building is upgraded, enable showing the BuildingUI again
			upgradedBuildingPlaceable.isPlaced = true;
		}
	}

	/// <summary>
	/// Takes care of resource management and effects (visual and audible).
	/// Then destroys the building.
	/// </summary>
	public void DestroyClickedBuilding()
	{
		// Play sound
		AudioMixerManager.instance.PlaySound("Destroy");

		// Create a (de)construction site
		BuildConstructionSite(buildings[clickedPlacable.GetID()], clickedGameObject.transform);

		// Spawn destruction particles
		SpawnBuildingParticles(destructionParticleGo, clickedGameObject.transform, GetParticleDuration(destructionParticleGo));

		// Regain half of the spent resources
		RegainResources(clickedGameObject);

		// If the city hall was destroyed, then enable ability to build the town hall again.
		if (clickedGameObject.name.Contains("Rathaus"))
		{
			isTownHallPlaced = false;
		}

		// Only show description and worker altering, but not the buildingUI when isPlaced is false
		clickedPlacable.isPlaced = false;

		// Destroy the building already!
		StartCoroutine(DescendBuilding());
	}

	/// <summary>
	/// Actually destroys the building and removes workers, if it has any.
	/// </summary>
	/// <param name="buildingToDestroy">The GameObject you want to see dead.</param>
	private void DestroyBuilding(GameObject buildingToDestroy)
	{
		// TODO: Don't destroy mines/ quarries

		BuildingWorkers workersAtBuilding = buildingToDestroy.GetComponent<BuildingWorkers>();
		
		if (workersAtBuilding != null)
		{
			if (workersAtBuilding.workers.Count != 0)
			{
				int activeWorkers = workersAtBuilding.workers.Count;
				//Maybe add warning here. If there are still workers
				for (int i = 0; i < activeWorkers; i++)
				{
					CitizenManager.citizenIdle.Add(workersAtBuilding.workers[0]);
					workersAtBuilding.workers.RemoveAt(0);
				}
			}
		}

		Destroy(buildingToDestroy);
	}

	/// <summary>
	/// Destroys all buildings by iterating through all children of the BuildingManager.
	/// Calls DestroyBuilding([n-th child]).
	/// </summary>
	private void DestroyAllBuildings()
	{
		// Destroy all existing buildings
		if (transform.childCount > 0)
		{
			for (int i = 0; i < this.transform.childCount; i++)
			{
				// Destroy buildings without effects/ resource management
				DestroyBuilding(transform.GetChild(i).gameObject);
			}
		}
	}

	private IEnumerator DescendBuilding()
	{
		int placeableId 		= clickedPlacable.GetID();
		int placeableUpgradeLvl	= clickedPlacable.GetUpgradeLevel();
		ScriptableBuilding buildingToSink = buildings[placeableId].building[placeableUpgradeLvl];

		float elapsedTime = 0f;
		int constructionTime = buildingToSink.constructionTime;
		
		Vector3 offSet 			= new Vector3(0, 5, 0);
		Vector3 startPosition	= clickedGameObject.transform.position;
		Vector3 endPosition		= startPosition - offSet;

		// Cache clickedGameObject so it doesn't get overwritten while coroutine is running
		GameObject tempDescendingGo = clickedGameObject;

		while (elapsedTime < constructionTime)
		{
			// Position
			tempDescendingGo.transform.position = Vector3.Lerp(startPosition, endPosition, 
				(elapsedTime / constructionTime));

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Hide the building ui again in case the building was clicked during deconstruction
		buildingUI.Hide();

		DestroyBuilding(tempDescendingGo);
	}

	public void SetBuilding(ScriptableObjectBuilding building)
	{
		buildingUI.Hide();
		if (currentPlacement.currentBuilding != null)
		{
			Destroy(currentPlacement.currentBuilding.gameObject);
		}

		if (building.building[0].name == "Steinbruch_0" || building.building[0].name == "Bergwerk_0")
		{
			currentPlacement = miningPlacement;
		}
		else
		{
			currentPlacement = buildingPlacment;
		}

		if(ResourceManager.instance.ResourceAmountCheck(
			building.building[0].wood,building.building[0].stone,building.building[0].iron,
			building.building[0].coal,building.building[0].gold, building.building[0].weapons))
		{
			if (!building.name.Equals("Wohnhaus"))
			{
				if (building.buildingName.Contains("Rathaus"))
				{
					if (isTownHallPlaced)
					{
						UserInterface.instance.DisplayMessage("Das Rathaus wurde bereits gebaut.");
					}
					else
					{
						currentPlacement.SetBuilding(building);
					}
				}
				else
				{
					currentPlacement.SetBuilding(building);
				}
			}
			else
			{
				if (ResourceManager.instance.AdditionalSuppliableCitizenAvailable())
				{
					currentPlacement.SetBuilding(building);
				}
				else
				{
					UserInterface.instance.DisplayMessage("Nicht genügend Nahrung für weitere Bewohner");
				}
			}
		}
		else
		{
			UserInterface.instance.DisplayMessage("Es fehlen Rohstoffe!");
			//Have to display info: Not enough Materials
		}
	}

	/// <summary>
	/// Creates the construction site of a building. Destroys itself after the specific construction time.
	/// </summary>
	/// <param name="buildingToBuild">Necessary to get the specific construction site of a building.</param>
	/// <param name="buildingTransform">Position to create the construction site.</param>
	public void BuildConstructionSite(ScriptableObjectBuilding buildingToBuild, Transform buildingTransform)
	{
		int constructionTime = buildingToBuild.building[0].constructionTime;

		GameObject constructionSite = Instantiate(buildingToBuild.constructionSite,
			buildingTransform.position, buildingTransform.rotation, this.transform);

		Destroy(constructionSite, constructionTime);
	}

	/// <summary>
	/// Increases resources by half the original build cost.
	/// Also takes upgrade cost into account.
	/// </summary>
	/// <param name="buildingToDestroy"></param>
	private void RegainResources(GameObject buildingToDestroy)
	{
		// Give back half of the original cost
		PlacableObject placedBuilding = buildingToDestroy.GetComponent<PlacableObject>();

		if (placedBuilding != null)
		{
			List<ScriptableBuilding> scriptableBuildings = buildings[placedBuilding.GetID()].building;
			ScriptableBuilding scriptableBuilding = scriptableBuildings[placedBuilding.GetUpgradeLevel()];
			
			int woodCost = scriptableBuilding.wood;
			int stoneCost= scriptableBuilding.stone;
			int ironCost = scriptableBuilding.iron;
			int coalCost = scriptableBuilding.coal;
			int goldCost = scriptableBuilding.gold;
			int weaponsCost = scriptableBuilding.weapons;

			// Was the destroyed building upgradeable
			if (scriptableBuildings.Count > 1)
			{
				// Get the previous building cost and add it to the existing cost
				for (int i = 0; i < placedBuilding.GetUpgradeLevel(); i++)
				{
					woodCost += scriptableBuildings[i].wood;
					stoneCost+= scriptableBuildings[i].stone;
					ironCost += scriptableBuildings[i].iron;
					coalCost += scriptableBuildings[i].coal;
					goldCost += scriptableBuildings[i].gold;
					weaponsCost += scriptableBuildings[i].weapons;
				}
			}
			
			ResourceManager.instance.IncreaseResources(GameResources.Wood, woodCost / 2);
			ResourceManager.instance.IncreaseResources(GameResources.Stone,stoneCost/ 2);
			ResourceManager.instance.IncreaseResources(GameResources.Iron, ironCost / 2);
			ResourceManager.instance.IncreaseResources(GameResources.Coal, coalCost / 2);
			ResourceManager.instance.IncreaseResources(GameResources.Gold, goldCost / 2);
			ResourceManager.instance.IncreaseResources(GameResources.Weapons, weaponsCost / 2);
		}
	}

	private void SpawnBuildingParticles(GameObject particleToSpawnGo, Transform spawnTransform, float duration)
	{
		Transform spawnBuildingTransform = spawnTransform.transform;
		Quaternion psRot = spawnBuildingTransform.rotation;
		psRot *= Quaternion.Euler(-90.0f, 0f, 0f);
		GameObject particleGo = Instantiate( particleToSpawnGo, spawnBuildingTransform.position, psRot, this.transform);

		// Destroy when it has finished playing
		Destroy(particleGo, duration);
	}

	public void LoadAndPlaceBuildings(ExtractSaveGameValues values)
	{
		// TODO: Fix bugs:
		// City Hall is not on the correct upgrade level
		// Wohnhaus_1 is not clickable (because of missing child?)

		DestroyAllBuildings();

		// Place buildings on loaded positions and rotations
		ScriptableObjectBuilding objectBuilding = new ScriptableObjectBuilding();

		List<int> buildingIds			= values.GetBuildingIds();
		List<Vector3> buildingPositions	= values.GetBuildingTranslations();
		List<float> buildingRotations	= values.GetBuildingRotations();
		List<int> buildingLvls			= values.GetBuildingLevel();

		// Set loaded CityHallLevel (Requirement to place certain buildings).
		// Since the city hall is always placed first, it's level is saved at index 0.
		CityHallLevel = buildingLvls[0];

		for (int i = 0; i < buildingIds.Count; i++)
		{
			objectBuilding = buildings[buildingIds[i]];
			GameObject goBuilding = objectBuilding.building[buildingLvls[i]].building;

			if (goBuilding.name.Contains("Steinbruch") ||
				goBuilding.name.Contains("Bergwerk"))
			{
				// TODO: Re- Enable MeshRenderer of quarry/ mine parts
			}
			else
			{
				Quaternion rot = Quaternion.Euler(0f, buildingRotations[i], 0f);
				GameObject buildingToPlace = Instantiate(goBuilding, buildingPositions[i], rot, this.transform);
			}
		}
	}

	// TODO: Move this into a general helper class?
	/// <summary>
	/// Returns particle system duration plus lifetime.
	/// </summary>
	/// <param name="goWithParticles">GameObject with a particle system component.</param>
	private float GetParticleDuration(GameObject goWithParticles)
	{
		ParticleSystem goParticles = goWithParticles.GetComponent<ParticleSystem>();

		if (goParticles != null)
		{
			var psModule = goParticles.main;
			return psModule.duration + psModule.startLifetime.constant;
		}

		Debug.LogWarning(goWithParticles.name + " has no particle system component!");
		return 1f;
	}
	/*private void OnGUI()
	{
		if (Input.GetKey(KeyCode.B))
		{
			for (int i = 0; i < buildings.Count; i++)
			{
				if (GUI.Button(new Rect(20, 20 + 25 * i, 150, 30), buildings[i].buildingName))
				{
					if (buildingPlacment.currentBuilding != null)
					{
						Destroy(buildingPlacment.currentBuilding.gameObject);
					}
					buildingPlacment.SetBuilding(buildings[i].buildingStages[0]);
				} 
			}
		}
	}*/
}
