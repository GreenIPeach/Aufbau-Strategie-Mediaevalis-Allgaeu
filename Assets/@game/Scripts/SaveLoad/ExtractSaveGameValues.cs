using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class extracts all data from the saved file by reassembling it into their original type.
/// </summary>
public class ExtractSaveGameValues
{
	private SaveGameValues loadedGame;

	public ExtractSaveGameValues(string saveGameName)
	{
		// TODO: Move this into constructor?
		loadedGame = SaveLoad.Load(saveGameName);
	}

	public Vector3 GetCameraPosition()
	{
		return new Vector3(loadedGame.cameraPosition[0],
							loadedGame.cameraPosition[1],
							loadedGame.cameraPosition[2]);
	}

	public Quaternion GetCameraRotation()
	{
		return new Quaternion(loadedGame.cameraRotation[0],
								loadedGame.cameraRotation[1],
								loadedGame.cameraRotation[2],
								loadedGame.cameraRotation[3]);
	}

	/// <summary>
	/// This method gets the x, y, z values of the building position list.
	/// </summary>
	public List<Vector3> GetBuildingTranslations()
	{
		List<Vector3> buildingTranslations = new List<Vector3>();

		for (int i = 0; i < loadedGame.buildingPositions.Count;)
		{
			Vector3 vec = new Vector3();
			vec.x = loadedGame.buildingPositions[i];
			vec.y = loadedGame.buildingPositions[i + 1];
			vec.z = loadedGame.buildingPositions[i + 2];

			buildingTranslations.Add(vec);
			i += 3;
		}

		return buildingTranslations;
	}

	/// <summary>
	/// This method gets the y- rotation of a building and adds them to a list.
	/// </summary>
	public List<float> GetBuildingRotations()
	{
		return loadedGame.buildingRotations;
	}

	/// <summary>
	/// This method returns the building ID.
	/// </summary>
	public List<int> GetBuildingIds()
	{
		return loadedGame.buildingIds;
	}

	/// <summary>
	/// This method returns the building (GameObject) name
	/// </summary>
	public List<string> GetBuildingNames()
	{
		return loadedGame.buildingNames;
	}

	/// <summary>
	/// Returns the upgrade level of a building.
	/// </summary>
	public List<int> GetBuildingLevel()
	{
		return loadedGame.buildingLvl;
	}

	/// <summary>
	/// Returns the number of workers a building has.
	/// </summary>
	public List<int> GetBuildingWorkers()
	{
		return loadedGame.buildingWorkers;
	}

	/// <summary>
	/// Returns the number of idle workers a building has.
	/// </summary>
	public List<int> GetBuildingIdleWorkers()
	{
		return loadedGame.buildingIdleWorkers;
	}

	/// <summary>
	/// This method returns the user inputted name of the save game.
	/// </summary>
	public string GetSaveGameName()
	{
		return loadedGame.saveGameName;
	}

	/// <summary>
	/// This method returns the saved timestamp.
	/// </summary>
	public int GetTimestamp()
	{
		return loadedGame.timeStamp;
	}

	/// <summary>
	/// Returns the locations of the citizens in x, y, z.
	/// </summary>
	public List<Vector3> GetCitizenLocations()
	{
		List<Vector3> locations = new List<Vector3>();

		for (int i = 0; i < loadedGame.citizenPositions.Count;)
		{
			Vector3 vec = new Vector3();
			vec.x = loadedGame.citizenPositions[i];
			vec.y = loadedGame.citizenPositions[i + 1];
			vec.z = loadedGame.citizenPositions[i + 2];

			locations.Add(vec);
			i += 3;
		}

		return locations;
	}

	/// <summary>
	/// Returns the y- rotation of the citizens.
	/// </summary>
	public List<float> GetCitizenRotation()
	{
		return loadedGame.citizenRotations;
	}

	public List<int> GetCitizenTarget()
	{
		return loadedGame.citizenTarget;
	}

	/// <summary>
	/// Returns the type (3d model) of a citizen.
	/// </summary>
	public List<int> GetCitizenType()
	{
		return loadedGame.citizenType;
	}

	/// <summary>
	/// Returns the amount of citizen, wood, stone, iron, coal and gold.
	/// </summary>
	public List<int> GetResourcesCitizenCount()
	{
		return loadedGame.resourcesCitizenCount;
	}

	/// <summary>
	/// Returns tree locations.
	/// </summary>
	public List<Vector3> GetTreeLocations()
	{
		List<Vector3> locations = new List<Vector3>();

		for (int i = 0; i < loadedGame.treePositions.Count; i++)
		{
			Vector3 vec = new Vector3();
			vec.x = loadedGame.treePositions[i];
			vec.y = loadedGame.treePositions[i + 1];
			vec.z = loadedGame.treePositions[i + 2];

			locations.Add(vec);
		}

		return locations;
	}

	/// <summary>
	/// This method Debug.Logs the values of the building translation, rotation, id and name.
	/// </summary>
	public void PrintBuildingValues()
	{
		List<Vector3> buildingTranslations = GetBuildingTranslations();
		List<float> buildingRots = GetBuildingRotations();
		List<int> buildingIds = GetBuildingIds();
		List<string> buildingNames = GetBuildingNames();
		List<int> buildingLvl = GetBuildingLevel();
		List<int> buildingWorkers = GetBuildingWorkers();
		List<int> buildingIdleWorkers = GetBuildingIdleWorkers();


		Debug.Log("======================================================================================");
		Debug.Log("================================ BUILDINGS ==================================");
		Debug.Log("SaveGame name: " + GetSaveGameName());
		Debug.Log("Time saved: " + GetTimestamp().ToString());

		for (int i = 0; i < GetBuildingTranslations().Count; i++)
		{
			Debug.Log(buildingNames[i] + ", ID: " + buildingIds[i].ToString());
			Debug.Log("Position X: " + buildingTranslations[i].x.ToString() + " Y: " + buildingTranslations[i].y.ToString() + " Z: " + buildingTranslations[i].z.ToString());
			Debug.Log("Rotation: " + buildingRots[i].ToString());
			Debug.Log("Level: " + buildingLvl[i].ToString());
			Debug.Log("Workers: " + buildingWorkers[i].ToString());
			Debug.Log("Idle Workers: " + buildingIdleWorkers[i].ToString());
		}
	}

	public void PrintCameraValues()
	{
		Vector3 camPos = GetCameraPosition();
		Vector3 camRot = GetCameraRotation().eulerAngles;

		Debug.Log("======================================================================================");
		Debug.Log("================================ CAMERA ==================================");
		Debug.Log("Position: " + camPos.x + ", " + camPos.y + ", " + camPos.z);
		Debug.Log("Rotation: " + camRot.x + ", " + camRot.y + ", " + camRot.z);
	}

	public void PrintResourceValues()
	{
		List<int> resources = GetResourcesCitizenCount();

		Debug.Log("======================================================================================");
		Debug.Log("================================ RESOURCES ==================================");
		Debug.Log("Citizen: "	+ resources[0]);
		Debug.Log("Wood: "		+ resources[1]);
		Debug.Log("Stone: "		+ resources[2]);
		Debug.Log("Iron: "		+ resources[3]);
		Debug.Log("Coal: "		+ resources[4]);
		Debug.Log("Gold: "		+ resources[5]);
	}

	public void PrintCitizenValues()
	{
		List<Vector3> citizenLocations = GetCitizenLocations();
		List<float> citizenRotations = GetCitizenRotation();
		List<int> citizenType = GetCitizenType();
		int citizenCount = GetResourcesCitizenCount()[0];

		Debug.Log("======================================================================================");
		Debug.Log("================================ CITIZEN ==================================");

		for (int i = 0; i < citizenCount; i++)
		{
			Debug.Log("Position: " + citizenLocations[i].x + ", " + citizenLocations[i].y + ", " + citizenLocations[i].z);
			Debug.Log("Rotation: " + citizenRotations[i]);
			Debug.Log("Type: " + citizenType[i]);
		}
	}
}
