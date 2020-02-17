using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject loadingScreenCanvas;
	[SerializeField]
	private GameObject userInterfaceGo;

	private void OnEnable()
	{
		PauseMenu.IsPauseMenuCallable = false;
	}

	public void ContinueGame()
	{
		this.gameObject.SetActive(false);
		PauseMenu.IsPauseMenuCallable = true;
	}

	public void QuitGame()
	{
		this.gameObject.SetActive(false);
		userInterfaceGo.SetActive(false);
		PauseMenu.IsPauseMenuCallable = true;
		loadingScreenCanvas.GetComponent<LoadingScreen>().StartLoadingScreen();
	}
}
