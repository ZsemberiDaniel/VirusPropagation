using UnityEngine;
using TMPro;

public class NodeAttributePanelHandler : MonoBehaviour {

    private RectTransform rectTransform;

    /// <summary>
    /// The name of the current node is written to this
    /// </summary>
    [SerializeField]
    private TMP_InputField nameTextInput;

    /// <summary>
    /// All the connected nodes are written here
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI connectedToText;

    /// <summary>
    /// How many hosts there are in the current network
    /// </summary>
    [SerializeField]
    private TMP_InputField hostCountInput;

    /// <summary>
    /// How many infected hosts there are in the current network
    /// </summary>
    [SerializeField]
    private TMP_InputField infectedCountInput;

    /// <summary>
    /// The current node for which the information is displayed
    /// </summary>
    private NodeHandler currentNode;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        
        rectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, Camera.main.pixelHeight);

        #region Input fields
        // Name
        nameTextInput.onSubmit.AddListener(newName => {
            if (currentNode != null) {
                currentNode.name = newName;
            }
        });
        // Set it to the name so if the wanted name had been taken the name stays
        nameTextInput.onEndEdit.AddListener(endText => {
            if (currentNode != null) {
                nameTextInput.text = currentNode.name;
            }
        });


        // Host count
        hostCountInput.onSubmit.AddListener(newCount => {
            if (currentNode != null) {
                try {
                    currentNode.HostCount = int.Parse(newCount);
                } catch (System.ArgumentException e) {
                    // TODO wrong number
                }
            }
        });
        // Set it to the before value so if it ha been changed invalidly it doesnt freak out
        hostCountInput.onEndEdit.AddListener(newCount => {
            if (currentNode != null) {
                hostCountInput.text = currentNode.HostCount.ToString();
            }
        });


        // Infected count
        infectedCountInput.onSubmit.AddListener(newCount => {
            if (currentNode != null) {
                try {
                    currentNode.InfectedCount = int.Parse(newCount);
                } catch (System.ArgumentException e) {
                    // TODO wrong number
                }
            }
        });
        // Set it to the before value so if it ha been changed invalidly it doesnt freak out
        infectedCountInput.onEndEdit.AddListener(newCount => {
            if (currentNode != null) {
                infectedCountInput.text = currentNode.InfectedCount.ToString();
            }
        });
        #endregion
    }

    public void UpdateToNoneSelected() {
        currentNode = null;

        nameTextInput.text = "None";
        connectedToText.text = "";
        hostCountInput.text = "0";
        infectedCountInput.text = "0";

        nameTextInput.readOnly = true;
        hostCountInput.readOnly = true;
        infectedCountInput.readOnly = true;
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
        nameTextInput.text = currentNode.name;
        nameTextInput.readOnly = false;

        // Host count
        hostCountInput.text = currentNode.HostCount.ToString();
        hostCountInput.readOnly = false;

        // Infected count
        infectedCountInput.text = currentNode.InfectedCount.ToString();
        infectedCountInput.readOnly = false;

        // Connected to
        connectedToText.text = "";
        var connectedTo = currentNode.ConnectedToNodes;
        for (int i = 0; i < connectedTo.Count; i++)
            connectedToText.text += connectedTo[i].name + "\n";
    }

}