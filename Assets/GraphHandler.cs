using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(LineRenderer))]
public class GraphHandler : MonoBehaviour {

    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private Toggle addEditToggle;

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
                        Destroy(nodes[i].gameObject);
                        nodes.RemoveAt(i);
                        break;
                    }
                }
            // Editing
            } else {
                // See whether the user clicked one
                for (int i = 0; i < nodes.Count; i++) {
                    if (nodes[i].Contains(mouseWorldPos)) {
                        ConnectStartNode = i;
                        break;
                    }
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
        #endregion
    }

    private void UpdateSelectedConnectStartNode() {
        if (ConnectStartNode != -1) {
            // Deselect all
            for (int i = 0; i < nodes.Count; i++) nodes[i].SelectedStart = false;
            // Select the current one
            nodes[ConnectStartNode].SelectedStart = true;

            // LineRenderer
            connectStartLineRenderer.positionCount = 2;

            var lineRendererStartPos = nodes[ConnectStartNode].transform.position;
            lineRendererStartPos.z = 0;
            connectStartLineRenderer.SetPosition(0, lineRendererStartPos);
        } else {
            connectStartLineRenderer.positionCount = 0;
        }
    }

    private void UpdateSelectedNode() {
        if (SelectedNode != -1) {
            // Deselect all
            for (int i = 0; i < nodes.Count; i++) nodes[i].Selected = false;
            // Select the current one
            nodes[SelectedNode].Selected = true;

            nodeAttributePanelHandler.UpdateCurrent(nodes[SelectedNode]);
        }
    }
}
