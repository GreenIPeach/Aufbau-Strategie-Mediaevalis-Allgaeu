using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SaveLoadUIDisplay : MonoBehaviour
{
	[Header("Parent Rect and Input Field")]
	//public Transform slotsTargetTransform;
	public TMP_InputField inputSaveNameField;

	[Header("Prompt Panels")]
	public GameObject invalidNamePromptPanel;
	public GameObject saveNameExistsPromptPanel;
	public GameObject deleteSaveGamePromptPanel;

	[Header("Save Slot Instantiation")]
	public GameObject saveSlotPrefab;
	public Transform slotsTargetTransform;

	[Header("Panels To Close")]
	public GameObject pauseMenuCanvas;

	private FileInfo[] saveGameFiles;
	private string saveGameNameInput;
	private bool aToZ = false;
	private bool newToOld = true;
	private bool isInvalidPanelActive = true;
	private bool isSaveExistsPanelActive = true;
	private bool isDeleteSavePanelActive = true;

	private void Awake()
	{
		Directory.CreateDirectory(SaveLoadHelpers.saveGamePath);
		Directory.CreateDirectory(SaveLoadHelpers.screenshotPath);
	}

	private void OnEnable()
	{
		SetSaveGameFiles();
		UpdateSlotsUi();
	}

	// Use this for initialization
	void Start ()
	{
		// Limit the chars that can be written into the input field.
		if (inputSaveNameField != null)
		{
			// Worst case: 36x "W" fits in UI space
			inputSaveNameField.characterLimit = 36;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////
	public void CloseSaveGame()
	{
		this.gameObject.SetActive(false);
		PauseMenu.IsPauseMenuCallable = true;
	}

	/// <summary>
	/// Calls GatherSaveGameValues.SaveGame(...) if save game doesn't exists.
	/// </summary>
	public void SaveGame()
	{
		GetInputFieldText();

		// TODO: Standard sorting order: new to old
		// TODO: Auto- select top (newest save file) button

		// Set string value of user specified save game name as argument
		if (DoesSaveGameExist(saveGameNameInput))
		{
			PromptSaveGameExists();
		}
		else
		{
			if (IsValidName(saveGameNameInput))
			{
				// This method seemingly takes too long, so that the screenshot isn't ready for display, so the workaround would be:
				// TODO: Take temporary screenshot when going into pause menu, change name if player decides to save the game,
				// Delete when he/ she/ it doesn't save or leaves the game.
				TakeScreenshot();
				GatherSaveGameValues.instance.SaveGame(saveGameNameInput);
				SetSaveGameFiles(); // Refresh array because a file was added
				UpdateSlotsUi();
				ClearInputFieldText();
			}
			else
			{
				PromptInvalidName();
			}
		}
	}

	/// <summary>
	/// User agreed to overwrite existing save file.
	/// </summary>
	/// <returns></returns>
	public void OverwriteSaveGame()
	{
		// Deactivate panel when clicking "Yes"
		saveNameExistsPromptPanel.SetActive(false);
		isSaveExistsPanelActive = !isSaveExistsPanelActive;

		TakeScreenshot();
		DeleteExistingSaveFile();
		GatherSaveGameValues.instance.SaveGame(saveGameNameInput);
		SetSaveGameFiles(); // Refresh array because a file was added
		UpdateSlotsUi();
	}

	/// <summary>
	/// Triggers all functions to load data.
	/// </summary>
	public void LoadGame()
	{
		string fileName = saveGameNameInput;
		ExtractSaveGameValues saveGameValues = new ExtractSaveGameValues(fileName);
		CloseUiPanels();

		// TODO: Trigger ALL functions to load data here:
		// Building locations, rotations, ids, ...
		// Citizen locations, rotations, ids, ...
		// Resource locations, remaining count, ...
		// Player gathered resources, citizen count, ...
		// Scripted events, miscellaneous stats

		//saveGameValues.PrintBuildingValues();
		//saveGameValues.PrintCameraValues();
		//saveGameValues.PrintResourceValues();
		//saveGameValues.PrintCitizenValues();

		// Camera
		Camera.main.GetComponent<CameraController>().LoadPositionRotation(saveGameValues);

		// Citizen
		// TODO

		// Buildings
		BuildingManager.instance.LoadAndPlaceBuildings(saveGameValues);

		// Resources
		ResourceManager.instance.LoadResources(saveGameValues);

		// Environment
		// TODO

		// Miscellaneous
		// TODO

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

	public void PromptInvalidName()
	{
		// Gets called again when clicking "Ok" to deactivate
		invalidNamePromptPanel.SetActive(isInvalidPanelActive);
		isInvalidPanelActive = !isInvalidPanelActive;
		Debug.Log("Invalid save game name!");
	}

	public void PromptSaveGameExists()
	{
		// Gets called again when clicking "No" to deactivate
		saveNameExistsPromptPanel.SetActive(isSaveExistsPanelActive);
		isSaveExistsPanelActive = !isSaveExistsPanelActive;
		Debug.Log("Save game already exists!");
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
			GameObject display = Instantiate(saveSlotPrefab);
			display.transform.SetParent(slotsTargetTransform, false);

			string[] substrings = SplitSaveFileName(fileName);
			display.GetComponent<SaveLoadUI>().FillSlot(substrings[0], substrings[1]);
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
	/// Sorts the save files after saved time, from newest to oldest or oldest to newest.
	/// </summary>
	public void SortTime()
	{
		if (newToOld)
		{
			// This lambda expression compares the unix seconds timestamps.
			Array.Sort(saveGameFiles, (x, y) => (
				SplitSaveFileName(Path.GetFileNameWithoutExtension(x.Name))[1]).CompareTo(
				SplitSaveFileName(Path.GetFileNameWithoutExtension(y.Name))[1]));

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
	/// Checks if file already exists in folder.
	/// </summary>
	/// <param name="filePath">Path of file to check.</param>
	/// <returns>True, if the file already exists.</returns>
	bool DoesSaveGameExist(string fileName)
	{
		List<string> fileNames = GetSaveFileNamesFromFolder();
		foreach (string name in fileNames)
		{
			if (name.Equals(fileName))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if string contains invalid tokens.
	/// False, if the string contains invalid tokens or nothing is typed into the input field.
	/// </summary>
	/// <param name="fileName">String to check.</param>
	bool IsValidName(string fileName)
	{
		// @ is a 'verbatim' specifier: special chars doesn't need to be escaped
		string expression = @"^[^\\/:*?<>|@]+$";
		Regex regex = new Regex(expression);

		// Since " doesn't work in the expression string, we have to check the fileName here for that character
		// regex.IsMatch returns false if the fileName contains invalid tokens.
		if (regex.IsMatch(fileName) &&
			!fileName.Contains("\"") &&
			fileName.Length > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
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

	/// <summary>
	/// This method takes a screenshot and saves it.
	/// </summary>
	void TakeScreenshot()
	{
		// TODO: Disable UI when taking a screenshot
		string fileSavePath = SaveLoadHelpers.screenshotPath + saveGameNameInput + SaveLoadHelpers.screenshotExtension;

		ScreenCapture.CaptureScreenshot(fileSavePath);
		Debug.Log(fileSavePath);
	}

	/// <summary>
	/// Close all panels when loading a save.
	/// </summary>
	void CloseUiPanels()
	{
		this.gameObject.SetActive(false);
		pauseMenuCanvas.GetComponent<PauseMenu>().ResumeGame();
		PauseMenu.IsPauseMenuCallable = true;
	}
}
