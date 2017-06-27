using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(LineRenderer))]
public class GraphHandler : MonoBehaviour {

    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private Toggle addEditToggle;

    [SerializeField]
    private SurelyDeletePanelHandler areYouSurePanel;

    private LineRenderer connectStartLineRenderer;

    private EditPanelHandler editPanelHandler;
    private AddPanelHandler addPanelHandler;

    private NodeAttributePanelHandler nodeAttributePanelHandler;

    private int selectedNode = -1;
    private int SelectedNode {
        set {
            selectedNode = value;
            UpdateSelectedNode();
        }
        get {
            return selectedNode;
        }
    }

    private int connectStartNode = -1;
    private int ConnectStartNode {
        set {
            connectStartNode = value;
            UpdateSelectedConnectStartNode();
        }
        get { return connectStartNode; }
    }

    private List<NodeHandler> nodes;

    void Start () {
        editPanelHandler = FindObjectOfType<EditPanelHandler>();
        addPanelHandler = FindObjectOfType<AddPanelHandler>();
        nodeAttributePanelHandler = FindObjectOfType<NodeAttributePanelHandler>();

        connectStartLineRenderer = GetComponent<LineRenderer>();

        nodes = new List<NodeHandler>();
	}
	
	void Update () {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (ConnectStartNode != -1) {
            connectStartLineRenderer.SetPosition(1, mouseWorldPos);
        }

        #region Input
        // Not above GUI
        if (!EventSystem.current.IsPointerOverGameObject()) {
            // Left mouse button
            if (Input.GetMouseButtonDown(0)) {
                // Adding
                if (!addEditToggle.isOn) {
                    mouseWorldPos.z = 0;

                    NodeHandler added = Instantiate(nodePrefab, mouseWorldPos, Quaternion.identity, transform).GetComponent<NodeHandler>();
                    nodes.Add(added);
                } else {
                    // See whether the user clicked one
                    for (int i = 0; i < nodes.Count; i++) {
                        if (nodes[i].Contains(mouseWorldPos)) {
                            SelectedNode = i;
                            break;
                        }
                    }
                }
            }

            // Right mouse button
            if (Input.GetMouseButtonDown(1)) {
                // Adding
                if (!addEditToggle.isOn) {
                    // Remove if it is clicked
                    for (int i = nodes.Count - 1; i >= 0; i--) {
                        if (nodes[i].Contains(mouseWorldPos)) {
                            // show at the top right corner of the node
                            Vector3 showDialogAt = Camera.main.WorldToScreenPoint(nodes[i].transform.position + nodes[i].Size);
                            // But if it won't fit in screen from top -> show it at bottom
                            if (showDialogAt.y > Camera.main.pixelHeight - areYouSurePanel.Size.y) {
                                showDialogAt.y = 
                                    (Camera.main.WorldToScreenPoint(nodes[i].transform.position) - areYouSurePanel.Size).y;
                            }
                            // And if it won't fit to the right -> show it at the left
                            if (showDialogAt.x > Camera.main.pixelWidth - areYouSurePanel.Size.x) {
                                showDialogAt.x = 
                                    (Camera.main.WorldToScreenPoint(nodes[i].transform.position) - areYouSurePanel.Size).x;
                            }


                            // Ask the user whether he is sure of deleting the node
                            areYouSurePanel.ShowAtWithAction(
                                showDialogAt, 
                                (isSure) => {
                                    if (isSure) {
                                        // First update node connections that it is connected to
                                        var others = nodes[i].ConnectedTo;
                                        for (int k = 0; k < others.Count; k++)
                                            others[k].DisconnectFrom(nodes[i]);

                                        Destroy(nodes[i].gameObject);
                                        nodes.RemoveAt(i);
                                    }
                                }
                            );
                            break;
                        }
                    }
                    // Editing
                } else {
                    int selected = -1;

                    // See whether the user clicked one
                    for (int i = 0; i < nodes.Count; i++) {
                        if (nodes[i].Contains(mouseWorldPos)) {
                            selected = i;
                            break;
                        }
                    }

                    // Selected none -> deselect the start thing
                    if (selected == -1) { 
                        ConnectStartNode = -1;
                    }
                    // We have a start node selected so connect them
                    else if (ConnectStartNode != -1) {
                        nodes[ConnectStartNode].ConnectTo(nodes[selected]);
                        nodes[selected].ConnectTo(nodes[ConnectStartNode]);

                        // Reflect change son the GUI
                        nodeAttributePanelHandler.UpdateNodeCurrent();

                        ConnectStartNode = -1;
                    }
                    // We have no start selected and we really did select one -> let that be the start node
                    else {
                        ConnectStartNode = selected;
                    }
                }
            }

            // add or edit toggle
            if (Input.GetKeyDown(KeyCode.Tab)) {
                addEditToggle.isOn = !addEditToggle.isOn;

                // Adding
                if (!addEditToggle.isOn) {
                    editPanelHandler.GetHideSequence().Play().OnComplete(() => {
                        addPanelHandler.GetShowSequence().Play();
                    });

                    SelectedNode = -1;
                // Editing
                } else {
                    addPanelHandler.GetHideSequence().Play().OnComplete(() => {
                        editPanelHandler.GetShowSequence().Play();
                    });
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Update variable, colors, linerenderer etc.
    /// </summary>
    private void UpdateSelectedConnectStartNode() {
        // Deselect all
        for (int i = 0; i < nodes.Count; i++) nodes[i].SelectedStart = false;

        if (ConnectStartNode != -1) {
            // Select the current one
            nodes[ConnectStartNode].SelectedStart = true;

            // LineRenderer
            connectStartLineRenderer.positionCount = 2;

            var lineRendererStartPos = nodes[ConnectStartNode].transform.position + nodes[ConnectStartNode].Size / 2f;
            lineRendererStartPos.z = 0;
            connectStartLineRenderer.SetPosition(0, lineRendererStartPos);
        } else {
            connectStartLineRenderer.positionCount = 0;
        }
    }

    /// <summary>
    /// Update variable, colors etc.
    /// </summary>
    private void UpdateSelectedNode() {
        // Deselect all
        for (int i = 0; i < nodes.Count; i++) nodes[i].Selected = false;

        if (SelectedNode != -1) {
            // Select the current one
            nodes[SelectedNode].Selected = true;

            nodeAttributePanelHandler.UpdateNodeTo(nodes[SelectedNode]);
        }
    }
}
