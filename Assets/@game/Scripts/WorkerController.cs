using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class WorkerController : MonoBehaviour {

public ThirdPersonCharacter character;
public NavMeshAgent agent;
public GameObject target;
public GameObject workplace;
private bool idle;
private float destructionSpeed = 1f;
private float destructionTimer;
private float destructionDuration = 5f;

private BuildingWorkers buildingWorkers;
private ResourceManager resourceManager;
private TreeController treeController;

private float minDisToTar = 2.5f;

private int[] stock = new int[2];		//[0] gibt Art an (Bsp. tree), [1] gibt Anzahl an

public float maxSpeed = 1.5f;


	private void Awake()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
		resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
	}
	// Use this for initialization
	
	void Start () 
	{
		idle = true;
		destructionTimer = destructionDuration;
		agent.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (target == null)
		{
			Stand();
			return;
		}
		if (!TargetReached())
		{
			GoToTarget();
		}
		else
		{
			Stand();
			if (!idle)
			{
				if (target != workplace && target != null)
				{
					GetResource();
				}
				else
				{
					SubmitResource();
					SetTarget();
				}
			}
		}
	}

	public void SetWorkplace(GameObject tar)
	{
		workplace = tar;
		buildingWorkers = workplace.GetComponent<BuildingWorkers>();
		idle = false;
		SetTarget(workplace);
	}

	private void GoToTarget()
	{
		character.Move(agent.desiredVelocity, false, false);
		agent.avoidancePriority = 1;
	}

	private void Stand()
	{
		character.Move(Vector3.zero, false, false);
		agent.SetDestination(transform.position);
		agent.avoidancePriority = 2;
	}

	///<summary>
	///Sets a gameObject as target for worker and agent.
	///</summary>
	public void SetTarget(GameObject tar)
	{
		idle = false;
		target = tar;

		if (agent == null)
		{
			return;
		}

		agent.SetDestination(target.transform.position);
		if (stock[1] > 0)
		{
			agent.speed = maxSpeed / 2;
		}
		else
		{
			agent.speed = maxSpeed;
		}

		if (target.layer == 10)
		{
			treeController = target.GetComponent<TreeController>();
			minDisToTar = 1f;
			return;
		}
		else if (target.name.Contains("Bauernhof"))
		{
			minDisToTar = 7f;
			agent.SetDestination(target.transform.GetChild(0).position);
			return;
		}
		else if (target == workplace)
		{
			minDisToTar = 3f;
			return;
		}
		else
		{
			minDisToTar = 2f;
		}
	}

	///<summary>
	///Sets target to "no target" and sets destination to own position
	///</summary>
	public void SetTarget()
	{
		idle = true;
		agent.SetDestination(transform.position);
		target = null;
	}

	public void SetRndDestination()
	{
		Vector3 pivot;
		Vector3 pos;
		if(workplace != null)
		{
			pivot = workplace.transform.position;
		}
		else
		{
			pivot = transform.position;
		}
		pos = new Vector3(pivot.x + (Random.value * 10), pivot.y, pivot.y + (Random.value * 10));
		if (isReachable(pos))
		{
			agent.SetDestination(pos);
		}
	}

	public bool TargetReached()
	{
		if (target != null)
		{
			if ((target.transform.position - transform.position).magnitude <= minDisToTar)
			{
				return true;
			}
		}
		return false;
	}


	private void GetResource()
	{

		if (destructionTimer < destructionDuration)
		{
			destructionTimer -= Time.deltaTime * destructionSpeed;
		}
		
		if (destructionTimer == destructionDuration)
		{
			transform.LookAt(target.transform.position);
			destructionTimer -= Time.deltaTime * destructionSpeed;
		}

		if (destructionTimer <= 0f)
		{
			if (target.layer == 10)
			{
				ChopTree();
			}
			else
			{
				CutResource();
			}

			destructionTimer = destructionDuration;
			
			SetTarget(workplace);
		}
	}

	public void SetIdle()
	{
		idle = true;
		buildingWorkers.idleWorkers.Add(this.gameObject);
	}

	public bool isReachable(GameObject tar)
	{
		NavMeshPath path = new NavMeshPath();
		return agent.CalculatePath(tar.transform.position, path);
	}

	public bool isReachable(Vector3 tarPos)
	{
		NavMeshPath path = new NavMeshPath();
		return agent.CalculatePath(tarPos, path);
	}

	private void CutResource()
	{
		stock[0] = target.layer;
		stock[1] += 10;
		Destroy(target);
	}

	private void ChopTree()
	{
		stock[0] = target.layer;
		stock[1] += 10;
		treeController.treeHealth = 0;
		SetTarget(workplace);
		treeController = null;
	}

	private void SubmitResource()
	{
		switch(stock[0])
		{
			case 10:
				resourceManager.IncreaseResources(GameResources.Wood, stock[1]);
				ClearStock();
				break;
			default:
				break;
		}
		SetIdle();
	}

	private void ClearStock()
	{
		stock[0] = 0;
		stock[1] = 0;
	}

	public void ResetWorker()
	{
		SetTarget();
		treeController = null;
		workplace = null;
		ClearStock();
		SetIdle();
	}

}
