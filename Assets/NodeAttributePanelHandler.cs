using UnityEngine;
using TMPro;

public class NodeAttributePanelHandler : MonoBehaviour {

    private RectTransform rectTransform;

    /// <summary>
    /// The name of the current node is written to this
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI nameText;

    /// <summary>
    /// All the connected nodes are written here
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI connectedToText;

    /// <summary>
    /// The current node for which the information is displayed
    /// </summary>
    private NodeHandler currentNode;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        
        rectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, Camera.main.pixelHeight);
    }

    /// <summary>
    /// Update all the text and stuff to reflect the new node
    /// </summary>
    public void UpdateNodeTo(NodeHandler newOne) {
        currentNode = newOne;

        UpdateNodeCurrent();
    }

    /// <summary>
    /// Update all the text and stuff to reflect the changes in the current node
    /// </summary>
    public void UpdateNodeCurrent() {
        if (currentNode == null) return;

        // Name
        nameText.text = currentNode.name;

        // Connected to
        connectedToText.text = "";
        var connectedTo = currentNode.ConnectedTo;
        for (int i = 0; i < connectedTo.Count; i++)
            connectedToText.text += connectedTo[i].name + "\n";

    }

}