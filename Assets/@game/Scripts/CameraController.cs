using UnityEngine;


public class CameraController : MonoBehaviour
{
	[Header("Speed 'n Stuff")]
	public float panSpeed = 10f;			//min movement speed
	private float panVarSpeed;				//making camera movement speed depending on height
	public float scrollSpeed = 2f;
	public float rotSpeed = 5f;
	public float panBorderThickness = 10f;
	public float wasdSpeed = 0.3f;
	private float wasdSpeedDefault;
	[Header("Distances and rotation")]
	public float minY = 1f;
	public float maxY = 80f;
	public float rotCeil = 5f;
	public float offsetY = 0f;	
	private float difMinYRotCeil;
	private float startRotX;
	public float minRotX = 20f;
	public float borderFloorX = 50f;
	public float borderCeilX = 450f;
	public float borderFloorZ = 50f;
	public float borderCeilZ = 450f;
	[Header("Bools")]
	private bool moveViaBorder = true;
	private bool onBottom = false;
	private bool activeDrag = false;

	private Vector3 diffPositions;

	private Vector3 dragOrigin;
	private Vector3 difToOrigin;

	public Terrain terrain;
	private float terrainHeight;
	public BoundariesScript boundariesScript;

	private int mouseButtonRotate = 1;
	private int mouseButtonDrag = 2;
	
	[HideInInspector]
	public static bool IsCameraLocked = false;

	[SerializeField] private LayerMask ignoredLayers;
	private Camera mainCamera;

	void Awake()
	{
		startRotX = transform.rotation.eulerAngles.x;
		difMinYRotCeil = rotCeil - minY;
		wasdSpeedDefault = wasdSpeed;
		mainCamera = Camera.main;
	}

	void Start()
	{
		offsetY = transform.position.y - terrain.SampleHeight(transform.position);
		panVarSpeed = (offsetY - minY) / (maxY - minY);
		boundariesScript = GameObject.Find("OpenableBoundaries").GetComponent<BoundariesScript>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			scrollSpeed = 4;
		}

		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			scrollSpeed = 1;
		}
	}

	void LateUpdate()
	{
		if (IsCameraLocked)
		{
			return;
		}

		TranslateCam();
		RotateCam();
	}

	//===============================================camera translation via mouse or keyboard===============================================
	private void TranslateCam()
	{
		Vector3 newPos = transform.position;
		terrainHeight = terrain.SampleHeight(newPos);

		if(Input.GetMouseButton(mouseButtonDrag) && newPos.y >= (rotCeil + terrainHeight))			//Movement by dragging (klick on map and drag around)
		{
			Ray ray;
			RaycastHit hitDrag;

			if(!activeDrag)
			{
				if (Input.GetMouseButtonDown(mouseButtonDrag))
				{
					ray = mainCamera.ScreenPointToRay(Input.mousePosition);
					if(Physics.Raycast(ray , out hitDrag, 2000, ~ignoredLayers))
					{
						if (hitDrag.collider.gameObject.layer == 8)
						{
							dragOrigin = hitDrag.point;
							activeDrag = true;
						}
					}
				}
			}
			
			else
			{
				ray = mainCamera.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray , out hitDrag, 2000, ~ignoredLayers))
				{
					difToOrigin =(hitDrag.point - dragOrigin);
					difToOrigin = -difToOrigin;
				}

				if (difToOrigin.magnitude > 0.01f)
				{
					newPos +=  new Vector3 (difToOrigin.x, 0, difToOrigin.z);
				}
			}
		}
		
		else
		{
			activeDrag = false;
			Vector3 currDir = transform.forward;						//current direction of camera (view direction)
			currDir.Normalize();										//projection-vector should alwasys be '1', so it's useable for speed 'n stuff
			currDir *= panSpeed * Time.deltaTime + (20f * panVarSpeed);

			if (newPos.y < terrainHeight + rotCeil)
			{
				moveViaBorder = false;
			}

			else
			{
				moveViaBorder = true;
			}

			//if (Input.GetKey("w")||Input.GetKey("a")||Input.GetKey("s")||Input.GetKey("d"))
			if (Input.GetKey("w") || (moveViaBorder && Input.mousePosition.y >= Screen.height - panBorderThickness)||Input.GetKey("s") || (moveViaBorder && Input.mousePosition.y <= panBorderThickness)||Input.GetKey("d") || (moveViaBorder && Input.mousePosition.x >= Screen.width - panBorderThickness)||Input.GetKey("a") || (moveViaBorder && Input.mousePosition.x <= panBorderThickness))
			{
				if (wasdSpeed < 1)
				{
					wasdSpeed += Time.deltaTime;
					if (wasdSpeed > 1f)
					{
						wasdSpeed = 1f;
					}
				}

				if (Input.GetKey("w") || (moveViaBorder && Input.mousePosition.y >= Screen.height - panBorderThickness))
				{
					newPos.z += currDir.z * wasdSpeed;	//forward is z
					newPos.x += currDir.x * wasdSpeed;
				}
				if (Input.GetKey("s") || (moveViaBorder && Input.mousePosition.y <= panBorderThickness))
				{
					newPos.z += -currDir.z * wasdSpeed;
					newPos.x += -currDir.x * wasdSpeed;
				}
				if (Input.GetKey("d") || (moveViaBorder && Input.mousePosition.x >= Screen.width - panBorderThickness))
				{
					newPos.z += -currDir.x * wasdSpeed;	//in 2D, orthogonal is x/-y or -x/y
					newPos.x += currDir.z * wasdSpeed;
				}
				if (Input.GetKey("a") || (moveViaBorder && Input.mousePosition.x <= panBorderThickness))
				{
					newPos.z += currDir.x * wasdSpeed;
					newPos.x += -currDir.z * wasdSpeed;
				}
			}

			else 
			{
				wasdSpeed = wasdSpeedDefault;
			}
		}

		if (!Input.GetMouseButton(mouseButtonDrag))
		{
			//scrolling up and down
			float scroll = Input.GetAxis("Mouse ScrollWheel");

			if ((scroll < 0 && (transform.position.y == terrainHeight + minY)) || (scroll != 0 && (transform.position.y > terrainHeight + minY)))
			{
				newPos += scroll * 100 * scrollSpeed * Time.deltaTime * transform.forward;
				offsetY += newPos.y - transform.position.y;
				offsetY = Mathf.Clamp(offsetY, minY, maxY);
				panVarSpeed = (offsetY - minY) / (maxY - minY);
			}
		}

		/* newPos.x = Mathf.Clamp(newPos.x, borderFloorX, borderCeilX);
		newPos.z = Mathf.Clamp(newPos.z, borderFloorZ, borderCeilZ); */

		newPos = boundariesScript.CalculatePos(newPos);

		terrainHeight = terrain.SampleHeight(newPos);
		newPos.y = terrainHeight + offsetY;

		//Collided with an object?
		diffPositions = newPos - transform.position;
		Ray ray2 = new Ray(transform.position, diffPositions);
		RaycastHit hit;
		if (Physics.Raycast(ray2, out hit, diffPositions.magnitude, 12))
		{
			newPos = transform.position;
			return;
		}
		
		transform.position = newPos;

		/* if(Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))			//New origin, if dragged, after y-correction
		{
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitDrag;
			if(Physics.Raycast(ray , out hitDrag, 2000, ~ignoredLayers))
			{
				dragOrigin = hitDrag.point;
			}
		} */
	}
	
	//camera translation via coords (für Minikarte später, kann außerhalb aufgerufen werden)
	public void TranslateCam(Vector3 pos)
	{
		transform.position = new Vector3(pos.x, terrain.SampleHeight(pos), pos.z);
	}

	//===============================================camera rotation===============================================
	private void RotateCam()
	{       
		Vector3 pos = transform.position;

		if(Input.GetMouseButton(mouseButtonRotate))
		{       
			float mouseAxisX = Input.GetAxis("Mouse X");		//Axis since last frame, mouse coordinates (x = left/right)
			transform.RotateAround(transform.position, Vector3.up, mouseAxisX * rotSpeed);
		}
		
		Vector3 newRot = transform.rotation.eulerAngles;
		float relY = pos.y - terrainHeight;						//relative Y-Position (in relation to bottom)


		if (relY < rotCeil && !onBottom)
		{
			onBottom = true;
		}
		
		if (relY > rotCeil)
		{
			newRot.x = startRotX;
			onBottom = false;
		}
		
		if (relY < rotCeil && onBottom)                    
		{
			float relPos = relY - minY;										//somewhere between rotCeil and minY
			float normPos = 1 - (relPos / difMinYRotCeil);					//relPos/difMinYRotCeil = value between 0 and 1, depending on the relative position
			newRot.x = (startRotX - (normPos * (startRotX - minRotX)));		//if closer to the bottom, value is closer to 1, camera's rotation gets closer to y-plane
		}
		transform.Rotate(newRot - transform.rotation.eulerAngles);
	}

	/* private Vector3 CorrectCollision(Vector3 wantedPos, Vector3 hitPoint, Ray ray, Collider collider)
	{
		Vector3 correctedPos;
		Vector3 mirroredPos;

		Ray ray1 = ray;
		ray1.direction.Set(ray.direction.x + 0.0001f, ray.direction.y, ray.direction.z + 0.0001f);
		Ray ray2 = ray;
		ray2.direction.Set(ray.direction.x - 0.0001f, ray.direction.y, ray.direction.z - 0.0001f);

		RaycastHit hit1;
		RaycastHit hit2;

		//Später mal noch Layer ändern
		if (!Physics.Raycast(ray1, out hit1, 100f))
		{
			Debug.Log("hit1 nicht getroffen");
			return transform.position;
		}
		if (!Physics.Raycast(ray2, out hit2, 100f))
		{
			Debug.Log("hit2 nicht getroffen");
			return transform.position;
		}

		Vector3 mirror = hit2.transform.position - hit1.transform.position;
		Vector3 mirroredDirection = wantedPos - hitPoint;
		
		mirroredDirection.
		ray.transform.Axis
		


		diffPositions = newPos - transform.position;
		Ray ray2 = new Ray(transform.position, diffPositions);
		RaycastHit hit;
		if (Physics.Raycast(ray2, out hit, diffPositions.magnitude))
		{

		return correctedPos;
	} */

	public void LoadPositionRotation(ExtractSaveGameValues values)
	{
		this.transform.position = values.GetCameraPosition();
		this.transform.rotation = values.GetCameraRotation();
	}
}
