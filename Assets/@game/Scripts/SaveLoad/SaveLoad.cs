using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public static class SaveLoad
{

	/// <summary>
	/// Saves all gathered values into a binary file with a user specified name.
	/// </summary>
	/// <param name="savables">Gathered values to save.</param>
	/// <param name="saveGameName">Defines the file name.</param>
	public static void Save(GatherSaveGameValues savables, string saveGameName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = new FileStream(SaveLoadHelpers.saveGamePath +
										saveGameName +
										SaveLoadHelpers.saveGameExtension,
										FileMode.Create);

		SaveGameValues game = new SaveGameValues(savables);

		bf.Serialize(file, game);
		file.Close();
	}
	
	/// <summary>
	/// Loads the file depending on the specified name.
	/// </summary>
	/// <param name="saveGameName">Name of the file to load.</param>
	/// <returns> SaveGameValues class with loaded values. </returns>
	public static SaveGameValues Load(string saveGameName)
	{
		FileInfo[] files = SaveLoadHelpers.GetSaveGameFiles();
		string fileToLoad = "";

		bool saveGameExists = false;

		foreach (FileInfo file in files)
		{
			string fileName = (file.Name).Split('@')[0];
			if(fileName == saveGameName)
			{
				fileToLoad = file.Name;
				saveGameExists = true;
			}
		}

		if(saveGameExists)
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = new FileStream(SaveLoadHelpers.saveGamePath + fileToLoad, FileMode.Open);

			// Casting deserialized object to game
			SaveGameValues game = bf.Deserialize(file) as SaveGameValues;
			file.Close();

			return game;
		}
		else
		{
			Debug.LogError("File does not exist!");
			return null;
		}
	}
}