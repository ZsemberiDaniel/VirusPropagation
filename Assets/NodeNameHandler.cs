using UnityEngine;
using TMPro;

public class NodeNameHandler : MonoBehaviour {

    private NodeHandler nodeToFollow;

    private TextMeshProUGUI text;
    private TextMeshProUGUI Text {
        get {
            if (text == null) text = GetComponent<TextMeshProUGUI>();

            return text;
        }
    }

    private RectTransform rectTransform;
    private RectTransform RectTransf {
        get {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

            return rectTransform;
        }
    }

    /// <summary>
    /// Set every frame
    /// </summary>
    private Vector3 offsetBy = new Vector3();

    float worldToScreen = 1 / (Camera.main.orthographicSize * 2) * Camera.main.pixelWidth;
    void Update () {
        offsetBy.x = (nodeToFollow.Size.x * worldToScreen - Text.bounds.size.x) / 2f;
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(nodeToFollow.transform.position + offsetBy);
	}

    public void SetNodeToFollow(NodeHandler node) {
        nodeToFollow = node;

        Text.text = node.name;
        name = node.name + "Text";

        // Set here as well so it does not simply just jump there
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(nodeToFollow.transform.position + offsetBy);
    }
}
