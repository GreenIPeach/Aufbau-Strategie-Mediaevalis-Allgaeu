using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Building", menuName = "Scriptable/Building", order = 2)]
public class ScriptableBuilding : ScriptableObject
{

	public GameObject building;
	public int constructionTime;
	public int maxWorkers;
	public int wood;
	public int stone;
	public int iron;
	public int coal;
	public int gold;
	public int weapons;
	public int requirment;
}
