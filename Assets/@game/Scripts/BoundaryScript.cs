using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryScript : MonoBehaviour {

	public bool locked;
	private BoxCollider boxCollider;

	void Start () 
	{
		boxCollider = gameObject.GetComponent<BoxCollider>();
		Lock();
	}
	
	public void Unlock()
	{
		boxCollider.enabled = false;
		locked = false;
	}

	public void Lock()
	{
		boxCollider.enabled = true;
		locked = true;
	}

	public BoxCollider GetBoxCollider()
	{
		return boxCollider;
	}
}
