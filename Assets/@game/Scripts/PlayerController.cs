
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

	public Camera cam;
	public NavMeshAgent agent;
	//public GameObject go;
	//NavMeshHit closestHit;

	//void start ()
	//{
	//    //go = new GameObject("Bottle");
	//    Vector3 sourcePosition = new Vector3(0, 0, -10); //Position, wo er gesetzt werden soll
	//    go.transform.position = closestHit.position;
	//    agent = go.AddComponent<NavMeshAgent>();
	//}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0))
		{
			Debug.Log("Klick!");
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				//MOVE PLAYER
				agent.SetDestination(hit.point);
			}
		}

	}
}
