using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forsthuette : MonoBehaviour
{

	private float range = 0.2f;
	
	
	public List<GameObject> treesInRange= new List<GameObject>();
	private BuildingWorkers workers;
	
	private void Awake()
	{
		workers = GetComponent<BuildingWorkers>();

		CapsuleCollider colliderTree = gameObject.transform.GetChild(0).gameObject.AddComponent<CapsuleCollider>();
		colliderTree.radius = range;
		colliderTree.center=new Vector3(0f,0f,0f);
		colliderTree.height = 0.5f;
		colliderTree.isTrigger = true;
		
	}

	private void Start()
	{

		if (workers == null)
		{
			workers = GetComponent<BuildingWorkers>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Trees"))
		{
			treesInRange.Add(other.gameObject);
		}
	}


	private GameObject GetRandomTree()
	{
		return treesInRange[Random.Range(0, treesInRange.Count - 1)];
	}

	private void Update()
	{
		if(workers.idleWorkers.Count>0)
		{
			for (int i = 0; i < workers.idleWorkers.Count; i++)
			{
				//Lauf los!
				var workercontroller = workers.idleWorkers[i].GetComponent<WorkerController>();
				if(treesInRange.Count>0)
				{
					GameObject randomTree = GetRandomTree();
					if (randomTree.GetComponent<TreeController>() != null)
					{
						var treecontroller = randomTree.GetComponent<TreeController>();
						if (treecontroller.IsAvailable()/*  && workercontroller.isReachable(randomTree) */)
						{
							workercontroller.SetTarget(randomTree);
							treecontroller.AssignTree();
							workers.idleWorkers.Remove(workers.idleWorkers[i]);
							treesInRange.Remove(randomTree);
						}
					}
				}
			}
		}
	}
}
