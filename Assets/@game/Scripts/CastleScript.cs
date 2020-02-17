using UnityEngine;
using TMPro;

public class CastleScript : MonoBehaviour
{
	public TextMeshProUGUI buildingName;
	public TextMeshProUGUI upgradeLev;
	private int buildingID;
	private int upgradeLevel;
	private int maxLevel;
	private GameObject clickedGameObject;
	public BoundariesScript bounds;

	public void Awake()
	{
		upgradeLevel = 0;
		maxLevel = 3;
	}

	public void Start()
	{
		//bounds = transform.Find("OpenableBoundaries").GetComponent<BoundariesScript>();
	}

	public void Update()
	{
		if (Input.GetKeyDown("x"))
		{
			UpgradeBuilding();
		}
	}

	private void SetInfo()
	{
		buildingName.text = BuildingManager.instance.buildings[buildingID].buildingName;
	}
	
	public void PassBuildingInfo(int id,int upgrade, GameObject _clickedObject)
	{
		buildingID = id;
		upgradeLevel = upgrade;
		clickedGameObject = _clickedObject;
		SetInfo();
	}

	public void UpgradeBuilding()
	{
		if(upgradeLevel < maxLevel)
		{
			upgradeLevel++;
			bounds.UnlockNext();
		}
		else
		{
			Debug.Log("No Upgrade available");
		}
	}


}