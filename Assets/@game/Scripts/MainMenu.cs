using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[Header("Loading Screen Canvas")]
	public GameObject loadingScreenCanvas;

	[Header("Panels To Open")]
	public GameObject saveLoadPanel;
	public GameObject optionsPanel;

	public void StartGame()
	{
		loadingScreenCanvas.GetComponent<LoadingScreen>().StartLoadingScreen();
		this.gameObject.SetActive(false);
	}

	public void LoadGame()
	{
		saveLoadPanel.SetActive(true);
	}

	public void GameOptions()
	{
		optionsPanel.SetActive(true);
	}

	public void QuitGame()
	{
		Debug.Log("Quit Game!");
		Application.Quit();
	}
}
