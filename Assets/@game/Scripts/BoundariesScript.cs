using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundariesScript : MonoBehaviour {

	public List<GameObject> boundaries = new List<GameObject>();
	private int numberOfBounds;
	public int upgradeCounter;

	public static BoundariesScript instance;

	private void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		numberOfBounds = transform.childCount;
		upgradeCounter = 0;

		for (int i = 0; i < numberOfBounds; i++)
		{
			boundaries.Add(transform.GetChild(i).gameObject);
		}
		UnlockBoundary(0);
	}

	public void UnlockBoundary(int i)
	{
		//boundaries[i].GetComponent<BoundaryScript>().Unlock();
		UpdateBorders();
		GameObject child = transform.GetChild(i).gameObject;
		child.layer = 0;
		child.tag = "Untagged";
	}

	public void LockBoundary(int i)
	{
		//boundaries[i].gameObject.GetComponent<BoundaryScript>().Lock();
		GameObject child = transform.GetChild(i).gameObject;
		child.layer = LayerMask.NameToLayer("Boundary");
		child.tag = "Boundary";

		UpdateBorders();
	}

	public void UnlockAll()
	{
		for (int i = 0; i < numberOfBounds; i++)
		{
			UnlockBoundary(i);
		}
	}

	public void UnlockNext()
	{
	
		if (upgradeCounter < numberOfBounds)
			{
			UnlockBoundary(++upgradeCounter);
			Debug.Log(upgradeCounter + " is now unlocked");
			return;
		}
		Debug.Log("Every boundary already unlocked!");
	}

	public int GetNumberOfBounds()
	{
		return numberOfBounds;
	}

	/* public BoxCollider GetCollider(int i)
	{
		boundary = boundaries[i].gameObject.GetComponent<BoundaryScript>();
		return boundary.GetBoxCollider();
		BoxCollider coll = boundaries[i].Get
	} */




	private float boundaryXFloor;
	private float boundaryXCeil;

	private float boundaryZFloor;
	private float boundaryZCeil;

	public void UpdateBorders()
	{
		switch(upgradeCounter)
		{
			case 0: 
			boundaryXFloor = boundaries[0].transform.position.x - (0.5f * boundaries[0].transform.localScale.x);
			boundaryXCeil = boundaries[0].transform.position.x + (0.5f * boundaries[0].transform.localScale.x);
			boundaryZFloor = boundaries[0].transform.position.z - (0.5f * boundaries[0].transform.localScale.z);
			boundaryZCeil = boundaries[0].transform.position.z + (0.5f * boundaries[0].transform.localScale.z);
			break;
			case 1:
			boundaryXFloor = boundaries[0].transform.position.x - (0.5f * boundaries[0].transform.localScale.x);
			boundaryZFloor = boundaries[0].transform.position.z - (0.5f * boundaries[0].transform.localScale.z);
			boundaryZCeil = boundaries[0].transform.position.z + (0.5f * boundaries[0].transform.localScale.z);
			boundaryXCeil = boundaries[1].transform.position.x + (0.5f * boundaries[1].transform.localScale.x);
			break;
			case 2:
			case 3:
			boundaryXFloor = boundaries[0].transform.position.x - (0.5f * boundaries[0].transform.localScale.x);
			boundaryZFloor = boundaries[0].transform.position.z - (0.5f * boundaries[0].transform.localScale.z);
			boundaryXCeil = boundaries[2].transform.position.x + (0.5f * boundaries[2].transform.localScale.x);
			boundaryZCeil = boundaries[2].transform.position.z + (0.5f * boundaries[2].transform.localScale.z);
			break;
		}
	}

	public Vector3 CalculatePos(Vector3 pos)
	{
		Vector3 calculatedPos = pos;

		if (upgradeCounter != 2)
		{
			calculatedPos.x = Mathf.Clamp(calculatedPos.x, boundaryXFloor, boundaryXCeil);
			calculatedPos.z = Mathf.Clamp(calculatedPos.z, boundaryZFloor, boundaryZCeil);
		}

		else
		{
			float diffX = calculatedPos.x - (boundaries[0].transform.position.x + (0.5f * boundaries[0].transform.localScale.x));
			float diffZ = calculatedPos.z - (boundaries[0].transform.position.z + (0.5f * boundaries[0].transform.localScale.z));

			if (diffX < 0 && diffZ > 0)
			{
				if (-diffX <= diffZ)
				{
					calculatedPos.x = Mathf.Clamp(calculatedPos.x, boundaries[0].transform.position.x + (0.5f * boundaries[0].transform.localScale.x), boundaryXCeil);
					calculatedPos.z = Mathf.Clamp(calculatedPos.z, boundaryZFloor, boundaryZCeil);
				}
				else
				{

					calculatedPos.x = Mathf.Clamp(calculatedPos.x, boundaryXFloor, boundaryXCeil);
					calculatedPos.z = Mathf.Clamp(calculatedPos.z, boundaryZFloor, boundaries[0].transform.position.z + (0.5f * boundaries[0].transform.localScale.z));
				}
			}
			else
			{
				calculatedPos.x = Mathf.Clamp(calculatedPos.x, boundaryXFloor, boundaryXCeil);
				calculatedPos.z = Mathf.Clamp(calculatedPos.z, boundaryZFloor, boundaryZCeil);
			}
		}
		return calculatedPos;
	}
}
