using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenManager : MonoBehaviour {

	public static CitizenManager instance;

	[Header("Bindings")] public List<GameObject> citizenToSpawn = new List<GameObject>();
	public static List<GameObject> citizenIdle = new List<GameObject>();

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one UnitManager in scene!");
			return;
		}

		instance = this;
	}
}
