using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProduction : MonoBehaviour
{
    private ResourceManager resourcescript;
    private BuildingWorkers workerassignedscript;

    public int producedFoodValue = 5;

    private int workers = 0;
    private int workersAssigned = 0;

    public bool additionalDependenceFulfilled = true;

    // Use this for initialization
    void Start()
    {
        resourcescript = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        workerassignedscript = gameObject.GetComponent<BuildingWorkers>();
    }

    // Update is called once per frame
    void Update()
    {
        //Should replace this as soon as all buildings work the same way
        // Only count idle workers(these who are at the building)
        workers = workerassignedscript.workers.Count;

        if (additionalDependenceFulfilled)
        {
            if (workers > workersAssigned)
            {
                resourcescript.IncreaseMaxCitizen(producedFoodValue * (workers - workersAssigned));
            }
            else if (workers < workersAssigned)
            {
                resourcescript.DecreaseMaxCitizen(producedFoodValue * (workersAssigned - workers));
            }

            workersAssigned = workers;
        }
        else
        {
            // If the Mill isnt built (better: not enough flour is availabe) the backery cant produce food.
            additionalDependenceFulfilled = ResourceManager.instance.MillIsBuild();
        }
    }
}
