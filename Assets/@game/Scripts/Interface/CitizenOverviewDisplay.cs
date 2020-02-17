using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CitizenOverviewDisplay : MonoBehaviour
{
	public static CitizenOverviewDisplay instance;

	[Header("General")]
	[SerializeField] private TextMeshProUGUI citizenText;
	[SerializeField] private TextMeshProUGUI maxCitizenText;
	[SerializeField] private TextMeshProUGUI idleCitizenText;

	[Header("Citizen Distribution")]
	[SerializeField] private TextMeshProUGUI lumberjackText;
	[SerializeField] private TextMeshProUGUI quarryText;
	[SerializeField] private TextMeshProUGUI smithText;
	[SerializeField] private TextMeshProUGUI farmersText;
	[SerializeField] private TextMeshProUGUI millersText;
	[SerializeField] private TextMeshProUGUI bakersText;
	[SerializeField] private TextMeshProUGUI minersText;
	[SerializeField] private TextMeshProUGUI doctorsText;

	int countLumberjacks;
	int countQuarryWorkers;
	int countSmiths;
	int countFarmers;
	int countMillers;
	int countBakers;
	int countMiners;
	int countDoctors;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one CitizenOverviewDisplay in scene!");
			return;
		}

		instance = this;
	}

	private void OnEnable()
	{
		citizenText.text	= ResourceManager.instance.Citizen.ToString();
		maxCitizenText.text = ResourceManager.instance.MaxCitizen.ToString();
		idleCitizenText.text= CitizenManager.citizenIdle.Count.ToString();

		lumberjackText.text	= countLumberjacks.ToString();
		quarryText.text		= countQuarryWorkers.ToString();
		smithText.text		= countSmiths.ToString();
		farmersText.text	= countFarmers.ToString();
		millersText.text	= countMillers.ToString();
		bakersText.text		= countBakers.ToString();
		minersText.text		= countMiners.ToString();
		doctorsText.text	= countDoctors.ToString();
	}

	/// <summary>
	/// Changes the count of the different professions in the citizen overview.
	/// Accepts the GameObject name to add the correct profession.
	/// </summary>
	/// <param name="profession">Checks the name to add to the correct profession.</param>
	/// <param name="plusMinus">+1 adds 1 to the count, -1 substracts 1 from the count.</param>
	public void ChangeWorkerCount(string profession, int plusMinus)
	{
		if (profession.Contains("Forsthuette"))
		{
			countLumberjacks += plusMinus;
			if (countLumberjacks < 0)
			{
				countLumberjacks = 0;
			}
		}
		else if (profession.Contains("Steinbruch"))
		{
			countQuarryWorkers += plusMinus;
			if (countQuarryWorkers < 0)
			{
				countQuarryWorkers = 0;
			}
		}
		else if (profession.Contains("Schmiede"))
		{
			countSmiths += plusMinus;
			if (countSmiths < 0)
			{
				countSmiths = 0;
			}
		}
		else if (profession.Contains("Bauernhof"))
		{
			countFarmers += plusMinus;
			if (countFarmers < 0)
			{
				countFarmers = 0;
			}
		}
		else if (profession.Contains("Muehle"))
		{
			countMillers += plusMinus;
			if (countMillers < 0)
			{
				countMillers = 0;
			}
		}
		else if (profession.Contains("Baeckerei"))
		{
			countBakers += plusMinus;
			if (countBakers < 0)
			{
				countBakers = 0;
			}
		}
		else if (profession.Contains("Bergwerk"))
		{
			countMiners += plusMinus;
			if (countMiners < 0)
			{
				countMiners = 0;
			}
		}
		else if (profession.Contains("Pesthaus"))
		{
			countDoctors += plusMinus;
			if (countDoctors < 0)
			{
				countDoctors = 0;
			}
		}
	}
}
