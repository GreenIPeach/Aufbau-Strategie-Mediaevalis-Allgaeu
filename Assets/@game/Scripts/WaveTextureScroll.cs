using UnityEngine;

public class WaveTextureScroll : MonoBehaviour
{
	public Renderer lakeMeshRenderer;

	[Header("Wave Direction")]
	public float scrollX = 0.02f;
	public float scrollY = 0.01f;

	[Header("Normal Animation")]
	public float amplitude = 0.1f;
	public float period = 1.5f;
	public float shift = 0.3f;

	private void Start()
	{
		lakeMeshRenderer.GetComponent<Renderer>();
	}

	void Update ()
	{
		float offsetX = Time.time * scrollX;
		float offsetY = Time.time * scrollY;
		float height = amplitude * Mathf.Sin(Time.time * period) + shift;

		// "_MainTex" also offsets normal map somehow
		lakeMeshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
		lakeMeshRenderer.material.SetFloat("_BumpScale", height);
	}
}
