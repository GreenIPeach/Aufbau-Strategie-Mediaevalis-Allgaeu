using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWorkers : MonoBehaviour {

	
	public List<GameObject> workers = new List<GameObject>();
	
	public List<GameObject> idleWorkers = new List<GameObject>();

	public void AddIdle(GameObject worker)
	{
		idleWorkers.Add(worker);
	}

}
