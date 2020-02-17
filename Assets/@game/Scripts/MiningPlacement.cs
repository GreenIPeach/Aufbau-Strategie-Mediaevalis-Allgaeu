using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;


public class MiningPlacement : BuildingPlacment
{
	
	public MiningBuildingsManager miningBuildings;
	[HideInInspector]
	public SnapCheck snapCheck;
	[HideInInspector]
	public GameObject snapObject;

	private void Awake()
	{
		mainCamera = Camera.main;
	}

	void Update ()
	{
		RaycastHit hitInfo;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));
		if (currentBuilding != null && !beenPlaced)
		{
			Vector3 position = hitInfo.point;

			position.x -= hitInfo.point.x % 1;
			position.z -= hitInfo.point.z % 1;
			position.y += 0.1f;
			currentBuilding.transform.position = position;
			
			if (Input.GetKeyDown(KeyCode.E))
			{
				currentBuilding.transform.localEulerAngles += rotateVector;
			}else if (Input.GetKeyDown(KeyCode.Q))
			{
				currentBuilding.transform.localEulerAngles -= rotateVector;
			}

			if (snapCheck.SomethingChanged())
			{
				if (snapObject != null)
				{
					if (currentBuilding.name.Contains("Steinbruch"))
					{
						for (int i = 0; i < currentBuilding.childCount; i++)
						{
							currentBuilding.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
							snapObject.transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().enabled = false;
						}
					}
					else
					{
						currentBuilding.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
						snapObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
					}
				}

				snapObject = null;
				if (snapCheck.GetClosestSnapPlace() != null)
				{
					snapObject = snapCheck.GetClosestSnapPlace();

					if (currentBuilding.name.Contains("Steinbruch"))
					{
						for (int i = 0; i < currentBuilding.childCount; i++)
						{
							currentBuilding.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
							snapObject.transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().enabled = true;
						}
					}
					else
					{
						currentBuilding.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
						snapObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
					}
				}
			}

			if(Input.GetMouseButtonDown(0))
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{
					return;
				}
				if(isValidPosition())
				{
					//Reduce Resurces
					ResourceManager.instance.ReduceResources(GameResources.Wood, currentBuildingScriptable.building[0].wood);
					ResourceManager.instance.ReduceResources(GameResources.Stone,currentBuildingScriptable.building[0].stone);
					ResourceManager.instance.ReduceResources(GameResources.Iron, currentBuildingScriptable.building[0].iron);
					ResourceManager.instance.ReduceResources(GameResources.Coal, currentBuildingScriptable.building[0].coal);
					ResourceManager.instance.ReduceResources(GameResources.Gold, currentBuildingScriptable.building[0].gold);
					//					
					StartCoroutine(StartConstruction(currentBuilding));
					StartCoroutine("ObjectPlaced");
					placedBuilding.Add(currentBuilding.gameObject);
					if (currentBuilding.name.Contains("Steinbruch"))
					{
						for (int i = 0; i < 3; i++)
						{
							currentBuilding.GetChild(i).GetComponent<NavMeshObstacle>().enabled = true;
						}
					}
					else
					{
						currentBuilding.GetComponent<NavMeshObstacle>().enabled = true;
					}
					/* posCollider.tag = "Untagged";
					posCollider.enabled = false;
					currentBuilding.GetComponent<BoxCollider>().enabled = true; */

					currentBuilding = null;
					currentBuildingScriptable = null;
					snapObject = null;
					Destroy(snapCheck);
					miningBuildings.HidePositions();
				}
			}
			if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButton(1))
			{
				miningBuildings.HidePositions();
				snapObject = null;
				Destroy(currentBuilding.gameObject);
			}
		}
	}

	protected override IEnumerator StartConstruction(Transform currentBuilding)
	{
		if (currentBuilding.name.Contains("Steinbruch"))
		{
			for (int i = 0; i < currentBuilding.childCount; i++)
			{
				currentBuilding.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
				snapObject.transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().enabled = false;
				currentBuilding.GetChild(i).localPosition = snapObject.transform.GetChild(0).GetChild(i).localPosition + snapObject.transform.GetChild(0).localPosition;
				currentBuilding.GetChild(i).localRotation = snapObject.transform.GetChild(0).GetChild(i).localRotation;
			}
		}
		else
		{
			currentBuilding.tag = snapObject.tag;							//Part of the workaround; see below.
			currentBuilding.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
			snapObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		}

		miningBuildings.SetInactive(snapObject);
	
		float elapsedTime = 0f;
		int consturctionTime = currentBuildingScriptable.building[0].constructionTime;
		//GameObject constructionSite = Instantiate(currentBuildingScriptable.constructionSite,currentBuilding.transform.position,currentBuilding.transform.rotation);
		currentBuilding.position = snapObject.transform.position - offSet;
		currentBuilding.rotation = snapObject.transform.rotation;
		var offSetPosition = currentBuilding.position;
		while (elapsedTime < consturctionTime)
		{
			currentBuilding.position = Vector3.Lerp(offSetPosition, offSetPosition + offSet, (elapsedTime/consturctionTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		currentBuilding.GetComponent<PlacableObject>().isPlaced = true;
		//Destroy(constructionSite);
		AddScriptsToBuilding();
	}

	protected override void AddScriptsToBuilding()
	{
		foreach (var building in placedBuilding.ToList())
		{
			if(building.GetComponent<PlacableObject>().isPlaced){
				if(building.name.Contains("Steinbruch"))
				{
					building.AddComponent<ResourceProduction>().ResourceProducing = GameResources.Stone;
				}
				if (building.name.Contains("Bergwerk"))
				{															//Workaround: Used Tag since snapObject can get changed because of the coroutine
					switch (building.tag)
					{
						case "GoldPlace":
						building.AddComponent<ResourceProduction>().ResourceProducing = GameResources.Gold;
						break;
						case "IronPlace":
						building.AddComponent<ResourceProduction>().ResourceProducing = GameResources.Iron;
						break;
						default:
						building.AddComponent<ResourceProduction>().ResourceProducing = GameResources.Coal;
						break;
					}
					building.tag = "Buildings";
				}
				placedBuilding.Remove(building);
			}
		}
	}

	protected override bool isValidPosition()
	{
		if (snapObject == null)
		{
			return false;
		}
		return true;
	}
	
	public override void SetBuilding(ScriptableObjectBuilding b)
	{
		beenPlaced = false;
		//currentBuilding = ((GameObject) Instantiate(b.buildingStages[0])).transform;
		currentBuilding = Instantiate(b.building[0].building.transform, this.transform);
		if (currentBuilding.name.Contains("Steinbruch"))
		{
			for (int i = 0; i < 3; i++)
			{
				currentBuilding.GetChild(i).GetComponent<NavMeshObstacle>().enabled = false;
			}
		}
		else
		{
			currentBuilding.GetComponent<NavMeshObstacle>().enabled = false;
		}
		currentBuildingScriptable = b;
		snapCheck = currentBuilding.gameObject.AddComponent<SnapCheck>();
		miningBuildings.HidePositions();
		miningBuildings.ShowUnusedPositions(currentBuilding.gameObject);
	}
}
