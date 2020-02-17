using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public enum OptionsSelected
{
	Audio,
	Controls,
	Graphics
}

public class OptionsMenu : MonoBehaviour
{
	[Header("Audio")]
	[SerializeField] private GameObject audioOptionsPanel;
	[SerializeField] private GameObject volumeValueControllerGo;
	[SerializeField] private GameObject musicValueControllerGo;
	[SerializeField] private GameObject soundValueControllerGo;
	[SerializeField] private float[] volumeArray =
	{
		-80.0f, -60.0f, -50.0f,
		-40.0f, -30.0f, -20.0f,
		-15.0f, -10.0f, -5.0f,
		0.0f, 5.0f
	};

	[Header("Controls")]
	public GameObject controlsOptionsPanel;

	[Header("PostProcessing")]
	[SerializeField] private GameObject graphicsOptionsPanel;
	[SerializeField] private GameObject brightnessValueControllerGo;
	[SerializeField] private Light mainLight;
	[SerializeField] private Toggle aaToggle;
	[SerializeField] private Toggle aoToggle;
	[SerializeField] private Toggle bloomToggle;
	[SerializeField] private float[] brightnessArray =
	{
		0.5f, 0.6f, 0.7f,
		0.8f, 0.9f, 1.0f,
		1.1f, 1.2f, 1.3f,
		1.4f, 1.5f
	};

	// Audio value controller
	private BarState oldVolumeBarState;
	private BarState oldMusicBarState;
	private BarState oldSoundBarState;

	private ValueController volumeValueController;
	private ValueController musicValueController;
	private ValueController soundValueController;

	// Graphics value controller
	private BarState oldBrightnessBarState;

	private ValueController brightnessValueController;

	private bool oldAaState;
	private bool oldAoState;
	private bool oldBloomState;

	private string settingsSavedKey		= "IsAudioSaved";
	private string masterVolumeKey		= "MasterVolume";
	private string musicVolumeKey		= "MusicVolume";
	private string soundVolumeKey		= "SoundVolume";
	private string brightnessValueKey	= "BrightnessValue";
	private string aaStateKey			= "AaState";
	private string aoStateKey			= "AoState";
	private string bloomStateKey		= "BloomState";

	private OptionsSelected currentSelection;

	private PostProcessProfile postProProfile;
	private PostProcessLayer.Antialiasing aa;
	private PostProcessLayer postProLayer;
	private AmbientOcclusion ao;
	private Bloom bloom;

	private void Awake()
	{
		// PostProcessing
		Camera mainCamera = Camera.main;
		// Get profile from the camera directly so it doesn't have to be assigned twice (this script and the camera),
		// which could lead to problems
		postProProfile	= mainCamera.GetComponent<PostProcessVolume>().profile;
		postProLayer	= mainCamera.GetComponent<PostProcessLayer>();
		// Assign defined settings
		aa				= postProLayer.antialiasingMode;
		ao				= postProProfile.GetSetting<AmbientOcclusion>();
		bloom			= postProProfile.GetSetting<Bloom>();
		
		// Assign the value controllers
		volumeValueController		= volumeValueControllerGo.GetComponent<ValueController>();
		musicValueController		= musicValueControllerGo.GetComponent<ValueController>();
		soundValueController		= soundValueControllerGo.GetComponent<ValueController>();
		brightnessValueController	= brightnessValueControllerGo.GetComponent<ValueController>();

		// Delete all PlayerPrefs. Only necessary when testing.
		//PlayerPrefs.DeleteAll();

		// Load option settings if they were edited before
		if (PlayerPrefs.GetInt(settingsSavedKey, 0) == 1)
		{
			LoadSettings();
		}
	}

	private void OnEnable()
	{
		// Set bar states
		BarState volumeBarState		= volumeValueController.GetBarState();
		BarState musicBarState		= musicValueController.GetBarState();
		BarState soundBarState		= soundValueController.GetBarState();
		BarState brightnessBarState	= brightnessValueController.GetBarState();

		// Update ui
		volumeValueController.SetBarState(volumeBarState);
		musicValueController.SetBarState(musicBarState);
		soundValueController.SetBarState(soundBarState);
		brightnessValueController.SetBarState(brightnessBarState);
		volumeValueController.UpdateBars();
		musicValueController.UpdateBars();
		soundValueController.UpdateBars();
		brightnessValueController.UpdateBars();

		// Update components
		UpdateMixerVolume("MasterVolume", volumeBarState);
		UpdateMixerVolume("MusicVolume", musicBarState);
		UpdateMixerVolume("SoundVolume", soundBarState);
		mainLight.intensity = GetBrightnessFromBarState(brightnessBarState);
		
		// Set old audio and graphic values in case user wants to cancel changes.
		oldVolumeBarState		= volumeValueController.GetBarState();
		oldMusicBarState		= musicValueController.GetBarState();
		oldSoundBarState		= soundValueController.GetBarState();
		oldBrightnessBarState	= brightnessValueController.GetBarState();

		// Set old graphic values
		oldAaState		= postProLayer.antialiasingMode != PostProcessLayer.Antialiasing.None;
		oldAoState		= ao.enabled.value;
		oldBloomState	= bloom.enabled.value;

		aaToggle.isOn		= oldAaState;
		aoToggle.isOn		= oldAoState;
		bloomToggle.isOn	= oldBloomState;
	}

	public void CloseOptions()
	{
		CancelAudioSettings();
		CancelGraphicsSettings();
		// TODO: Check if settings changed and prompt "Unsaved Changes. Really Quit?"
		this.gameObject.SetActive(false);
		PauseMenu.IsPauseMenuCallable = true;
	}

	// Sidebar buttons
	public void AudioSelected()
	{
		currentSelection = OptionsSelected.Audio;
		SwitchOptions();
	}

	public void ControlsSelected()
	{
		currentSelection = OptionsSelected.Controls;
		SwitchOptions();
	}

	public void GraphicsSelected()
	{
		currentSelection = OptionsSelected.Graphics;
		SwitchOptions();
	}

	private void SwitchOptions()
	{
		switch (currentSelection)
		{
			case OptionsSelected.Audio:
				{
					Debug.Log("Audio selected");

					// Deactivate all other panels
					controlsOptionsPanel.SetActive(false);
					graphicsOptionsPanel.SetActive(false);

					audioOptionsPanel.SetActive(true);
					break;
				}
			case OptionsSelected.Controls:
				{
					Debug.Log("Controls selected");

					// Deactivate all other panels
					audioOptionsPanel.SetActive(false);
					graphicsOptionsPanel.SetActive(false);

					controlsOptionsPanel.SetActive(true);
					break;
				}
			case OptionsSelected.Graphics:
				{
					Debug.Log("Graphics selected");

					// Deactivate all other panels
					audioOptionsPanel.SetActive(false);
					controlsOptionsPanel.SetActive(false);

					graphicsOptionsPanel.SetActive(true);
					break;
				}
			default:
				break;
		}
	}

	// Audio Options
	public void SetMusicVolume(float musicVolume)
	{
		AudioMixerManager.instance.SetMixerVolume("MusicVolume", musicVolume);
	}

	public void SetSoundVolume(float soundVolume)
	{
		AudioMixerManager.instance.SetMixerVolume("SoundVolume", soundVolume);
	}

	public void MuteVolume(bool muteState)
	{
		AudioMixerManager.instance.MuteMusic(muteState);
	}

	public void SetBrightness(float brightness)
	{
		mainLight.intensity = brightness;
	}

	public void SetAaToggle(bool aaState)
	{
		aa = aaState
			? PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing
			: PostProcessLayer.Antialiasing.None;
		postProLayer.antialiasingMode = aa;
	}

	public void SetAoToggle(bool aoState)
	{
		ao.enabled.value = aoState;
	}

	public void SetBloomToggle(bool bloomState)
	{
		bloom.enabled.value = bloomState;
	}

	public void SaveSettings()
	{
		// Data saved into player prefs
		PlayerPrefs.SetInt(settingsSavedKey, 1);

		// Audio
		PlayerPrefs.SetInt(masterVolumeKey, (int)volumeValueController.GetBarState());
		PlayerPrefs.SetInt(musicVolumeKey, (int)musicValueController.GetBarState());
		PlayerPrefs.SetInt(soundVolumeKey, (int)soundValueController.GetBarState());

		// Graphic options
		PlayerPrefs.SetInt(brightnessValueKey, (int)brightnessValueController.GetBarState());
		bool isAaEnabled = aa != PostProcessLayer.Antialiasing.None;
		PlayerPrefs.SetInt(aaStateKey, Convert.ToInt32(isAaEnabled));
		PlayerPrefs.SetInt(aoStateKey, Convert.ToInt32(ao.enabled));
		PlayerPrefs.SetInt(bloomStateKey, Convert.ToInt32(bloom.enabled));

		PauseMenu.IsPauseMenuCallable = true;
	}

	private void LoadSettings()
	{
		BarState volumeBarState = (BarState)PlayerPrefs.GetInt(masterVolumeKey, 8);
		BarState musicBarState = (BarState)PlayerPrefs.GetInt(musicVolumeKey, 8);
		BarState soundBarState = (BarState)PlayerPrefs.GetInt(soundVolumeKey, 8);
		BarState brightnessBarState = (BarState)PlayerPrefs.GetInt(brightnessValueKey, 5);

		volumeValueController.SetBarState(volumeBarState);
		musicValueController.SetBarState(musicBarState);
		soundValueController.SetBarState(soundBarState);
		brightnessValueController.SetBarState(brightnessBarState);
		volumeValueController.UpdateBars();
		musicValueController.UpdateBars();
		soundValueController.UpdateBars();
		brightnessValueController.UpdateBars();

		UpdateMixerVolume("MasterVolume", volumeBarState);
		UpdateMixerVolume("MusicVolume", musicBarState);
		UpdateMixerVolume("SoundVolume", soundBarState);
		mainLight.intensity = GetBrightnessFromBarState(brightnessBarState);
		// aa and ao is enabled by default, so if there are no PlayerPrefs default to enabled state
		bool isAaEnabled = PlayerPrefs.GetInt(aaStateKey, 1) == 1 ? true : false;
		bool isAoEnabled = PlayerPrefs.GetInt(aoStateKey, 1) == 1 ? true : false;
		bool isBloomEnabled = PlayerPrefs.GetInt(bloomStateKey, 0) == 1 ? true : false;

		aaToggle.isOn		= isAaEnabled;
		aoToggle.isOn		= isAoEnabled;
		bloomToggle.isOn 	= isBloomEnabled;

		SetAaToggle(isAaEnabled);
		SetAoToggle(isAoEnabled);
		SetBloomToggle(isBloomEnabled);
	}

	public void CancelAudioSettings()
	{
		// Update ui bars
		volumeValueController.SetBarState(oldVolumeBarState);
		musicValueController.SetBarState(oldMusicBarState);
		soundValueController.SetBarState(oldSoundBarState);
		volumeValueController.UpdateBars();
		musicValueController.UpdateBars();
		soundValueController.UpdateBars();

		// Update AudioMixer
		UpdateMixerVolume("MasterVolume", oldVolumeBarState);
		UpdateMixerVolume("MusicVolume", oldMusicBarState);
		UpdateMixerVolume("SoundVolume", oldSoundBarState);

		PauseMenu.IsPauseMenuCallable = true;
	}

	public void CancelGraphicsSettings()
	{
		mainLight.intensity = GetBrightnessFromBarState(oldBrightnessBarState);
		
		aa = oldAaState
			? PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing
			: PostProcessLayer.Antialiasing.None;
		postProLayer.antialiasingMode = aa;
		ao.enabled.value	= oldAoState;
		bloom.enabled.value	= oldBloomState;

		brightnessValueController.SetBarState(oldBrightnessBarState);
		brightnessValueController.UpdateBars();
		
		aaToggle.isOn		= oldAaState;
		aoToggle.isOn		= oldAoState;
		bloomToggle.isOn	= oldBloomState;

		PauseMenu.IsPauseMenuCallable = true;
	}

	/// <summary>
	/// Intended to use to set the volume from an **existing** bar state.
	/// </summary>
	/// <param name="mixerName">"MasterVolume", "MusicVolume" or "SoundVolume".</param>
	public void UpdateMixerVolume(string mixerName)
	{
		ValueController mixerController = new ValueController();

		switch (mixerName)
		{
			case "MasterVolume":
				mixerController = volumeValueController;
				break;
			case "MusicVolume":
				mixerController = musicValueController;
				break;
			case "SoundVolume":
				mixerController = soundValueController;
				break;
			default:
				break;
		}

		if (mixerController == null)
		{
			return;
		}

		float volume = GetVolumeFromBarState(mixerController.GetBarState());

		AudioMixerManager.instance.SetMixerVolume(mixerName, volume);
	}

	/// <summary>
	/// Intended to use to set the volume from a **saved** bar state.
	/// </summary>
	/// <param name="mixerName">"MasterVolume", "MusicVolume" or "SoundVolume".</param>
	/// <param name="state">Bar state to load value from.</param>
	private void UpdateMixerVolume(string mixerName, BarState state)
	{
		float volume = GetVolumeFromBarState(state);

		AudioMixerManager.instance.SetMixerVolume(mixerName, volume);
	}

	public void UpdateBrightnessValue()
	{
		mainLight.intensity = GetBrightnessFromBarState(brightnessValueController.GetBarState());
	}

	private float GetVolumeFromBarState(BarState state)
	{
		float volume = 0.0f;

		switch (state)
		{
			case BarState.ZeroBars:
				volume = volumeArray[0];
				break;
			case BarState.OneBars:
				volume = volumeArray[1];
				break;
			case BarState.TwoBars:
				volume = volumeArray[2];
				break;
			case BarState.ThreeBars:
				volume = volumeArray[3];
				break;
			case BarState.FourBars:
				volume = volumeArray[4];
				break;
			case BarState.FiveBars:
				volume = volumeArray[5];
				break;
			case BarState.SixBars:
				volume = volumeArray[6];
				break;
			case BarState.SevenBars:
				volume = volumeArray[7];
				break;
			case BarState.EightBars:
				volume = volumeArray[8];
				break;
			case BarState.NineBars:
				volume = volumeArray[9];
				break;
			case BarState.TenBars:
				volume = volumeArray[10];
				break;
		}

		return volume;
	}

	private float GetBrightnessFromBarState(BarState state)
	{
		float brightness = 0.0f;

		switch (state)
		{
			case BarState.ZeroBars:
				brightness = brightnessArray[0];
				break;
			case BarState.OneBars:
				brightness = brightnessArray[1];
				break;
			case BarState.TwoBars:
				brightness = brightnessArray[2];
				break;
			case BarState.ThreeBars:
				brightness = brightnessArray[3];
				break;
			case BarState.FourBars:
				brightness = brightnessArray[4];
				break;
			case BarState.FiveBars:
				brightness = brightnessArray[5];
				break;
			case BarState.SixBars:
				brightness = brightnessArray[6];
				break;
			case BarState.SevenBars:
				brightness = brightnessArray[7];
				break;
			case BarState.EightBars:
				brightness = brightnessArray[8];
				break;
			case BarState.NineBars:
				brightness = brightnessArray[9];
				break;
			case BarState.TenBars:
				brightness = brightnessArray[10];
				break;
		}

		return brightness;
	}
}
