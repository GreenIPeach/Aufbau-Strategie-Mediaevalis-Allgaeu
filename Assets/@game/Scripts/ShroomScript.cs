
using UnityEngine;

public class ShroomScript : MonoBehaviour {

    public GameObject Player;
    [SerializeField]
    private BoxCollider colliderBox;

    private Vector3 dif;
    private float minDist = 2.0f;
    private float timeToActivate = 0f;
	
	// Update is called once per frame
	void Update ()
    {
        timeToActivate -= Time.deltaTime;

        if (timeToActivate < 0f)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            colliderBox.enabled = true;
            dif = transform.position - Player.transform.position;
            if (dif.magnitude < minDist)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
                colliderBox.enabled = false;


                timeToActivate = 10f;
            }
        }
	}
}
