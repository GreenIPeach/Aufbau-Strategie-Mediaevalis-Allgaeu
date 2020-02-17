using System.Collections.Generic;

[System.Serializable]
public class SaveGameValues
{
	// Camera information
	public float[] cameraPosition = new float[3];
	public float[] cameraRotation = new float[4];

	// Citizen information
	public List<float> citizenPositions	= new List<float>();
	public List<float> citizenRotations	= new List<float>();
	public List<int> citizenTarget		= new List<int>();
	public List<int> citizenType		= new List<int>();

	// Building information
	public List<float> buildingPositions	= new List<float>();
	public List<float> buildingRotations	= new List<float>();
	public List<int> buildingIds			= new List<int>();
	public List<string> buildingNames		= new List<string>();
	public List<int> buildingLvl			= new List<int>();
	public List<int> buildingWorkers		= new List<int>();
	public List<int> buildingIdleWorkers	= new List<int>();

	// Resources
	public List<int> resourcesCitizenCount = new List<int>();

	// Environment
	public List<float> treePositions = new List<float>();

	// Miscellaneous
	public int timeStamp;
	public string saveGameName;

	/// <summary>
	/// This constructor sets all fields ready to be serialized and saved.
	/// </summary>
	/// <param name="saveables">Gathered values to serialize.</param>
	public SaveGameValues(GatherSaveGameValues saveables)
	{
		// Camera
		SetCameraPositionField(saveables);
		SetCameraRotationField(saveables);

		// Citizen
		SetCitizenPositionField(saveables);
		SetCitizenRotationField(saveables);
		SetCitizenTargetField(saveables);
		SetCitizenTypeField(saveables);
		
		// Buildings
		SetBuildingTranslationField(saveables);
		SetBuildingRotationField(saveables);
		SetBuildingIdField(saveables);
		SetBuildingNameField(saveables);
		SetBuildingUpgradeLvlField(saveables);
		SetBuildingIdleWorkersField(saveables);
		SetBuildingWorkersField(saveables);

		// Resources
		SetResourcesCitizenCountField(saveables);

		// Environment
		SetTreePositionField(saveables);
		
		// Miscellaneous
		SetTimestampField(saveables);
		SetSaveGameNameField(saveables);
	}

	////////////////////////////////////////////////////////////////////////////////////
	// Set fields to fill and ready to serialize them

	void SetCameraPositionField(GatherSaveGameValues saveables)
	{
		cameraPosition[0] = saveables.GatherCameraPosition().x;
		cameraPosition[1] = saveables.GatherCameraPosition().y;
		cameraPosition[2] = saveables.GatherCameraPosition().z;
	}

	void SetCameraRotationField(GatherSaveGameValues saveables)
	{
		cameraRotation[0] = saveables.GatherCameraRotation().x;
		cameraRotation[1] = saveables.GatherCameraRotation().y;
		cameraRotation[2] = saveables.GatherCameraRotation().z;
		cameraRotation[3] = saveables.GatherCameraRotation().w;
	}

	void SetBuildingTranslationField(GatherSaveGameValues saveables)
	{
		foreach (var buildingTranslation in saveables.GatherBuildingTranslations())
		{
			buildingPositions.Add(buildingTranslation.x);
			buildingPositions.Add(buildingTranslation.y);
			buildingPositions.Add(buildingTranslation.z);
		}
	}

	void SetBuildingRotationField(GatherSaveGameValues saveables)
	{

		buildingRotations = saveables.GatherBuildingRotations();
	}

	void SetBuildingIdField(GatherSaveGameValues saveables)
	{
		buildingIds = saveables.GatherBuildingIds();
	}

	void SetBuildingNameField(GatherSaveGameValues saveables)
	{
		buildingNames = saveables.GatherBuildingObjectNames();
	}

	void SetBuildingUpgradeLvlField(GatherSaveGameValues saveables)
	{
		buildingLvl = saveables.GatherBuildingUpgradeLevel();
	}

	void SetBuildingWorkersField(GatherSaveGameValues saveables)
	{
		buildingWorkers = saveables.GatherBuildingWorkers();
	}

	void SetBuildingIdleWorkersField(GatherSaveGameValues saveables)
	{
		buildingIdleWorkers = saveables.GatherBuildingIdleWorkers();
	}

	void SetTimestampField(GatherSaveGameValues saveables)
	{
		timeStamp = saveables.GatherCurrentTimestamp();
	}

	void SetSaveGameNameField(GatherSaveGameValues saveables)
	{
		saveGameName = saveables.GatherSaveGameName();
	}

	void SetCitizenPositionField(GatherSaveGameValues saveables)
	{
		foreach (var citizenLocation in saveables.GatherCitizenLocations())
		{
			citizenPositions.Add(citizenLocation.x);
			citizenPositions.Add(citizenLocation.y);
			citizenPositions.Add(citizenLocation.z);
		}
	}

	void SetCitizenRotationField(GatherSaveGameValues saveables)
	{
		citizenRotations = saveables.GatherCitizenRotations();
	}

	void SetCitizenTargetField(GatherSaveGameValues saveables)
	{
		citizenType = saveables.GatherCitizenTarget();
	}

	void SetCitizenTypeField(GatherSaveGameValues saveables)
	{
		citizenType = saveables.GatherCitizenType();
	}

	void SetResourcesCitizenCountField(GatherSaveGameValues saveables)
	{
		resourcesCitizenCount = saveables.GatherResourceAndCitizenCount();
	}

	void SetTreePositionField(GatherSaveGameValues saveables)
	{
		foreach (var treeLocation in saveables.GatherTreeLocations())
		{
			treePositions.Add(treeLocation.x);
			treePositions.Add(treeLocation.y);
			treePositions.Add(treeLocation.z);
		}
	}

}
