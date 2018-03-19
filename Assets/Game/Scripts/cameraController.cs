using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    private int gridSize;
    private int endX;
    private int startX;
    private int startY;
    private int endY;
    private Material lineMaterial;
    public Color mainColor = new Color(0f, 1f, 0f, 0.1f);
    int step = 1;
    Camera cam;
    public Shader shader;
    /* 
     * This method will create the gameBoard plane as soon as the objectPlacementController is instantiated
     */

    void OnPostRender() {
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(mainColor);
        GL.PushMatrix();
        for(int i = 0; i < gridSize; i+=step) {
            GL.Vertex3(startX, 0, startY + i + 0.5f);
            GL.Vertex3(endX, 0, startY + i + 0.5f);

            GL.Vertex3(startX + i + 0.5f, 0, startY);
            GL.Vertex3(startX + i + 0.5f, 0, endY);
        }

        GL.End();
        GL.PopMatrix();
    }
    void Start() {
        cam = GetComponent<Camera>();
        Vector3 temp = new Vector3(1, 1, cam.nearClipPlane);
        float distance = cam.ViewportToWorldPoint(temp).y;

        endX = (int)(Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance)).x * 1.2);
        startX = (int)(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x * 1.2);
        startY = (int)(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).z * 1.2);
        endY = (int)(Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance)).z * 1.2);
        gridSize = Mathf.Abs(endX) + Mathf.Abs(startX);
    }
    void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            //var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            //lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void hideGrid() {
        GL.Begin(GL.LINES);
        GL.PopMatrix();
        GL.Clear(true, true, Color.clear);
        GL.PushMatrix();
        GL.End();
    }
    void Update () {
		
	}
}
