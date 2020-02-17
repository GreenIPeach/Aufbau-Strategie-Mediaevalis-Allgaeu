using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
	
	[Header("Main Menu")]
	public string sceneName = "MainMenu";
	public static bool GameIsPaused = false;

	[Header("Save Load")]
	public GameObject saveLoadPanel;

	[Header("Options")]
	public GameObject optionsMenuPanel;

	[Header("MainUI")]
	public GameObject userInterfaceCanvas;

	[Header("Tooltips")]
	public GameObject tooltipGameObject;

	[Header("Loading Screen")]
	public GameObject loadingScreenCanvas;

	private GameObject saveLoadUIDisplay;

	[HideInInspector]
	public static bool IsPauseMenuCallable = true;

	// TODO: Freeze Game
	// TODO: WARNING: Time.timeScale may cause problems in build game!
	// TODO: Make Time.timeScale work

	// TODO: Deactivate user interface

	public void ResumeGame()
	{
		this.gameObject.SetActive(false);
		// Time.timeScale = 1.0f;

		GameIsPaused = false;
		userInterfaceCanvas.SetActive(true);
		CameraController.IsCameraLocked = false;
	}

	public void PauseGame()
	{
		if (IsPauseMenuCallable)
		{
			this.gameObject.SetActive(true);
			// Time.timeScale = 0.0f;

			GameIsPaused = true;
			userInterfaceCanvas.SetActive(false);
			CameraController.IsCameraLocked = true;

			// Hide tooltip here, because when the main ui pause button is clicked
			// it would stick to the mouse
			tooltipGameObject.GetComponent<TooltipHelper>().HideTooltip();
		}
	}

	public void OpenSaveLoad()
	{
		saveLoadPanel.SetActive(true);
		IsPauseMenuCallable = false;
	}

	public void OpenOptions()
	{
		optionsMenuPanel.SetActive(true);
		IsPauseMenuCallable = false;
	}

	public void BackToMainMenu()
	{
		IsPauseMenuCallable = true;
		CameraController.IsCameraLocked = false;
		GameIsPaused = false;
		this.gameObject.SetActive(false);
		loadingScreenCanvas.GetComponent<LoadingScreen>().StartLoadingScreen();
	}
}
