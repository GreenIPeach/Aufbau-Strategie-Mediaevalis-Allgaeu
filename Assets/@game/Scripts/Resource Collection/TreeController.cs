using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public int treeHealth = 5;
   // public int fallVelocity = 1;
    public int destructionTime = 4;
    private bool isTreeAvailable = true;

    private GameObject tree;
    public Transform logs;

    //private Rigidbody rigid;
	// Use this for initialization

    //-----------------------------------------------------------------------------
    //We don´t use this as the trees are currently not able to be physicle correct
    //-----------------------------------------------------------------------------
/*    void Awake()
    {
        if(gameObject.GetComponent<Rigidbody>() == null)
        {
         rigid = gameObject.AddComponent<Rigidbody>();
         rigid.isKinematic = true;
        }
        else
        {
            rigid = gameObject.GetComponent<Rigidbody>();
        }
    }*/

    void Start ()
    {
    }
        
    // Update is called once per frame
    void Update ()
    {
        if (treeHealth == 0)
        {
            //rigid.isKinematic = false;
            //rigid.AddForce(transform.forward * fallVelocity);
            Destroy(gameObject);
            //Invoke("DestroyTree", destructionTime);
        }
    }

    void DestroyTree()
    {
        

        /* Example Implementation, if one Tree gets split into logs (because the ranger cant bring back the whole tree)
        Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        Instantiate(logs, transform.position + new Vector3(0, 0.1f, 0) + position, Quaternion.identity);
        Instantiate(logs, transform.position + new Vector3(0.2f, 0.2f, 1) + position, Quaternion.identity);
        Instantiate(logs, transform.position + new Vector3(-0.1f, 0.3f, 2) + position, Quaternion.identity);
        */
    }

    public bool IsAvailable()
    {
        return isTreeAvailable;
    }

    public void AssignTree()
    {
        isTreeAvailable = false;
    }
}
