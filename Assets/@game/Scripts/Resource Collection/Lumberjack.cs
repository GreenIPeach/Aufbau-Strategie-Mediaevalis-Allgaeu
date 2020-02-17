using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : MonoBehaviour
{
    private TreeController treescript;
    private ResourceManager resourcescript;

    public int rayLength = 2;
    public float hitIntervall = 2f;

    public int stock = 0;
    private float elapsed = 0;
    private WorkerController workerController;

    // Use this for initialization
    void Start()
    {
        workerController = GetComponent<WorkerController>();
        resourcescript = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
    }

    public void Chop()
    {
        elapsed += Time.deltaTime;

        RaycastHit hit = new RaycastHit();
        Vector3 fwd = workerController.target.transform.position-transform.position;

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Trees"))
            {
                treescript = workerController.target.GetComponentInChildren<TreeController>();

                if (elapsed >= hitIntervall && treescript.treeHealth > 0)
                {
                    elapsed = elapsed % 1f;

                    treescript.treeHealth -= 1;

                    if (treescript.treeHealth == 0)
                    {
                        CollectWood(10);
                    }
                }
            }
            if (stock != 0)
            {
                SubmitWood();
            }
        }
	}

    public void CollectWood(int amount)
    {
        stock += amount;
    }

    public void SubmitWood()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 fwd = workerController.workplace.transform.position-transform.position;

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength))
        {
            if (hit.collider.gameObject.name == "Forsthuette(Clone)")
            {
                PlacableObject placableObject = workerController.workplace.GetComponent<PlacableObject>();

                if (placableObject.isPlaced == true)
                {
                    resourcescript.IncreaseResources(GameResources.Wood, stock);
                    workerController.SetIdle();
                    stock = 0;
                }
            }
        }
    }
}
