using System.IO;
using UnityEngine;

public static class SaveLoadHelpers
{
	// Create a folder in "..\Users\[Username]\AppData\LocalLow\DefaultCompany\HS_Kempten_Aufbau - Strategie\Savegames" if it not already exists
	public static string saveGamePath = Application.persistentDataPath +
										Path.DirectorySeparatorChar +
										"Savegames" +
										Path.DirectorySeparatorChar;
	// Create a folder in "..\Users\[Username]\AppData\LocalLow\DefaultCompany\HS_Kempten_Aufbau - Strategie\Screenshots" if it not already exists
	public static string screenshotPath = Application.persistentDataPath +
										Path.DirectorySeparatorChar +
										"Screenshots" +
										Path.DirectorySeparatorChar;
	public static string saveGameExtension = ".sav";
	public static string screenshotExtension = ".png";

	public static FileInfo[] GetScreenshotFiles()
	{
		DirectoryInfo info = new DirectoryInfo(screenshotPath);
		return info.GetFiles();
	}

	public static FileInfo[] GetSaveGameFiles()
	{
		DirectoryInfo info = new DirectoryInfo(saveGamePath);
		return info.GetFiles();
	}
}
