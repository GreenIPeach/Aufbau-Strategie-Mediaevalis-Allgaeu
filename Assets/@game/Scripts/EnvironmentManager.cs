using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
	[Header("Skybox")]
	[SerializeField] private float rotationSpeed = 0.3f;
	private float currentRotation;
	
	// Update is called once per frame
	void Update ()
	{
		RotateSkyMaterial();
	}

	void RotateSkyMaterial()
	{
		currentRotation += rotationSpeed * Time.deltaTime;
		currentRotation %= 360;
		RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
	}
}
