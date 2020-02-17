using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : MonoBehaviour
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
        
        if(BuildingManager.instance.BuildingPlacment.beenPlaced && placableObjectParent.isPlaced)
        {
            BuildingManager.instance.CityHallLevel = placableObjectParent.GetUpgradeLevel();
            Destroy(this);
        }
    }

    private void GetPlaceableObject()
    {
        placableObjectParent = GetComponentInParent<PlacableObject>();
    }
}
