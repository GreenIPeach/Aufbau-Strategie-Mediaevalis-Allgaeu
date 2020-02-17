using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
	[Header("Scene To Load")]
	public string sceneName = "Eisenberg";

	[Header("UI")]
	public GameObject loadingScreenPanel;
	public TextMeshProUGUI hintTextbox;
	public List<TextAsset> hintTexts;

	AsyncOperation async;

	public void StartLoadingScreen()
	{
		loadingScreenPanel.SetActive(true);
		DisplayRandomHint();
		StartCoroutine(LoadAsync(sceneName));
	}

	void DisplayRandomHint()
	{
		int hintTextRange = hintTexts.Count - 1;
		hintTextbox.text = hintTexts[Random.Range(0, hintTextRange)].text;
	}

	IEnumerator LoadAsync(string scene)
	{
		async = SceneManager.LoadSceneAsync(sceneName);
		async.allowSceneActivation = false;

		while (async.isDone == false)
		{
			if (async.progress == 0.9f)
			{
				async.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
