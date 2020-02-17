using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SaveLoadUI : MonoBehaviour
{
	public TextMeshProUGUI saveGameNameText;
	public TextMeshProUGUI saveGameTimestampText;
	public RawImage screenShotThumbnail;

	private SaveLoadUIDisplay saveLoadDisp;

	////////////////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		saveLoadDisp = FindObjectOfType<SaveLoadUIDisplay>();
	}

	/// <summary>
	/// Fills the save game panel with information: save game name, timestamp and screenshot.
	/// </summary>
	/// <param name="fileName">Save file to load.</param>
	public void FillSlot(string fileName, string timestamp)
	{
		if (saveGameNameText != null)
		{
			saveGameNameText.text = fileName;
		}

		if (screenShotThumbnail != null)
		{
			screenShotThumbnail.texture = LoadPNG(SaveLoadHelpers.screenshotPath + fileName + SaveLoadHelpers.screenshotExtension);
		}

		if (saveGameTimestampText != null)
		{
			saveGameTimestampText.text = ConvertTimestampReadable(timestamp);
		}
	}

	/// <summary>
	/// Gets the currently clicked save game slot save name and sets it to the input field.
	/// </summary>
	public void GetSaveSlotName()
	{
		GameObject saveSlotButton = EventSystem.current.currentSelectedGameObject;
		TextMeshProUGUI saveSlotName = saveSlotButton.GetComponentInChildren<TextMeshProUGUI>();

		if ((saveSlotName.name).Equals("SaveGameName"))
		{
			if (saveSlotName != null)
			{
				saveLoadDisp.SetInputFieldText(saveSlotName.text);
				saveLoadDisp.SetInputFieldTextFromButton();
			}
		}
	}

	/// <summary>
	/// Loads from a path a screenshot was saved to.
	/// </summary>
	/// <param name="filePath">Path of saved screenshot.</param>
	/// <returns>The saved texture.</returns>
	Texture LoadPNG(string filePath)
	{
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filePath))
		{
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData);
		}

		return tex;
	}

	/// <summary>
	/// Converts a raw unix timestamp in format into a readable, displayable format.
	/// </summary>
	/// <param name="timestamp">Raw timestamp to convert.</param>
	/// <returns>Readable timestamp.</returns>
	string ConvertTimestampReadable(string timestamp)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds(Convert.ToInt32(timestamp)).ToLocalTime();

		string date = dateTime.ToString("HHmmssddMMyyyy");
		return	date.Substring(0, 2) + ":" +
				date.Substring(2, 2) + ":" +
				date.Substring(4, 2) + " Uhr, " +
				date.Substring(6, 2) + "." +
				date.Substring(8, 2) + "." +
				date.Substring(10);
	}
}
