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
    private List<NodeConnection> nodeConnections;

    void Start () {
        editPanelHandler = FindObjectOfType<EditPanelHandler>();
        addPanelHandler = FindObjectOfType<AddPanelHandler>();
        nodeAttributePanelHandler = FindObjectOfType<NodeAttributePanelHandler>();
        nodeNameParentHandler = FindObjectOfType<NodeNameParentHandler>();

        connectStartLineRenderer = GetComponent<LineRenderer>();

        nodes = new List<NodeHandler>();
        nodeConnections = new List<NodeConnection>();
	}

    /// <summary>
    /// Used when a node is being moved. If it's not -1 then one is being moved.
    /// </summary>
    private int whichNodeIsBeingMoved = -1;

    // These are used so when we click on a node it does not immediatly start moving
    private Vector3 mousePanStartedAt = new Vector3();
    private float minMouseDistanceForNodeMoving = 0.1f;

	void Update() {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (ConnectStartNode != -1) {
            connectStartLineRenderer.SetPosition(1, mouseWorldPos);
        }

        // Node names 
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
            int aboveNodeIndex = -1;

            // See whether the user clicked one
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                for (int i = 0; i < nodes.Count; i++) {
                    if (nodes[i].Contains(mouseWorldPos)) {
                        aboveNodeIndex = i;
                        break;
                    }
                }
            }

            // Left mouse button down
            if (Input.GetMouseButtonDown(0)) {
                // Adding
                if (!addEditToggle.isOn) {
                    if (aboveNodeIndex == -1) {
                        AddNodeAtPos(mouseWorldPos);
                    }
                // Select
                } else {
                    mousePanStartedAt = mouseWorldPos;
                    SelectedNode = aboveNodeIndex;
                }
            }

            // Left mouse button held
            if (Input.GetMouseButton(0)) {
                // Editing
                // The second part: we started the holding down on a node or we have not released the button yet
                if (addEditToggle.isOn) {
                    // we are above a node when moving the mouse around so let that be the moving node
                    if (aboveNodeIndex != -1 && whichNodeIsBeingMoved == -1) { 
                        whichNodeIsBeingMoved = aboveNodeIndex;
                        SelectedNode = whichNodeIsBeingMoved;
                    }

                    // We are moving a node
                    if (whichNodeIsBeingMoved != -1 && (mouseWorldPos - mousePanStartedAt).magnitude >= minMouseDistanceForNodeMoving) { 
                        var nodeNewPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        nodeNewPos.z = 0;

                        nodes[whichNodeIsBeingMoved].Position = nodeNewPos - nodes[whichNodeIsBeingMoved].Size / 2f;
                    }
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
                    if (aboveNodeIndex != -1) {
                        RemoveNodeWithIndexWithSurePanel(aboveNodeIndex);
                    }
                // Editing
                } else {
                    // Selected none -> deselect the start thing
                    if (aboveNodeIndex == -1) { 
                        ConnectStartNode = -1;
                    }
                    // We have a start node selected so connect them
                    else if (ConnectStartNode != -1) {
                        ConnectTwoNodesWithIndices(ConnectStartNode, aboveNodeIndex);

                        ConnectStartNode = -1;
                    }
                    // We have no start selected and we really did select one -> let that be the start node
                    else {
                        SelectedNode = aboveNodeIndex;
                        ConnectStartNode = aboveNodeIndex;
                    }
                }
            }

            // Middle mouse button
            if (Input.GetMouseButtonDown(2))
                areYouSurePanel.HidePanel();

            // add or edit toggle
            if (Input.GetKeyDown(KeyCode.Tab)) {
                ToggleAddEditMode();
            }
        }
        #endregion
    }

    /// <summary>
    /// Whether the given name is taken by a node or not.
    /// </summary>
    public bool IsNameTaken(string name) {
        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i].name == name)
                return true;

        return false;
    }

    /// <summary>
    /// Toggle the add/edit mode
    /// </summary>
    private void ToggleAddEditMode() {
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

    /// <summary>
    /// Connect these node mutually in the nodes list.
    /// </summary>
    private void ConnectTwoNodesWithIndices(int index1, int index2) {
        var connection = new NodeConnection(nodes[index1], nodes[index2]);

        nodes[index1].ConnectTo(connection);
        nodes[index2].ConnectTo(connection);

        nodeConnections.Add(connection);

        // Reflect change son the GUI
        nodeAttributePanelHandler.UpdateNodeCurrent();
    }

    /// <summary>
    /// Removed the node in the indexth place of the nodes array WITH the are you sure panel.
    /// </summary>
    private void RemoveNodeWithIndexWithSurePanel(int index) {
        // Hide immediatly so if it is shown at another node it can show the panel again
        areYouSurePanel.HideImmediatly();

        // show at the top right corner of the node
        Vector3 showDialogAt = Camera.main.WorldToScreenPoint(nodes[index].transform.position + nodes[index].Size);
        areYouSurePanel.RectTransf.pivot = new Vector2();
        // But if it won't fit in screen from top -> show it at bottom
        if (showDialogAt.y > Camera.main.pixelHeight - areYouSurePanel.Size.y) {
            areYouSurePanel.RectTransf.pivot = new Vector2(areYouSurePanel.RectTransf.pivot.x, 1);
            showDialogAt.y =
                (Camera.main.WorldToScreenPoint(nodes[index].transform.position)).y;
        }
        // And if it won't fit to the right -> show it at the left
        if (showDialogAt.x > Camera.main.pixelWidth - areYouSurePanel.Size.x) {
            areYouSurePanel.RectTransf.pivot = new Vector2(1, areYouSurePanel.RectTransf.pivot.y);
            showDialogAt.x =
                (Camera.main.WorldToScreenPoint(nodes[index].transform.position)).x;
        }


        // Ask the user whether he is sure of deleting the node
        areYouSurePanel.ShowAtWithAction(
            showDialogAt,
            (isSure) => {
                if (isSure) {
                    RemoveNodeWithIndex(index);
                }
            }
        );
    }

    /// <summary>
    /// Removed the node in the indexth place of the nodes array without the are you sure panel.
    /// </summary>
    private void RemoveNodeWithIndex(int index) {
        // First update node connections that it is connected to
        var others = nodes[index].ConnectedToNodes;
        for (int k = 0; k < others.Count; k++)
            others[k].DisconnectFromNode(nodes[index]);

        // Delete the connections here, in the graph
        for (int i = nodeConnections.Count - 1; i >= 0; i--)
            if (nodeConnections[i].Contains(nodes[i]))
                nodeConnections.RemoveAt(i);

        nodeNameParentHandler.QueueText(nodes[index].NodeNameHandler);

        Destroy(nodes[index].gameObject);
        nodes.RemoveAt(index);
    }

    /// <summary>
    /// Adds a node at the given position
    /// </summary>
    private void AddNodeAtPos(Vector3 position) {
        NodeHandler added = Instantiate(nodePrefab, position, Quaternion.identity, transform).GetComponent<NodeHandler>();
        nodes.Add(added);
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

            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            connectStartLineRenderer.SetPosition(1, mouseWorldPos);
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
        } else {
            nodeAttributePanelHandler.UpdateToNoneSelected();
        }
    }
}
