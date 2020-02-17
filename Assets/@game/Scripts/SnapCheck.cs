using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapCheck : MonoBehaviour 
{
	public List<Collider> quarryColliders = new List<Collider>();
	public List<Collider> mineColliders = new List<Collider>();

	GameObject closestPlace;

	bool didChange = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("QuarryPlace"))
		{
			quarryColliders.Add(other);
		}
		else if (other.CompareTag("GoldPlace") || other.CompareTag("IronPlace") || other.CompareTag("CoalPlace"))
		{
			mineColliders.Add(other);
		}
		didChange = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("QuarryPlace"))
		{
			quarryColliders.Remove(other);
		}
		else if (other.CompareTag("GoldPlace") || other.CompareTag("IronPlace") || other.CompareTag("CoalPlace"))
		{
			mineColliders.Remove(other);
		}
		didChange = true;
	}

	public GameObject GetClosestSnapPlace()
	{
		closestPlace = null;
		int numberClosest = 0;

		if (gameObject.name.Contains("Steinbruch"))
		{
			if (quarryColliders.Count == 0)
			{
				return closestPlace;
			}

			else
			{
				closestPlace = quarryColliders[0].gameObject;
			}

			if (quarryColliders.Count > 1)
			{
				for (int i = 1; i < quarryColliders.Count; i++)
				{
					if ((quarryColliders[i].transform.position - transform.position).magnitude < (quarryColliders[numberClosest].transform.position - transform.position).magnitude)
					{
						numberClosest = i;
					}
				}
			}
			closestPlace = quarryColliders[numberClosest].gameObject;
		}

		else
		{
			if (mineColliders.Count == 0)
			{
				return closestPlace;
			}

			else
			{
				closestPlace = mineColliders[0].gameObject;
			}

			if (mineColliders.Count > 1)
			{
				for (int i = 1; i < mineColliders.Count; i++)
				{
					if ((mineColliders[i].transform.position - transform.position).magnitude < (mineColliders[numberClosest].transform.position - transform.position).magnitude)
					{
						numberClosest = i;
					}
				}
			}
			closestPlace = mineColliders[numberClosest].gameObject;
		}
		didChange = false;
		return closestPlace;
	}

	public bool SomethingChanged()
	{
		return didChange;
	}
}