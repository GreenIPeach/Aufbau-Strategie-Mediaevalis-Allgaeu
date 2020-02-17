using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class ScreenShotTaker : MonoBehaviour
{

	public string path;
	public string names;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ScreenShot.TakeScreenShot(path,names);
		}
	}
}
