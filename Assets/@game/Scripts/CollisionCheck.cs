using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour {

	public List<Collider> colliders = new List<Collider>();
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Buildings") || other.CompareTag("Environment") || other.CompareTag("Boundary")||  (other.gameObject.layer == LayerMask.NameToLayer("Trees") ))
		{
			colliders.Add(other);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Buildings") || other.CompareTag("Environment") || other.CompareTag("Boundary") ||  (other.gameObject.layer == LayerMask.NameToLayer("Trees") ))
		{
			colliders.Remove(other);
		}
	}
}
