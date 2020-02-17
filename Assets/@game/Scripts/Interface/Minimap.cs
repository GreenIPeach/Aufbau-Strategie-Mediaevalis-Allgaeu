using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public Camera MainCamera;
    public Vector3 Baseposition = new Vector3(250.0f, 100.0f, 250.0f);

    // With MinimapCamSize 125.0
    private float minZ = 210.0f; // Cam 50
    private float maxZ = 290.0f; // Cam 450
    private float minX = 210.0f; // Cam 50
    private float maxX = 290.0f; // Cam 450

    void LateUpdate()
    {
        Vector3 newPosition = MainCamera.transform.position;
        newPosition.y = transform.position.y;
        newPosition.x = Baseposition.x + ((newPosition.x - Baseposition.x) * 0.5f);
        newPosition.z = Baseposition.z + ((newPosition.z - Baseposition.z) * 0.5f);

        if (newPosition.z < minZ)
        {
            newPosition.z = minZ;
        }
        else if (newPosition.z > maxZ)
        {
            newPosition.z = maxZ;
        }

        if (newPosition.x < minX)
        {
            newPosition.x = minX;
        }
        else if (newPosition.x > maxX)
        {
            newPosition.x = maxX;
        }

        transform.position = newPosition;
    }


    // Minimap Trapez
    public Camera minimapCamera;
    private Vector3 topLeftPosition, topRightPosition, bottomLeftPosition, bottomRightPosition;

    public void Start()
    {
        if (MainCamera == null)
        {
            Debug.LogError("Unable to determine where the Player Camera component is at.");
        }

        if (minimapCamera == null)
        {
            minimapCamera = this.GetComponent<Camera>();

            if (minimapCamera == null)
            {
                Debug.LogError("Unable to determine where the Minimap Camera component is at.");
            }
        }
    }
    /*
    public void Update()
    {
        Ray topLeftCorner = MainCamera.ScreenPointToRay(new Vector3(0f, 0f));
        Ray topRightCorner = MainCamera.ScreenPointToRay(new Vector3(Screen.width, 0f));
        Ray bottomLeftCorner = MainCamera.ScreenPointToRay(new Vector3(0, Screen.height));
        Ray bottomRightCorner = MainCamera.ScreenPointToRay(new Vector3(Screen.width, Screen.height));

        RaycastHit[] hits = new RaycastHit[4];
        if (Physics.Raycast(topLeftCorner, out hits[0], 600.0f))
        {
            topLeftPosition = hits[0].point;
        }

        if (Physics.Raycast(topRightCorner, out hits[1], 600.0f))
        {
            topRightPosition = hits[1].point;
        }

        if (Physics.Raycast(bottomLeftCorner, out hits[2], 600.0f))
        {
            bottomLeftPosition = hits[2].point;
        }

        if (Physics.Raycast(bottomRightCorner, out hits[3], 600.0f))
        {
            bottomRightPosition = hits[3].point;
        }

        topLeftPosition = minimapCamera.WorldToViewportPoint(topLeftPosition);
        topRightPosition = minimapCamera.WorldToViewportPoint(topRightPosition);
        bottomLeftPosition = minimapCamera.WorldToViewportPoint(bottomLeftPosition);
        bottomRightPosition = minimapCamera.WorldToViewportPoint(bottomRightPosition);

        topLeftPosition.z = -1f;
        topRightPosition.z = -1f;
        bottomLeftPosition.z = -1f;
        bottomRightPosition.z = -1f;
    }

    public void OnPostRender()
    {
        GL.PushMatrix();
        {
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            {
                GL.Vertex(topLeftPosition);
                GL.Vertex(topRightPosition);
                GL.Vertex(topRightPosition);
                GL.Vertex(bottomRightPosition);
                GL.Vertex(bottomRightPosition);
                GL.Vertex(bottomLeftPosition);
                GL.Vertex(bottomLeftPosition);
                GL.Vertex(topLeftPosition);
            }
            GL.End();
        }
        GL.PopMatrix();
    }*/
}
