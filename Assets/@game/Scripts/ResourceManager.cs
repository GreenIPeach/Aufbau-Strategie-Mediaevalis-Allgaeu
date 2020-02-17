using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Flags]
public enum GameResources
{
	Wood	= 0,
	Stone	= 1 << 0,
	Iron	= 1 << 1,
	Coal	= 1 << 2,
	Gold	= 1 << 3,
	Food	= 1 << 4,
	Flour	= 1 << 5,
	Weapons	= 1 << 6
}

public class ResourceManager : MonoBehaviour
{
	public static ResourceManager instance;
	
	[Header("Resources")]
	[EditDefaultsOnly] [SerializeField] private int wood 	= 220;
	[EditDefaultsOnly] [SerializeField] private int stone 	= 50;
	[EditDefaultsOnly] [SerializeField] private int iron 	= 0;
	[EditDefaultsOnly] [SerializeField] private int coal	= 0;
	[EditDefaultsOnly] [SerializeField] private int gold	= 0;
	[EditDefaultsOnly] [SerializeField] private int food	= 0;
	[EditDefaultsOnly] [SerializeField] private int flour	= 0;
	[EditDefaultsOnly] [SerializeField] private int weapons = 0;

	[EditDefaultsOnly] [SerializeField] private int citizen 	= 0;
	[EditDefaultsOnly] [SerializeField] private int maxCitizen	= 10;
	
	#region ResourceProperties
	public int Wood
	{
		get { return wood; }
		private set
		{
			wood = value;
			wood = Mathf.Max(0, wood);
		}
	}

	public int Stone
	{
		get { return stone; }
		private set
		{
			stone = value;
			stone = Mathf.Max(0, stone);
		}
	}
	
	public int Iron
	{
		get { return iron; }
		private set
		{
			iron = value;
			iron = Mathf.Max(0, iron);
		}
	}
	
	public int Coal
	{
		get { return coal; }
		private set
		{
			coal = value;
			coal = Mathf.Max(0, coal);
		}
	}
	
	public int Gold
	{
		get { return gold; }
		private set
		{
			gold = value;
			gold = Mathf.Max(0, gold);
		}
	}
	
	public int Food
	{
		get { return food; }
		private set
		{
			food = value;
			food = Mathf.Max(0, food);
		}
	}
	
	public int Flour
	{
		get { return flour; }
		private set
		{
			flour = value;
			flour = Mathf.Max(0, flour);
		}
	}

	public int Weapons
	{
		get { return weapons; }
		private set
		{
			weapons = value;
			weapons = Mathf.Max(0, weapons);
		}
	}
	
	public int Citizen
	{
		get { return citizen; }
		private set
		{
			citizen = value;
			citizen = Mathf.Max(0, citizen);
		}
	}
	
	public int MaxCitizen
	{
		get { return maxCitizen; }
		private set
		{
			maxCitizen = value;
			maxCitizen = Mathf.Max(0, maxCitizen);
		}
	}
	#endregion

	[Header("Ui")]
	[SerializeField] private TextMeshProUGUI woodText;
	[SerializeField] private TextMeshProUGUI stoneText;
	[SerializeField] private TextMeshProUGUI ironText;
	[SerializeField] private TextMeshProUGUI coalText;
	[SerializeField] private TextMeshProUGUI goldText;
	[SerializeField] private TextMeshProUGUI weaponsText;
	[SerializeField] private TextMeshProUGUI citizenText;
	[SerializeField] private TextMeshProUGUI maxCitizenText;

	private bool millIsBuilt = false;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one ResourceManager in scene!");
			return;
		}

		instance = this;
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		woodText.text		= Wood.ToString();
		stoneText.text		= Stone.ToString();
		ironText.text		= Iron.ToString();
		coalText.text		= Coal.ToString();
		goldText.text		= Gold.ToString();
		weaponsText.text 	= Weapons.ToString();
		citizenText.text	= citizen.ToString();
		maxCitizenText.text	= maxCitizen.ToString();
	}

	public void ReduceResources(GameResources resource, int amount)
	{
		AlterResourceValue(resource, amount * -1);
	}

	public void IncreaseResources(GameResources resource, int amount)
	{
		AlterResourceValue(resource, amount);
	}
	/// <summary>
	/// Checks if resources are available.
	/// </summary>
	/// <returns>False, if one resource is not available.</returns>
	public bool ResourceAmountCheck(int woodRequired, int stoneRequired, int ironRequired,
		int coalRequired, int goldRequired, int weaponsRequired)
	{
		return SpecificResourceAmountCheck(GameResources.Wood, woodRequired)	&&
			   SpecificResourceAmountCheck(GameResources.Stone,stoneRequired)	&&
			   SpecificResourceAmountCheck(GameResources.Iron, ironRequired)	&&
			   SpecificResourceAmountCheck(GameResources.Coal, coalRequired)	&&
			   SpecificResourceAmountCheck(GameResources.Gold, goldRequired)	&&
			   SpecificResourceAmountCheck(GameResources.Weapons, weaponsRequired);
	}

	public bool SpecificResourceAmountCheck(GameResources resource, int amount)
	{
		switch (resource)
		{
			case GameResources.Wood:
				return Wood >= amount;
			case GameResources.Stone:
				return Stone >= amount;
			case GameResources.Iron:
				return Iron >= amount;
			case GameResources.Coal:
				return Coal >= amount;
			case GameResources.Gold:
				return Gold >= amount;
			case GameResources.Weapons:
				return Weapons >= amount;
			case GameResources.Food:
				return Food >= amount;
			case GameResources.Flour:
				return Flour >= amount;
			default:
				Debug.LogWarning("Should not have happened! No resource to check found.");
				return false;
		}
	}

	void AlterResourceValue(GameResources resource, int amount)
	{
		switch (resource)
		{
			case GameResources.Wood:
				Wood += amount;
				break;
			case GameResources.Stone:
				Stone += amount;
				break;
			case GameResources.Iron:
				Iron += amount;
				break;
			case GameResources.Coal:
				Coal += amount;
				break;
			case GameResources.Gold:
				Gold += amount;
				break;
			case GameResources.Weapons:
				Weapons += amount;
				break;
			case GameResources.Food:
				Food += amount;
				break;
			case GameResources.Flour:
				Flour += amount;
				break;
			default:
				Debug.LogWarning("Should not have happened! No resource to alter found.");
				break;
		}
	}

	/// <summary>
	/// Checks if there is enough food for more citizen.
	/// </summary>
	/// <returns>False, if there is not enough food.</returns>
	public bool AdditionalSuppliableCitizenAvailable()
	{
		return (maxCitizen - citizen) >= 5;
	}

	public void IncreaseMaxCitizen(int amount)
	{
		maxCitizen += amount;
	}

	public void DecreaseMaxCitizen(int amount)
	{
		maxCitizen -= amount;
	}

	public void IncreaseCitizen(int amount)
	{
		citizen += amount;
	}

	public void ConstructionOfTheMill()
	{
		millIsBuilt = false;
	}

	public bool MillIsBuild()
	{
		return millIsBuilt;
	}

	public void LoadResources(ExtractSaveGameValues values)
	{
		ResetResources();

		List<int> resources = values.GetResourcesCitizenCount();

		AlterResourceValue(GameResources.Wood, resources[0]);
		AlterResourceValue(GameResources.Stone,resources[1]);
		AlterResourceValue(GameResources.Iron, resources[2]);
		AlterResourceValue(GameResources.Coal, resources[3]);
		AlterResourceValue(GameResources.Gold, resources[4]);
		AlterResourceValue(GameResources.Weapons, resources[5]);
		IncreaseCitizen(resources[6]);
		IncreaseMaxCitizen(resources[7]);
	}

	private void ResetResources()
	{
		Wood	= 0;
		Stone	= 0;
		Iron	= 0;
		Coal	= 0;
		Gold	= 0;
		Weapons = 0;

		citizen = 0;
		maxCitizen = 0;
	}
}