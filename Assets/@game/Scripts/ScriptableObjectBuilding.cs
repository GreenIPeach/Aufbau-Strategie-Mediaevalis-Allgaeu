using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTyp", menuName = "Scriptable/BuildingTyp", order = 1)]
public class ScriptableObjectBuilding : ScriptableObject
{
    public string buildingName = "New Building";
    [TextArea]
    public string buildingDescription = "Building Description";
    public List<ScriptableBuilding> building;
    public GameObject constructionSite;
    public BuldingCategory category;
    public int requirment;
}
