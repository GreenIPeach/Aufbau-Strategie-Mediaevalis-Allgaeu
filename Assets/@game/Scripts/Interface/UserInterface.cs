using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
	[SerializeField]
	private Transform scrollViewContent;
	[SerializeField] 
	private GameObject categoryPanel;
	
	[Header("Prefabs")]
	[SerializeField]
	private GameObject buildingBtn;
	[SerializeField]
	private GameObject categoryBtn;

	[Header("Pause Menu")]
	public GameObject pauseMenuCanvas;

	[Header("BuildingUI")]
	[SerializeField] private GameObject buildingUi;

	[Header("Text Bindings")]
	//replaced
	//[SerializeField]
	//private TextMeshProUGUI citizenMax;
	[SerializeField] 
	private TMP_FontAsset font;
	[Header("Screen Bindings")]
	[SerializeField] 
	private GameObject buildingTree;
	[SerializeField] 
	private GameObject citizenOverview;
	[SerializeField] 
	private GameObject blur;


	private BuldingCategory category;

	public static UserInterface instance;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one UserInterface in scene!");
			return;
		}
		instance = this;
	}

	private void Start()
	{
		foreach (var category in Enum.GetValues(typeof(BuldingCategory)))
		{
			var button = Instantiate(categoryBtn, categoryPanel.transform);
			button.name = category.ToString() + "Button";
			button.GetComponentInChildren<TextMeshProUGUI>().text = category.ToString();
			
			button.GetComponent<Button>().onClick.AddListener(()=>UpdateCategory(category));
		}
		UpdateCategory(BuldingCategory.Hauptgebäude);
	}

	public void UpdateCategory(object category)
	{


		foreach (Transform child in scrollViewContent.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		int i = 0;
		float tmpPos = 0;
		foreach (var building in BuildingManager.instance.buildings)
		{
			if(building.category.Equals(category) && (building.requirment <= BuildingManager.instance.CityHallLevel) && (building.requirment > -2))
			{
				switch (i)
				{
						case 0:
							tmpPos = 0f;
							break;
						case 1:
							tmpPos = 0.3f;
							break;
						case 2:
							tmpPos = 1.8f;
							break;
				}
				//Also enable this
				//float theta = (2 * Mathf.PI / 3) * tmpPos ;
				//float xPos = Mathf.Sin(theta);
				//float yPos = Mathf.Abs(Mathf.Cos(theta));
				var button = Instantiate(buildingBtn, transform);
				button.name = building.name + "Button";
				button.GetComponentInChildren<TextMeshProUGUI>().text = building.name;
				button.transform.SetParent(scrollViewContent.transform);
				//Enable this if we have buttons for selecting buildings
				//Layout Group at "Content" must be disabled
				//button.transform.position = new Vector3(xPos,yPos,0f) * 100f + pressedBtn.transform.position;

				button.GetComponent<Button>().onClick.AddListener(() => BuildingManager.instance.SetBuilding(building));

				BuildingUI buildUi = buildingUi.GetComponent<BuildingUI>();

				if (buildUi != null)
				{
					int id = GetBuildingIdByName(building.buildingName);
					
					button.GetComponent<CustomHoverEvent>().onHoverEnter.AddListener(() => buildUi.ShowCost(id));
					button.GetComponent<CustomHoverEvent>().onHoverExit.AddListener(() => buildUi.ShowCostPanel(false));
				}
				
				i++;
			}
		}
	}
	
	private void Update()
	{
		//replaced
		//citizenMax.text = "Untätige Bewohner: "+ CitizenManager.citizenIdle.Count.ToString();
	}

	public void BuildingTree(bool trigger)
	{
		buildingTree.SetActive(trigger);
		blur.SetActive(trigger);
		PauseMenu.IsPauseMenuCallable = !trigger;
		CameraController.IsCameraLocked = trigger;
	}
	
	public void CitizenOverview(bool trigger)
	{
		citizenOverview.SetActive(trigger);
		blur.SetActive(trigger);
		PauseMenu.IsPauseMenuCallable = !trigger;
		CameraController.IsCameraLocked = trigger;
	}

	public void OpenPauseMenu()
	{
		pauseMenuCanvas.GetComponent<PauseMenu>().PauseGame();
	}

	public void DisplayMessage(string text)
	{
		GameObject messageText = new GameObject();
		messageText.transform.SetParent(transform);
		messageText.transform.localPosition = Vector3.zero;
		var textmeshpro  = messageText.AddComponent<TextMeshProUGUI>();
		textmeshpro.text = text;
		textmeshpro.color = Color.red;
		textmeshpro.font = font;
		textmeshpro.alignment = TextAlignmentOptions.Midline;
		textmeshpro.raycastTarget = false;
		messageText.GetComponent<RectTransform>().sizeDelta = new Vector2(1000,500);
		StartCoroutine(FadeText(textmeshpro, messageText));
	}

	IEnumerator FadeText(TextMeshProUGUI text, GameObject messageText)
	{
		text.color = new Color(text.color.r,text.color.g,text.color.b,1);
		while (text.color.a > 0.0f)
		{
			text.color = new Color(text.color.r,text.color.g,text.color.b,text.color.a - (Time.deltaTime/2));
			yield return null;
		}
		Destroy(messageText);
	}

	private int GetBuildingIdByName(string buildingName)
	{
		int id = 0;
		
		switch (buildingName)
		{
			case "Bäckerei":
				id = 0;
				break;
			case "Bauernhof":
				id = 1;
				break;
			case "Bergwerk":
				id = 2;
				break;
			case "Forsthütte":
				id = 3;
				break;
			case "Mühle":
				id = 4;
				break;
			case "Pesthaus":
				id = 5;
				break;
			case "Rathaus":
				id = 6;
				break;
			case "SchmiedeSchmelze":
				id = 7;
				break;
			case "Steinbruch":
				id = 8;
				break;
			case "Wohnhaus":
				id = 9;
				break;
			case "Burg":
				id = 10;
				break;
			default:
				id = 6; // Town hall doesn't cost anything to build
				break;
		}

		return id;
	}
}
