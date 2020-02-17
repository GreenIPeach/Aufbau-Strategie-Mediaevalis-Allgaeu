using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCitizen : MonoBehaviour {

    public int maxCitizen = 5;

    private int spawnedCitizen = 0;
    private Vector3 offset = new Vector3(0,0,-2f);
    private float offsetX;
    private float offsetZ;
    private void Start()
    {
        InvokeRepeating("SpawnTheCitizen",1.0f,1.0f);
    }
	
	
    private void Update()
    {
        if (spawnedCitizen == maxCitizen)
        {
            CancelInvoke();
            Destroy(this);
        }
    }
    public void SpawnTheCitizen()
    {

        if ((spawnedCitizen % 2) == 0)
        {
            offsetX = -0.5f;
        }
        else
        {
            offsetX = +0.5f;
        }
        offsetZ = offsetZ + (-0.3f) * (spawnedCitizen % 2);
        offset = offset + new Vector3(offsetX,0,offsetZ);
        
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(0, Random.Range(90,270), 0);
        //GameObject unit =  Instantiate(CitizenManager.instance.citizenToSpawn[Random.Range(0,CitizenManager.instance.citizenToSpawn.Count-1)], transform.position+offset,Quaternion.identity);
        GameObject unit =  Instantiate(CitizenManager.instance.citizenToSpawn[Random.Range(0,CitizenManager.instance.citizenToSpawn.Count-1)], new Vector3(transform.position.x + offset.x,transform.position.y+offset.y,transform.position.z+ offset.z),rotation);

        //unit.transform.position = transform.position + offset;
        //unit.transform.localEulerAngles = new Vector3(0, Random.Range(90,270), 0);
        CitizenManager.citizenIdle.Add(unit);
        unit.transform.SetParent(CitizenManager.instance.gameObject.transform);
        spawnedCitizen++;

        ResourceManager.instance.IncreaseCitizen(1);
    }
}
