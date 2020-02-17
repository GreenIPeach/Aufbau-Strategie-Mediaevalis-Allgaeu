using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

///////////////////////////////////////////////////////////////////////
//
// TODO: Create class to share methods with SaveLoadUIDisplay?
//
///////////////////////////////////////////////////////////////////////
public class LoadUIDisplay : MonoBehaviour
{
	[Header("Input Field")]
	public TMP_InputField inputSaveNameField;

	[Header("Prompt Panels")]
	public GameObject deleteSaveGamePromptPanel;

	[Header("Load Slot Instantiation")]
	public GameObject loadSlotPrefab;
	public Transform slotsTargetTransform;

	private GatherSaveGameValues gatherSaveGameValues;
	private FileInfo[] saveGameFiles;
	private string saveGameNameInput;
	private bool aToZ = false;
	private bool newToOld = true;
	private bool isDeleteSavePanelActive = true;

	private void OnEnable()
	{
		SetSaveGameFiles();
		UpdateSlotsUi();
	}

	////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Triggers all functions to load data.
	/// </summary>
	public void LoadGame()
	{
		string fileName = saveGameNameInput;
		ExtractSaveGameValues saveGameValues = new ExtractSaveGameValues(fileName);

		// TODO: Trigger ALL functions to load data here:
		// Building locations, rotations, ids, ...
		// Citizen locations, rotations, ids, ...
		// Resource locations, remaining count, ...
		// Player gathered resources, citizen count, ...
		// Scripted events, miscellaneous stats

		// TODO: Open loading screen, start game, then load savegame

		//BuildingManager.instance.LoadAndPlaceBuildings(saveGameValues);
		saveGameValues.PrintBuildingValues();

		GameObject cam = GameObject.Find("Main Camera");
		cam.GetComponent<CameraController>().LoadPositionRotation(saveGameValues);

		Debug.Log("Loaded Game!");
	}

	void UpdateSlotsUi()
	{
		// Delete all slots
		foreach (Transform child in slotsTargetTransform)
		{
			if (child != null)
			{
				Destroy(child.gameObject);
				Debug.Log("Destroyed " + child.name);
			}
		}

		// Create new slots
		CreateSlots();
	}

	public void PromptDeleteSaveGame()
	{
		// Gets called again when clicking "No" to deactivate
		deleteSaveGamePromptPanel.SetActive(isDeleteSavePanelActive);
		isDeleteSavePanelActive = !isDeleteSavePanelActive;
		Debug.Log("Delete save game!");
	}

	/// <summary>
	/// Looping through all save game files, instantiates then the panels with the save game information.
	/// </summary>
	public void CreateSlots()
	{
		string fileName;

		foreach (var sg in saveGameFiles)
		{
			fileName = Path.GetFileNameWithoutExtension(sg.Name);
			GameObject display = Instantiate(loadSlotPrefab);
			display.transform.SetParent(slotsTargetTransform, false);

			string[] substrings = SplitSaveFileName(fileName);
			display.GetComponent<LoadUI>().FillSlot(substrings[0], substrings[1]);
		}

		Debug.Log("Created slots");
	}

	/// <summary>
	/// Method called when delete button is pressed. Deletes save files and updates ui slots.
	/// </summary>
	public void DeleteSaveGame()
	{
		GetInputFieldText();
		DeleteSaveFiles();
		SetSaveGameFiles();
		UpdateSlotsUi();
		isDeleteSavePanelActive = false;
		PromptDeleteSaveGame();
		ClearInputFieldText();
	}

	/// <summary>
	/// This method deletes the binary save game file and the screenshot.
	/// </summary>
	void DeleteSaveFiles()
	{
		DeleteExistingSaveFile();

		foreach (FileInfo screenshot in SaveLoadHelpers.GetScreenshotFiles())
		{
			if (Path.GetFileNameWithoutExtension(screenshot.Name) == saveGameNameInput)
			{
				if (File.Exists(SaveLoadHelpers.screenshotPath + saveGameNameInput + SaveLoadHelpers.screenshotExtension))
				{
					screenshot.Delete();
					return; // File to delete found, no need to search further.
				}
			}
		}
	}

	/// <summary>
	/// Delete the existing binary save file. A save file can not be overwritten easily, because of the timestamp appended to it.
	/// </summary>
	void DeleteExistingSaveFile()
	{
		foreach (FileInfo save in saveGameFiles)
		{
			if ((SplitSaveFileName(save.Name)[0]).Equals(saveGameNameInput))
			{
				if (save.Exists)
				{
					save.Delete();
					return; // File to delete found, no need to search further.
				}
				else
				{
					Debug.Log("File does not exist!");
				}
			}
		}
	}

	/// <summary>
	/// Sorts the save games in alphabetical order, from a - z or
	/// from z- a, depending on a boolean.
	/// </summary>
	public void SortAlphabethical()
	{
		if (aToZ)
		{
			Array.Sort(saveGameFiles, (x, y) => 
			String.Compare(
				SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[0],
				SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[0])
				);

			Debug.Log("Sorted A- Z");
			UpdateSlotsUi();
		}
		else
		{
			Array.Reverse(saveGameFiles);

			Debug.Log("Sorted Z- A");
			UpdateSlotsUi();
		}

		// Toggle order when button is pressed twice
		aToZ = !aToZ;
	}

	/// <summary>
	/// Sorts the save files after saved time, from newest to oldest or oldest to newest, depending on a boolean.
	/// </summary>
	public void SortTime()
	{
		if (newToOld)
		{
			// HHmmddMMyyyy format of the timestamp appended to the file name
			// yyyyMMddhhmmss format to convert timestamp to unix time
			// Start of unix time 1. January 1970, 00:00:00.
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			// Unix timestamp: seconds since epoch

			// This lambda expression compares two integers:
			// It gets the name of a save file, splits that string to get the timestamp, the timestamp is then split again into it's time components.
			// Since the time components are in string format, we have to convert them into integers.
			// The time components make up a date time, this date time then gets converted to universal time,
			// by subtracting it with the start of the unix epoch and converting it to total seconds we get a unix timestamp.
			// Since this is returned a double format, we convert it into integer format.
			Array.Sort(saveGameFiles, (x, y) =>
			Convert.ToInt32((((new DateTime(
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1].Substring(8, 4)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1].Substring(6, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1].Substring(4, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1].Substring(0, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1].Substring(2, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1].Substring(2, 2))).ToUniversalTime() - epoch)).TotalSeconds)).CompareTo(
			Convert.ToInt32((((new DateTime(
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1].Substring(8, 4)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1].Substring(6, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1].Substring(4, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1].Substring(0, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1].Substring(2, 2)),
			Convert.ToInt32(SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1].Substring(2, 2))).ToUniversalTime() - epoch)).TotalSeconds)))
			);

			Debug.Log("Sorted new- old");
			UpdateSlotsUi();
		}
		else
		{
			Array.Reverse(saveGameFiles);

			Debug.Log("Sorted old- new");
			UpdateSlotsUi();
		}

		// Toggle order when button is pressed twice
		newToOld = !newToOld;
	}

	/// <summary>
	/// Called when a save game slot is clicked to retrieve the name of the save file which should be loaded.
	/// </summary>
	public void SetInputFieldTextFromButton()
	{
		GameObject saveSlotButton = EventSystem.current.currentSelectedGameObject;
		TextMeshProUGUI saveSlotName = saveSlotButton.GetComponentInChildren<TextMeshProUGUI>();

		if ((saveSlotName.name).Equals("SaveGameName"))
		{
			if (saveSlotName != null)
			{
				saveGameNameInput = saveSlotName.text;
			}
		}

		if (inputSaveNameField != null)
		{
			inputSaveNameField.text = saveGameNameInput;
		}
	}

	/// <summary>
	/// Sets the saveGameNameInput variable to the value of the ui input field text.
	/// </summary>
	void GetInputFieldText()
	{
		if (inputSaveNameField != null)
		{
			saveGameNameInput = inputSaveNameField.text;
		}
	}

	/// <summary>
	/// Sets value of saveGameNameInput.
	/// </summary>
	/// <param name="name">Name to assign to the saveGameNameInput</param>
	public void SetInputFieldText(string name)
	{
		saveGameNameInput = name;
	}

	void ClearInputFieldText()
	{
		inputSaveNameField.text = "";
	}

	/// <summary>
	/// Set save game file array from binary files in folder.
	/// </summary>
	public void SetSaveGameFiles()
	{
		saveGameFiles = SaveLoadHelpers.GetSaveGameFiles();
	}

	List<string> GetSaveFileNamesFromFolder()
	{
		List<string> fileNames = new List<string>();
		DirectoryInfo info = new DirectoryInfo(SaveLoadHelpers.saveGamePath);
		FileInfo[] fileInfo = info.GetFiles();

		foreach (var file in fileInfo)
		{
			fileNames.Add(SplitSaveFileName(file.Name)[0]);
		}

		return fileNames;
	}

	/// <summary>
	/// This method converts the timestamp from a clicked button back to the raw format, in which it was saved.
	/// </summary>
	/// <param name="timestamp">Readable timestamp string to convert.</param>
	string ConvertTimestampRaw(string timestamp)
	{
		return new string(timestamp.Where(c => char.IsDigit(c)).ToArray()); // '=>' is a lambda expression
	}

	/// <summary>
	/// The save file has the format [Name]@[timestamp] to quickly get all needed information for the UI
	/// without loading the save game itself.
	/// </summary>
	/// <param name="fileName">File name to split</param>
	/// <returns>First string file name, second string timestamp</returns>
	string[] SplitSaveFileName(string fileName)
	{
		return fileName.Split('@');
	}
}
