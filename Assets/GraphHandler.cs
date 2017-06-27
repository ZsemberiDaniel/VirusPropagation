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
    private NodeNameParentHandler nodeNameParentHandler;

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
        nodeNameParentHandler = FindObjectOfType<NodeNameParentHandler>();

        connectStartLineRenderer = GetComponent<LineRenderer>();

        nodes = new List<NodeHandler>();
	}

    /// <summary>
    /// Used when a node is being moved. If it's not -1 then one is being moved.
    /// </summary>
    private int whichNodeIsBeingMoved = -1;
	void Update () {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (ConnectStartNode != -1) {
            connectStartLineRenderer.SetPosition(1, mouseWorldPos);
        }

        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].DoesNeedNodeNameHandler()) {
                nodes[i].NodeNameHandler = nodeNameParentHandler.GetNewText(nodes[i]);
            } else if (nodes[i].DoesNoLongerNeedNodeNameHandler()) {
                nodeNameParentHandler.QueueText(nodes[i].NodeNameHandler);
                nodes[i].NodeNameHandler = null;
            }
        }

        #region Input
        // Not above GUI
        if (!EventSystem.current.IsPointerOverGameObject()) {
            // which node the mouse is over
            // only avliable when either mouse 0 or 1 is held down
            int aboveIndex = -1;

            // See whether the user clicked one
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                for (int i = 0; i < nodes.Count; i++) {
                    if (nodes[i].Contains(mouseWorldPos)) {
                        aboveIndex = i;
                        break;
                    }
                }
            }

            // Left mouse button down
            if (Input.GetMouseButtonDown(0)) {
                // Adding
                if (!addEditToggle.isOn) {
                    if (aboveIndex == -1) { 
                        mouseWorldPos.z = 0;

                        NodeHandler added = Instantiate(nodePrefab, mouseWorldPos, Quaternion.identity, transform).GetComponent<NodeHandler>();
                        nodes.Add(added);
                    }
                // Select
                } else {
                    SelectedNode = aboveIndex;
                }
            }

            // Left mouse button held
            if (Input.GetMouseButton(0)) {
                // Editing
                // The second part: we started the holding down on a node or we have not released the button yet
                if (addEditToggle.isOn && (aboveIndex != -1 || whichNodeIsBeingMoved != -1)) {
                    if (aboveIndex != -1) whichNodeIsBeingMoved = aboveIndex;

                    var nodeNewPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    nodeNewPos.z = 0;

                    nodes[whichNodeIsBeingMoved].Position = nodeNewPos - nodes[whichNodeIsBeingMoved].Size / 2f;
                }
            }

            // Left mouse button up
            if (Input.GetMouseButtonUp(0)) {
                // Editing
                if (addEditToggle.isOn) {
                    whichNodeIsBeingMoved = -1;
                }
            }

            // Right mouse button
            if (Input.GetMouseButtonDown(1)) {
                // Adding
                if (!addEditToggle.isOn) {
                    // Remove if one is clicked
                    if (aboveIndex != -1) {
                        // Hide immediatly so if it is shown at another node it can show the panel again
                        areYouSurePanel.HideImmediatly();

                        // show at the top right corner of the node
                        Vector3 showDialogAt = Camera.main.WorldToScreenPoint(nodes[aboveIndex].transform.position + nodes[aboveIndex].Size);
                        areYouSurePanel.RectTransf.pivot = new Vector2();
                        // But if it won't fit in screen from top -> show it at bottom
                        if (showDialogAt.y > Camera.main.pixelHeight - areYouSurePanel.Size.y) {
                            areYouSurePanel.RectTransf.pivot = new Vector2(areYouSurePanel.RectTransf.pivot.x, 1);
                            showDialogAt.y =
                                (Camera.main.WorldToScreenPoint(nodes[aboveIndex].transform.position)).y;
                        }
                        // And if it won't fit to the right -> show it at the left
                        if (showDialogAt.x > Camera.main.pixelWidth - areYouSurePanel.Size.x) {
                            areYouSurePanel.RectTransf.pivot = new Vector2(1, areYouSurePanel.RectTransf.pivot.y);
                            showDialogAt.x =
                                (Camera.main.WorldToScreenPoint(nodes[aboveIndex].transform.position)).x;
                        }


                        // Ask the user whether he is sure of deleting the node
                        areYouSurePanel.ShowAtWithAction(
                            showDialogAt,
                            (isSure) => {
                                if (isSure) {
                                    // First update node connections that it is connected to
                                    var others = nodes[aboveIndex].ConnectedTo;
                                    for (int k = 0; k < others.Count; k++)
                                        others[k].DisconnectFrom(nodes[aboveIndex]);

                                    Destroy(nodes[aboveIndex].gameObject);
                                    nodes.RemoveAt(aboveIndex);
                                }
                            }
                        );
                    }
                // Editing
                } else {
                    // Selected none -> deselect the start thing
                    if (aboveIndex == -1) { 
                        ConnectStartNode = -1;
                    }
                    // We have a start node selected so connect them
                    else if (ConnectStartNode != -1) {
                        nodes[ConnectStartNode].ConnectTo(nodes[aboveIndex]);
                        nodes[aboveIndex].ConnectTo(nodes[ConnectStartNode]);

                        // Reflect change son the GUI
                        nodeAttributePanelHandler.UpdateNodeCurrent();

                        ConnectStartNode = -1;
                    }
                    // We have no start selected and we really did select one -> let that be the start node
                    else {
                        ConnectStartNode = aboveIndex;
                    }
                }
            }

            // Middle mouse button
            if (Input.GetMouseButtonDown(2))
                areYouSurePanel.HidePanel();

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
                    areYouSurePanel.HidePanel();
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
