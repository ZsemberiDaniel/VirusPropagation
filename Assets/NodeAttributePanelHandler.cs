using UnityEngine;
using TMPro;

public class NodeAttributePanelHandler : MonoBehaviour {

    private RectTransform rectTransform;

    [SerializeField]
    private RectTransform canvasRect;

    [SerializeField]
    private TextMeshProUGUI nameText;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        
        rectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, Camera.main.pixelHeight);
    }

    public void UpdateCurrent(NodeHandler newOne) {
        nameText.text = newOne.name;
    }

}