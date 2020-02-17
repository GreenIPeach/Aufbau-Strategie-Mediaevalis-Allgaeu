using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
	public static AudioMixerManager instance;

	[Header("Audio Mixer")]
	[SerializeField]
	private AudioMixer masterAudioMixer;

	[Header("Music")]
	[SerializeField]
	private GameObject musicGo;
	[SerializeField]
	private AudioClip[] musicClips;
	[SerializeField]
	private float pauseBetweenTracks = 25.0f;
	private AudioSource musicSource;

	[Header("Music Info")]
	[SerializeField]
	[VisibleOnly] private string trackName;
	[SerializeField]
	[VisibleOnly] private float trackLength;
	[SerializeField]
	[VisibleOnly] private float remainingPlaytime;
	[SerializeField]
	[VisibleOnly] private float pauseTimer;
	private float remainingTimer;
	private bool isPlaying;

	[Header("Sound")]
	[SerializeField]
	private List<GameObject> buildingSoundGos;
	[SerializeField]
	private List<GameObject> buildingUpgradeSoundGos;
	[SerializeField]
	private List<GameObject> soundEffectGos;

	private List<AudioSource> buildingSoundSources;
	private List<AudioSource> buildingUpgradeSoundSources;
	private List<AudioSource> soundEffectSources;

	private float volumeBeforeMute;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one AudioMixerManager in scene!");
			return;
		}
		instance = this;

		// Sound
		buildingSoundSources = new List<AudioSource>();
		buildingUpgradeSoundSources = new List<AudioSource>();
		soundEffectSources = new List<AudioSource>();

		// Assign the Audio Sources from the game objects.
		musicSource = musicGo.GetComponent<AudioSource>();

		for (int i = 0; i < buildingSoundGos.Count; i++)
		{
			buildingSoundSources.Add(buildingSoundGos[i].GetComponent<AudioSource>());
		}

		for (int i = 0; i < buildingUpgradeSoundGos.Count; i++)
		{
			buildingUpgradeSoundSources.Add(buildingUpgradeSoundGos[i].GetComponent<AudioSource>());
		}

		for (int i = 0; i < soundEffectGos.Count; i++)
		{
			soundEffectSources.Add(soundEffectGos[i].GetComponent<AudioSource>());
		}

		// Music
		remainingTimer = 0.0f;
		pauseTimer = 0.0f;
		isPlaying = true;

		musicSource = musicGo.GetComponent<AudioSource>();
		musicSource.loop = false;
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (!musicSource.isPlaying)
		{
			pauseTimer += Time.deltaTime;

			if (isPlaying)
			{
				isPlaying = false;
				pauseTimer = 0.0f;

				musicSource.clip = GetRandomClip();

				// Info
				trackName = musicSource.clip.name;
				trackLength = musicSource.clip.length;
				remainingPlaytime = trackLength;

				// Reset timer when new clip started playing
				remainingTimer = 0.0f;

				musicSource.Play();
			}

			if (pauseTimer >= pauseBetweenTracks)
			{
				isPlaying = true;
			}
			else
			{
				isPlaying = false;
			}
		}

		remainingTimer += Time.deltaTime;
		remainingPlaytime = trackLength - remainingTimer;
	}

	/// <summary>
	/// Plays the sound dependent on the inputted string.
	/// </summary>
	/// <param name="name">The name of the Game Object or the specific sound name.</param>
	public void PlaySound(string gameObjectOrSound)
	{
		string soundName;

		if (gameObjectOrSound.Contains("Rathaus"))
		{
			soundName = "CityHall";
		}
		else if (gameObjectOrSound.Contains("Castle"))
		{
			soundName = "Castle";
		}
		else if (gameObjectOrSound.Contains("Bauernhof"))
		{
			soundName = "Farm";
		}
		else if (gameObjectOrSound.Contains("Forsthuette"))
		{
			soundName = "Lumberjack";
		}
		else if (gameObjectOrSound.Contains("Steinbruch"))
		{
			soundName = "Quarry";
		}
		else if (gameObjectOrSound.Contains("Bergwerk"))
		{
			soundName = "Mine";
		}
		else if (gameObjectOrSound.Contains("Baecker"))
		{
			soundName = "Bakery";
		}
		else if (gameObjectOrSound.Contains("Muehle"))
		{
			soundName = "Mill";
		}
		else if (gameObjectOrSound.Contains("Wohnhaus"))
		{
			soundName = "House";
		}
		else if (gameObjectOrSound.Contains("Pesthaus"))
		{
			soundName = "Plaguehouse";
		}
		else if (gameObjectOrSound.Contains("Build"))
		{
			soundName = "Built";
		}
		else if (gameObjectOrSound.Contains("Destroy"))
		{
			soundName = "Destroy";
		}
		else if (gameObjectOrSound.Contains("Upgrade"))
		{
			soundName = "General";
		}
		else if (gameObjectOrSound.Contains("Lvl_1"))
		{
			soundName = "CastleLvl1";
		}
		else if (gameObjectOrSound.Contains("Lvl_2"))
		{
			soundName = "CastleLvl2";
		}
		else if (gameObjectOrSound.Contains("Lvl_3"))
		{
			soundName = "CastleLvl3";
		}
		else
		{
			soundName = "General";
		}

		AudioSource foundAudioClip = new AudioSource();
		bool isAlreadyFound = false;

		for (int i = 0; i < buildingSoundGos.Count; i++)
		{
			if (soundName == buildingSoundGos[i].name)
			{
				foundAudioClip = buildingSoundSources[i];
				isAlreadyFound = true;
				break;
			}
		}

		if (!isAlreadyFound) // Small optimization
		{
			for (int i = 0; i < buildingUpgradeSoundGos.Count; i++)
			{
				if (soundName == buildingUpgradeSoundGos[i].name)
				{
					foundAudioClip = buildingUpgradeSoundSources[i];
					break;
				}
			}

			for (int i = 0; i < soundEffectGos.Count; i++)
			{
				if (soundName == soundEffectGos[i].name)
				{
					foundAudioClip = soundEffectSources[i];
					break;
				}
			}
		}

		if (foundAudioClip != null)
		{
			foundAudioClip.Play();
		}
		else
		{
			Debug.Log("No audio clip found");
		}
	}

	/// <summary>
	/// Sets the volume of an audio mixer group depending on the mixer name given.
	/// </summary>
	/// <param name="mixerName">"MasterVolume", "MusicVolume" or "SoundVolume".</param>
	/// <param name="volume">Volume value.</param>
	public void SetMixerVolume(string mixerName, float volume)
	{
		// The fader value (-80db - 0db) is a logarithmic scale and the slider value is linear.
		masterAudioMixer.SetFloat(mixerName, volume);
	}

	/// <summary>
	/// Returns the volume of the audio mixer groups.
	/// </summary>
	/// <param name="mixerName">"MasterVolume", "MusicVolume" or "SoundVolume".</param>
	public float GetMixerVolume(string mixerName)
	{
		float value;
		bool result = masterAudioMixer.GetFloat(mixerName, out value);

		if (result)
		{
			return value;
		}
		else
		{
			return 0.0f;
		}
	}

	/// <summary>
	/// Toggles the music volume between 0 and an old volume of the music audio mixer group.
	/// </summary>
	/// <param name="isMuted">Current mute state.</param>
	/// <returns></returns>
	public void MuteMusic(bool isMuted)
	{
		if (isMuted)
		{
			volumeBeforeMute = GetMixerVolume("MusicVolume");
			masterAudioMixer.SetFloat("MusicVolume", -80.0f);
		}
		else
		{
			masterAudioMixer.SetFloat("MusicVolume", volumeBeforeMute);
		}
	}

	private AudioClip GetRandomClip()
	{
		return musicClips[Random.Range(0, musicClips.Length)];
	}
}
