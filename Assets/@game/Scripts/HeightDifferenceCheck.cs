using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightDifferenceCheck : MonoBehaviour {

	public Vector3[] boxPoints = new Vector3[5];
	
	public Terrain terrain;
	public BoxCollider boxCollider;
	private Vector3 colliderCenter;

	private float maxY;
	private float minY;
	private float diffY;

	void Start()
	{
		//terrain = GameObject.FindObjectOfType<Terrain>();
		boxCollider = gameObject.GetComponent<BoxCollider>();
	}

	public float GetDifference()
	{
		colliderCenter = boxCollider.bounds.center;

		boxPoints[0] = colliderCenter;
		boxPoints[0].y = terrain.SampleHeight(colliderCenter);
		boxPoints[1] = new Vector3(boxCollider.bounds.min.x,0f,boxCollider.bounds.min.z);
		boxPoints[1].y = terrain.SampleHeight(boxPoints[1]);
		boxPoints[2] = new Vector3(boxCollider.bounds.max.x,0f,boxCollider.bounds.min.z);
		boxPoints[2].y = terrain.SampleHeight(boxPoints[2]);
		boxPoints[3] = new Vector3(boxCollider.bounds.min.x,0f,boxCollider.bounds.max.z);
		boxPoints[3].y = terrain.SampleHeight(boxPoints[3]);
		boxPoints[4] = new Vector3(boxCollider.bounds.max.x,0f,boxCollider.bounds.max.z);
		boxPoints[4].y = terrain.SampleHeight(boxPoints[4]);

		maxY = minY = boxPoints[0].y;

		for (int i = 1; i < boxPoints.Length; i++)
		{
			if (boxPoints[i].y < minY)
			{
				minY = boxPoints[i].y;
			}
			if (boxPoints[i].y > maxY)
			{
				maxY = boxPoints[i].y;
			}
		}
		diffY = maxY - minY;
		return diffY;
	}
}
