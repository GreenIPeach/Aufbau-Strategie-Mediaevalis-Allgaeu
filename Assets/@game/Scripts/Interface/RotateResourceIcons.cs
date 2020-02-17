using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateResourceIcons : MonoBehaviour
{
	private Camera mainCamera;

	private void Awake()
	{
		mainCamera = Camera.main;
	}


	// Update is called once per frame
	void Update ()
	{
		Quaternion camRot = mainCamera.transform.rotation;
		
		transform.LookAt(transform.position + camRot * Vector3.back,
						camRot * Vector3.up);
	}
}
