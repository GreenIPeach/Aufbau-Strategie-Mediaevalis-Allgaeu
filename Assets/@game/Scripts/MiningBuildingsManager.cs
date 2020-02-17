using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBuildingsManager : MonoBehaviour {


	public List<GameObject> quarryPositions = new List<GameObject>();
	public List<GameObject> minePositions = new List<GameObject>();
	public List<bool> isUsedQuarryPositions = new List<bool>();
	public List<bool> isUsedMinePositions = new List<bool>();
	private bool isShowing = false;

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).tag == "QuarryPlace")
			{
				quarryPositions.Add(transform.GetChild(i).gameObject);
				isUsedQuarryPositions.Add(false);
			}
			else
			{
				minePositions.Add(transform.GetChild(i).gameObject);
				isUsedMinePositions.Add(false);
			}
		}
	}

	public void ShowUnusedPositions(GameObject obj)
	{
		if (!isShowing)
		{
			if (obj.name.Contains("Bergwerk"))
			{
				for (int i = 0; i < minePositions.Count; i++)
				{
					if (!isUsedMinePositions[i])
					{
						minePositions[i].GetComponent<MeshRenderer>().enabled = true;
					}
				}
			}

			if (obj.name.Contains("Steinbruch"))
			{
				for (int i = 0; i < quarryPositions.Count; i++)
				{
					if (!isUsedQuarryPositions[i])
					{
						quarryPositions[i].GetComponent<MeshRenderer>().enabled = true;
					}
				}
			}
			isShowing = true;
		}
	}

	public void HidePositions()
	{
		for (int i = 0; i < minePositions.Count; i++)
			{
				if (!isUsedMinePositions[i])
				{
					minePositions[i].GetComponent<MeshRenderer>().enabled = false;
				}
			}
		
		for (int i = 0; i < quarryPositions.Count; i++)
		{
			if (!isUsedQuarryPositions[i])
			{
				quarryPositions[i].GetComponent<MeshRenderer>().enabled = false;
			}
		}
		isShowing = false;
	}

	public void SetActive(GameObject obj)
	{
		if (obj.transform.GetChild(0).name.Contains("Bergwerk"))
		{
			for (int i = 0; i < minePositions.Count; i++)
			{
				if (minePositions[i].gameObject == obj)
				{
					isUsedMinePositions[i] = false;
					minePositions[i].SetActive(true);
					return;
				}
			}
		}
		else
		{
			for (int i = 0; i < quarryPositions.Count; i++)
			{
				if (quarryPositions[i].gameObject == obj)
				{
					isUsedQuarryPositions[i] = false;
					quarryPositions[i].SetActive(true);
					return;
				}
			}
		}
	}

	public void SetInactive(GameObject obj)
	{
		if (obj.transform.GetChild(0).name.Contains("Bergwerk"))
		{
			for (int i = 0; i < minePositions.Count; i++)
			{
				if (minePositions[i].gameObject == obj)
				{
					isUsedMinePositions[i] = true;
					minePositions[i].SetActive(false);
					return;
				}
			}
		}
		else
		{
			for (int i = 0; i < quarryPositions.Count; i++)
			{
				if (quarryPositions[i].gameObject == obj)
				{
					isUsedQuarryPositions[i] = true;
					quarryPositions[i].SetActive(false);
					return;
				}
			}
		}
	}
}
