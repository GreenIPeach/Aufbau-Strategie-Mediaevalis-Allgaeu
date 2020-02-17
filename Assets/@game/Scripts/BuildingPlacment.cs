using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BuildingPlacment : MonoBehaviour
{
	[HideInInspector] public Transform currentBuilding;
	[HideInInspector] public bool beenPlaced = false;
	public Terrain terrain;
	public float maxHeightDifference = 2f;

	protected CollisionCheck collisonCheck;
	protected ScriptableObjectBuilding currentBuildingScriptable;
	protected Vector3 offSet		= new Vector3(0, 3, 0);
	protected Vector3 rotateVector	= new Vector3(0, 90, 0);
	protected List<GameObject> placedBuilding = new List<GameObject>();
	protected Camera mainCamera;

	private HeightDifferenceCheck heightCheck;

	void Awake()
	{
		mainCamera = Camera.main;
	}

	void Update()
	{
		RaycastHit hitInfo;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));
		if (currentBuilding != null && !beenPlaced)
		{
			// See description below, when layer is added again
			currentBuilding.gameObject.layer = 0;

			Vector3 position = hitInfo.point;
			position.y = terrain.SampleHeight(position);
			position.x -= hitInfo.point.x % 1;
			position.z -= hitInfo.point.z % 1;
			position.y += 0.1f;
			currentBuilding.transform.position = position;

			if (Input.GetKeyDown(KeyCode.E))
			{
				currentBuilding.transform.localEulerAngles += rotateVector;
			}
			else if (Input.GetKeyDown(KeyCode.Q))
			{
				currentBuilding.transform.localEulerAngles -= rotateVector;
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{
					return;
				}

				if (isValidPosition())
				{
					// The previous set to "Default" and now back to "Buildings",
					// the grid of the building itself is no longer red as long as it is buildable
					currentBuilding.gameObject.layer = LayerMask.NameToLayer("Buildings");

					BuildingManager.instance.BuildNewBuilding(currentBuildingScriptable);
					BuildingManager.instance.BuildConstructionSite(currentBuildingScriptable, currentBuilding);

					StartCoroutine(StartConstruction(currentBuilding));
					StartCoroutine("ObjectPlaced");
					placedBuilding.Add(currentBuilding.gameObject);
					currentBuilding.GetComponent<NavMeshObstacle>().enabled = true;

					currentBuilding = null;
					currentBuildingScriptable = null;

					Destroy(collisonCheck);
				}
			}

			if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButton(1))
			{
				Destroy(currentBuilding.gameObject);
				currentBuilding = null;
				currentBuildingScriptable = null;
				beenPlaced = true;
			}
		}
	}

	protected virtual IEnumerator StartConstruction(Transform currentBuilding)
	{
		float elapsedTime = 0f;
		int constructionTime = currentBuildingScriptable.building[0].constructionTime;
		Vector3 endPosition = currentBuilding.transform.position;
		currentBuilding.position -= offSet;
		var offSetPosition = currentBuilding.position;

		while (elapsedTime < constructionTime)
		{
			currentBuilding.position = Vector3.Lerp(offSetPosition, endPosition,
				(elapsedTime / constructionTime));
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		currentBuilding.GetComponent<PlacableObject>().isPlaced = true;
		AddScriptsToBuilding();
	}

	protected virtual void AddScriptsToBuilding()
	{
		foreach (var building in placedBuilding.ToList())
		{
			if (building.GetComponent<PlacableObject>().isPlaced)
			{
				if (building.name.Contains("Wohnhaus"))
				{
					building.AddComponent<SpawnCitizen>();
				}

				if (building.name.Contains("Forsthuette"))
				{
					building.AddComponent<Forsthuette>();
				}

				placedBuilding.Remove(building);
			}
		}
	}

	protected IEnumerator ObjectPlaced()
	{
		yield return new WaitForSeconds(0.1f);
		beenPlaced = true;
	}

	protected virtual bool isValidPosition()
	{
		if (collisonCheck.colliders.Count > 0)
		{
			UserInterface.instance.DisplayMessage("Du kannst hier nicht bauen");
			return false;
		}

		if (heightCheck.GetDifference() > maxHeightDifference)
		{
			UserInterface.instance.DisplayMessage("Das ist zu steil");
			return false;
		}

		return true;
	}

	public virtual void SetBuilding(ScriptableObjectBuilding b)
	{
		beenPlaced = false;
		//currentBuilding = ((GameObject) Instantiate(b.buildingStages[0])).transform;
		currentBuilding = (Instantiate(b.building[0].building.transform, this.transform));
		currentBuilding.GetComponent<NavMeshObstacle>().enabled = false;
		currentBuildingScriptable = b;

		collisonCheck = currentBuilding.gameObject.AddComponent<CollisionCheck>();

		heightCheck = currentBuilding.gameObject.AddComponent<HeightDifferenceCheck>();
		heightCheck.terrain = terrain;
	}
}