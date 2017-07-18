using UnityEngine;
using TMPro;

public class ConnectionData : MonoBehaviour {

    private NodeConnection connectionToFollow;

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
    public Vector3 Size {
        get { return rectTransform.sizeDelta; }
        set {
            rectTransform.sizeDelta = value;
            Text.RecalculateClipping();
        }
    }

    void Update() {
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(connectionToFollow.MiddlePos);

        UpdateText();
    }

    public void SetConnectionToFollow(NodeConnection connection) {
        connectionToFollow = connection;

        UpdateText();

        // Set here as well so it does not simply just jump there
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(connectionToFollow.MiddlePos);
    }

    private void UpdateText() {
        Text.text = connectionToFollow.UsedCapacity + "/" + connectionToFollow.Capacity;
        name = connectionToFollow.name + "Text";
    }
}
