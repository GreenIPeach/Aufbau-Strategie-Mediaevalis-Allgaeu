using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class TreeReplacer : MonoBehaviour
{
    public GameObject root;
    public List<GameObject> availableLabbaum = new List<GameObject>();
    public List<GameObject> availableNadelbaum = new List<GameObject>();
    private List<GameObject> Laubbaum = new List<GameObject>();
    private List<GameObject> Nadelbaum = new List<GameObject>();

    public void ChangeTree()
    {

        foreach (Transform child in root.transform)
        {
            if (child.gameObject.name.Contains("Laubbaum"))
            {
                Laubbaum.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
            else if (child.gameObject.name.Contains("Nadelbaum"))
            {
                Nadelbaum.Add((child.gameObject));
                child.gameObject.SetActive(false);
            }
        }

        foreach (var tree in Laubbaum)
        {
            GameObject randomTree = availableLabbaum[Random.Range(0, availableLabbaum.Count - 1)];
            GameObject newTree = Instantiate(randomTree,tree.transform.position,tree.transform.rotation);
            newTree.transform.SetParent(root.transform);
            Vector3 rotation = new Vector3(0, newTree.transform.rotation.eulerAngles.y, 0);
            newTree.transform.localEulerAngles = rotation;
        }

        foreach (var tree in Nadelbaum)
        {
            
            GameObject randomTree = availableNadelbaum[Random.Range(0, availableNadelbaum.Count - 1)];   
            GameObject newTree  = Instantiate(randomTree,tree.transform.position,tree.transform.rotation);
            newTree.transform.SetParent(root.transform);
            Vector3 rotation = new Vector3(0, newTree.transform.rotation.eulerAngles.y, 0);
            newTree.transform.localEulerAngles = rotation;
        }
    }

    public void DeleteOldTrees()
    {

        foreach (var tree in Laubbaum)
        {
            DestroyImmediate(tree);
        }

        foreach (var tree in Nadelbaum)
        {
            DestroyImmediate(tree);
        }
    }

}
