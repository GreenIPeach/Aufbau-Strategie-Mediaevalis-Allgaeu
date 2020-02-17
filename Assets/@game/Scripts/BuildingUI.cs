using UnityEngine;
using TMPro;

public class BuildingUI : MonoBehaviour
{
	public GameObject destroyBtn;
	public GameObject upgradeBtn;

	[Header("Building Description")]
	public GameObject buildingUIPanel;
	public TextMeshProUGUI buildingClickedName;
	public TextMeshProUGUI buildingDescr;
	private bool wasBuildingUiActive = false;
	
	[Header("Worker Count")]
	public TextMeshProUGUI workerCount;

	public GameObject workerAlteringPanel;
	public GameObject addWorker;
	public GameObject removeWorker;

	[Header("Building Cost")]
	public GameObject costPanel;
	[SerializeField] private TextMeshProUGUI buildingCostName;
	[SerializeField] private TextMeshProUGUI woodText;
	[SerializeField] private TextMeshProUGUI stoneText;
	[SerializeField] private TextMeshProUGUI ironText;
	[SerializeField] private TextMeshProUGUI coalText;
	[SerializeField] private TextMeshProUGUI goldText;
	[SerializeField] private TextMeshProUGUI weaponsText;
	
	private int buildingID;
	private int upgradeLevel;
	private GameObject clickedGameObject;
	private BuildingWorkers buildingWorkers;
	public CitizenOverviewDisplay citizenOverviewDisplay;
	private GameObject buildingActionsCanvas;
	private Camera mainCamera;

	private void Awake()
	{
		buildingActionsCanvas = transform.GetChild(0).gameObject;
		
		mainCamera = Camera.main;
	}

	private void Update()
	{
		Quaternion camRot = mainCamera.transform.rotation;
		
		buildingActionsCanvas.transform.LookAt(
			buildingActionsCanvas.transform.position + camRot * Vector3.forward,
			camRot * Vector3.up);
	}

	private void SetInfo()
	{
		wasBuildingUiActive = true;
		
		buildingClickedName.text = BuildingManager.instance.buildings[buildingID].buildingName;
		buildingDescr.text = BuildingManager.instance.buildings[buildingID].buildingDescription;
		
		if (buildingID == 10) // wenn es die Burg ist
		{
			destroyBtn.SetActive(false);
		}
		else
		{
			destroyBtn.SetActive(true);
		}
		if(clickedGameObject.GetComponent<BuildingWorkers>() != null)
		{
			buildingWorkers = clickedGameObject.GetComponent<BuildingWorkers>();
			workerCount.text = buildingWorkers.workers.Count.ToString();
			workerAlteringPanel.SetActive(true);
		}
		else
		{
			workerCount.text = "";
			workerAlteringPanel.SetActive(false);
		}
	}

	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	public void PassBuildingInfo(int id,int upgrade, GameObject _clickedObject)
	{
		buildingID = id;
		upgradeLevel = upgrade;
		clickedGameObject = _clickedObject;
		SetInfo();
	}

	public void ShowBuildingUi()
	{
		gameObject.SetActive(true);

		// + 1 because upgradeLevel starts at 0
		int upgradeCount = BuildingManager.instance.buildings[buildingID].building.Count;
		bool isUpgradable = upgradeLevel + 1 < upgradeCount;
		upgradeBtn.SetActive(isUpgradable);
	}

	public void ShowBuildingPanelUi()
	{
		buildingUIPanel.SetActive(true);
	}

	public void UpgradeBuilding()
	{
		if(CheckResources())
		{
			Hide();
			ShowCostPanel(false);
			
			BuildingManager.instance.UpgradeBuilding();
		}
		else
		{
			UserInterface.instance.DisplayMessage("Nicht genügend Rohstoffe");
		}
	}

	public void ShowUpgradeCost()
	{
		CheckBuildingUiActive();
		
		if (BuildingManager.instance.buildings[buildingID].building.Count >= upgradeLevel + 2)
		{
			ShowCostPanel(true);

			ScriptableBuilding buildingToUpgrade = BuildingManager.instance.buildings[buildingID].building[upgradeLevel + 1];
			
			buildingCostName.text = BuildingManager.instance.buildings[buildingID].buildingName;
			woodText.text	= buildingToUpgrade.wood.ToString();
			stoneText.text	= buildingToUpgrade.stone.ToString();
			ironText.text	= buildingToUpgrade.iron.ToString();
			coalText.text	= buildingToUpgrade.coal.ToString();
			goldText.text	= buildingToUpgrade.gold.ToString();
			weaponsText.text = buildingToUpgrade.weapons.ToString();
		}
		else
		{
			// No upgrade exists, don't show the cost panel
			ShowCostPanel(false);
		}
	}

	public void ShowCost(int passedBuildingId)
	{
		CheckBuildingUiActive();
		
		ShowCostPanel(true);

		ScriptableBuilding buildingToBuild = BuildingManager.instance.buildings[passedBuildingId].building[0];
		
		buildingCostName.text = BuildingManager.instance.buildings[passedBuildingId].buildingName;
		woodText.text	= buildingToBuild.wood.ToString();
		stoneText.text	= buildingToBuild.stone.ToString();
		ironText.text	= buildingToBuild.iron.ToString();
		coalText.text	= buildingToBuild.coal.ToString();
		goldText.text	= buildingToBuild.gold.ToString();
		weaponsText.text= buildingToBuild.weapons.ToString();
	}

	public void ShowCostPanel(bool visibility)
	{
		if (visibility)
		{
			buildingUIPanel.SetActive(false);
		}
		else
		{
			if (wasBuildingUiActive)
			{
				buildingUIPanel.SetActive(true);
			}
		}
		
		costPanel.SetActive(visibility);
	}

	private bool CheckResources()
	{
		ScriptableBuilding sb = BuildingManager.instance.buildings[buildingID].building[upgradeLevel + 1];

		return ResourceManager.instance.ResourceAmountCheck(sb.wood, sb.stone, sb.iron, sb.coal, sb.gold, sb.weapons);
	}

	public void AddWorker()
	{
		PlacableObject clickedPlacedObject = clickedGameObject.GetComponent<PlacableObject>();

		if (clickedPlacedObject != null)
		{
			if (!(buildingWorkers.workers.Count >= BuildingManager.instance.buildings[clickedPlacedObject.GetID()].building[0].maxWorkers))
			{
				if (CitizenManager.citizenIdle.Count > 0)
				{
					GameObject worker = CitizenManager.citizenIdle[0];
					worker.GetComponent<WorkerController>().SetWorkplace(clickedGameObject);
					buildingWorkers.workers.Add(CitizenManager.citizenIdle[0]);
			
					//buildingWorkers.idleWorkers.Add(CitizenManager.citizenIdle[0]);
					CitizenManager.citizenIdle.RemoveAt(0);

					citizenOverviewDisplay.ChangeWorkerCount(clickedGameObject.name, +1);

					SetInfo();
				}
			}
			//else
			//{
			//	UserInterface.instance.DisplayMessage("Mehr Arbeiter können hier nicht arbeiten!");
			//}
		}
	}

	public void RemoveWorker()
	{
		if (buildingWorkers.workers.Count > 0)
		{
			GameObject worker = buildingWorkers.workers[0];
			worker.GetComponent<WorkerController>().workplace = null;
			CitizenManager.citizenIdle.Add(buildingWorkers.workers[0]);
			buildingWorkers.workers.RemoveAt(0);

			citizenOverviewDisplay.ChangeWorkerCount(gameObject.name, -1);

			SetInfo();
		}
	}

	public void DestroyBuilding()
	{
		gameObject.SetActive(false);
		Hide();
		
		BuildingManager.instance.DestroyClickedBuilding();
	}
	public void Hide()
	{
		gameObject.SetActive(false);
		buildingUIPanel.SetActive(false);
		wasBuildingUiActive = false;
		costPanel.SetActive(false);
	}

	private void CheckBuildingUiActive()
	{
		wasBuildingUiActive = buildingUIPanel.activeInHierarchy;
	}
}
