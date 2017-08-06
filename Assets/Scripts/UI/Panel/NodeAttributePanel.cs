using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class NodeAttributePanel : MonoBehaviour, AttributePanel {
    
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
    /// The node delete button
    /// </summary>
    [SerializeField]
    private Button deleteButton;

    /// <summary>
    /// The current node for which the information is displayed
    /// </summary>
    private NodeHandler currentNode;
    
    private GraphHandler graph;

    private void Start() {
        graph = FindObjectOfType<GraphHandler>();

        #region Input fields
        // Name
        nameTextInput.onValueChanged.AddListener(newName => {
            if (newName.Length == 0) return;
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
        hostCountInput.onValueChanged.AddListener(newCount => {
            if (newCount.Length == 0) return;
            if (currentNode != null) {
                try {
                    int hostCount = int.Parse(newCount);
                    if (hostCount < currentNode.InfectedCount) {
                        // TODO wrong number
                        return;
                    }

                    currentNode.HostCount = hostCount;
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
        infectedCountInput.onValueChanged.AddListener(newCount => {
            if (newCount.Length == 0) return;
            if (currentNode != null) {
                try {
                    int infectedCount = int.Parse(newCount);
                    if (currentNode.HostCount < infectedCount) {
                        // TODO wrong number
                        return;
                    }

                    currentNode.InfectedCount = infectedCount;
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

        deleteButton.onClick.AddListener(() => {
            if (currentNode != null) {
                graph.RemoveNodeWithSurePanel(currentNode);
            }
        });
        #endregion
    }

    public void UpdateToNoneSelected() {
        currentNode = null;

        nameTextInput.text = "";
        connectedToText.text = "";
        hostCountInput.text = "";
        infectedCountInput.text = "";

        nameTextInput.interactable = false;
        hostCountInput.interactable = false;
        infectedCountInput.interactable = false;

        deleteButton.interactable = false;
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

        deleteButton.interactable = true;

        // Name
        nameTextInput.text = currentNode.name;
        nameTextInput.interactable = true;

        // Host count
        hostCountInput.text = currentNode.HostCount.ToString();
        hostCountInput.interactable = true;

        // Infected count
        infectedCountInput.text = currentNode.InfectedCount.ToString();
        infectedCountInput.interactable = true;

        // Connected to
        connectedToText.text = "";
        var connectedTo = currentNode.GetConnectedToNodes();
        for (int i = 0; i < connectedTo.Count; i++)
            connectedToText.text += connectedTo[i].name + "\n";

        FocusFirstUIElement();
    }

    public void FocusFirstUIElement() {
        EventSystem.current.SetSelectedGameObject(nameTextInput.gameObject);
        nameTextInput.ActivateInputField();
    }
}