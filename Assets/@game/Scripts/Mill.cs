using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mill : MonoBehaviour
{
    private PlacableObject placableObjectParent;

    private void Awake()
    {
        GetPlaceableObject();
    }

    void Update()
    {
        if (placableObjectParent == null)
        {
            GetPlaceableObject();
        }

        if (BuildingManager.instance.BuildingPlacment.beenPlaced && placableObjectParent.isPlaced)
        {
            ResourceManager.instance.ConstructionOfTheMill();
            Destroy(this);
        }
    }

    private void GetPlaceableObject()
    {
        placableObjectParent = GetComponentInParent<PlacableObject>();
    }
}
