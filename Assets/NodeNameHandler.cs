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
    
    void Update () {
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(nodeToFollow.transform.position + offsetBy);

        UpdateText();
	}

    public void SetNodeToFollow(NodeHandler node) {
        nodeToFollow = node;

        UpdateText();

        offsetBy.x = nodeToFollow.Size.x / 2f;
        offsetBy.y = -nodeToFollow.Size.y * 0.1f;

        // Set here as well so it does not simply just jump there
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(nodeToFollow.transform.position + offsetBy);
    }

    private void UpdateText() {
        Text.text = nodeToFollow.name + "\n<size=60%>" + nodeToFollow.InfectedCount + "/" + nodeToFollow.PatchedCount + "/" + nodeToFollow.HostCount;
        name = nodeToFollow.name + "Text";
    }
}
