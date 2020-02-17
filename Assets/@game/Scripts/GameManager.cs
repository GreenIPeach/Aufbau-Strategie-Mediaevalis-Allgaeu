using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("PauseMenu")]
	public GameObject pauseMenuCanvas;

	private PauseMenu pauseMenu;

	void Start()
	{
		pauseMenu = pauseMenuCanvas.GetComponent<PauseMenu>();
	}

	// Update is called once per frame
	void Update()
	{
		if (PauseMenu.IsPauseMenuCallable)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (PauseMenu.GameIsPaused)
				{
					pauseMenu.ResumeGame();
				}
				else
				{
					pauseMenu.PauseGame();
				}
			}
		}
	}
}
