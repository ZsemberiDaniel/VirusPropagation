using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour {

    [SerializeField]
    [Range(0.5f, 5f)]
    private float mouseSensitivity = 2f;

    [SerializeField]
    [Range(80f, 200f)]
    private float zoomSensitivity = 20f;

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
        if (Input.GetMouseButtonDown(2)) lastMousePos = cam.WorldToViewportPoint(Input.mousePosition); 

        // Middle mouse button
		if (Input.GetMouseButton(2)) {
            var delta = cam.WorldToViewportPoint(Input.mousePosition) - lastMousePos;
            delta.z = 0;

            // Translate by the amount we have moved the mouse since the last frame
            transform.Translate(-delta * cam.orthographicSize * 2 * Time.deltaTime * mouseSensitivity);

            lastMousePos = cam.WorldToViewportPoint(Input.mousePosition);
        }
        
        // Zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            float targetOrtho = Mathf.Clamp(cam.orthographicSize - scroll * Time.deltaTime * zoomSensitivity, minOrthoSize, maxOrthoSize);
            cam.orthographicSize = targetOrtho;
        }
	}
}
