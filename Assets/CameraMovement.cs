using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour {

    [SerializeField]
    [Range(0.5f, 1.8f)]
    private float mouseSensitivity = 2f;

    [SerializeField]
    [Range(0.8f, 5f)]
    private float zoomSensitivity = 2f;

    private float minOrthoSize = 2f;
    private float maxOrthoSize = 10f;

    private Camera cam;
    /// <summary>
    /// In viewport pos
    /// </summary>
    private Vector3 lastMousePos;

	void Start () {
        cam = GetComponent<Camera>();
	}

	void Update () {
        // so the last pos is not pointing to zero zero
        if (Input.GetMouseButtonDown(2)) lastMousePos = cam.ScreenToViewportPoint(Input.mousePosition); 

        // Middle mouse button
		if (Input.GetMouseButton(2)) {
            var currMousePos = cam.ScreenToViewportPoint(Input.mousePosition);
            var delta = currMousePos - lastMousePos;
            delta.z = 0;
            
            // Translate by the amount we have moved the mouse since the last frame
            transform.Translate(-delta * (cam.orthographicSize * 2) * mouseSensitivity);

            lastMousePos = currMousePos;
        }
        
        // Zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            float targetOrtho = Mathf.Clamp(cam.orthographicSize - scroll * zoomSensitivity, minOrthoSize, maxOrthoSize);
            cam.orthographicSize = targetOrtho;
        }
	}
}
