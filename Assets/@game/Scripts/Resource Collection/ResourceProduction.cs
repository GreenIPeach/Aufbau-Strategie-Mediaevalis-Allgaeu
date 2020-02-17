using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProduction : MonoBehaviour
{
	[SerializeField] private float productionInterval = 10.0f;
	[SerializeField] private int producedResourceValue = 5;
	[SerializeField] private GameResources resourceType;

	[Header("Needed Resources")]
	[SerializeField] private bool doesNeedResources = false;
	[SerializeField] private List<GameResources> neededResourceTypes;
	[SerializeField] private List<int> neededResourceAmounts;

	[Header("Productivity Info")]
	[VisibleOnly] [SerializeField] private bool currentlyProducing = false;
	[VisibleOnly] [SerializeField] private bool neededResourcesAvailable = false;
	
	private BuildingWorkers workerAssignedScript;
	private int workersInIdle = 0;
	private WaitForSeconds waitForSecondsInterval;

	public GameResources ResourceProducing
	{
		get { return resourceType; }
		set { resourceType = value; }
	}

	// Use this for initialization
	void Start()
	{
		workerAssignedScript = gameObject.GetComponent<BuildingWorkers>();

		// Small optimization to save memory allocation on every call
		waitForSecondsInterval = new WaitForSeconds(productionInterval);
	}

	// Update is called once per frame
	void Update()
	{
		if (!currentlyProducing)
		{
			workersInIdle = workerAssignedScript.idleWorkers.Count;

			if (doesNeedResources)
			{
				// Check if needed resources are available atm.
				for (int i = 0; i < neededResourceTypes.Count; i++)
				{
					neededResourcesAvailable = ResourceManager.instance.SpecificResourceAmountCheck(
						neededResourceTypes[i], neededResourceAmounts[i]);

					// If one of the needed resources is not available, break
					if (!neededResourcesAvailable)
					{
						break;
					}
				}
			}
			else
			{
				// If no resources are needed, set this to true to allow the following if- statement to continue
				neededResourcesAvailable = true;
			}

			if (workersInIdle > 0 && neededResourcesAvailable)
			{
				StartCoroutine(ProduceGood());
			}
		}
	}

	private IEnumerator ProduceGood()
	{
		currentlyProducing = true;
		
		// Reduce all processed resources
		if (doesNeedResources)
		{
			for (int i = 0; i < neededResourceTypes.Count; i++)
			{
				ResourceManager.instance.ReduceResources(neededResourceTypes[i], neededResourceAmounts[i]);
			}
		}

		yield return waitForSecondsInterval;

		ResourceManager.instance.IncreaseResources(ResourceProducing, (workersInIdle * producedResourceValue));
		currentlyProducing = false;
	}
}