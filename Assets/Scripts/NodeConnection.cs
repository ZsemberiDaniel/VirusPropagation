using UnityEngine;

public class NodeConnection : MonoBehaviour {

    private bool selected = false;
    public bool Selected { get { return selected; } }

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color selectedColor;

    private NodeHandler nodeOne;
    public NodeHandler NodeOne { get { return nodeOne; } }
    private NodeHandler nodeTwo;
    public NodeHandler NodeTwo { get { return nodeTwo; } }

    public Vector3 MiddlePos { get { return (nodeOne.transform.position + nodeTwo.transform.position) / 2f; } }

    private int capacity;
    public int Capacity {
        get { return capacity; }
        set { capacity = value; }
    }

    private LineRenderer lineRenderer;

    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        UpdateLineRenderer();
    }
    private void Update() {
        UpdateLineRenderer();
    }

    public void SetNodes(NodeHandler one, NodeHandler two) {
        this.nodeOne = one;
        this.nodeTwo = two;

        name = one.name + " -> " + two.name;
    }

    public NodeHandler OtherNode(NodeHandler currNode) {
        if (currNode.Equals(nodeOne)) return nodeTwo;
        else if (currNode.Equals(nodeTwo)) return nodeOne;
        return null;
    }

    private void UpdateLineRenderer() {
        lineRenderer.SetPosition(0, nodeOne.PositionMiddle);
        lineRenderer.SetPosition(1, nodeTwo.PositionMiddle);
    }

    public void Select() {
        if (selected) return;
        selected = true;

        lineRenderer.startColor = selectedColor;
        lineRenderer.endColor = selectedColor;
    }
    public void Deselect() {
        if (!selected) return;
        selected = false;

        lineRenderer.startColor = normalColor;
        lineRenderer.endColor = normalColor;
    }

    /// <summary>
    /// Disconnects both nodes from each other
    /// </summary>
    public void DisconnectBoth() {
        nodeOne.Disconnect(nodeTwo);
        nodeTwo.Disconnect(nodeOne);
    }

    public bool Contains(NodeHandler node) {
        return nodeOne.Equals(node) || nodeTwo.Equals(node);
    }
    public bool Contains(Vector3 mouseWorldPos, float maxDistance) {
        Vector3 pos1 = nodeOne.PositionMiddle;
        Vector3 pos2 = nodeTwo.PositionMiddle;

        float pointLineDist = Mathf.Abs((pos2.y - pos1.y) * (pos1.x - mouseWorldPos.x) - (pos2.x - pos1.x) * (pos1.y - mouseWorldPos.y))
            / Mathf.Sqrt((pos2.y - pos1.y) * (pos2.y - pos1.y) + (pos2.x - pos1.x) * (pos2.x - pos1.x));
        
        if (pointLineDist <= maxDistance) {
            Vector3 connectionVector = (pos2 - pos1).normalized;
            Vector3 normalConnectionVector = new Vector3(-connectionVector.y, connectionVector.x, 0);

            Vector3 pointOnLine1 = mouseWorldPos + normalConnectionVector * pointLineDist;
            Vector3 pointOnLine2 = mouseWorldPos - normalConnectionVector * pointLineDist;
            float connectionDist = Vector3.Distance(pos1, pos2);

            return Mathf.Approximately(
                    connectionDist, 
                    Vector3.Distance(pos1, pointOnLine1) + Vector3.Distance(pos2, pointOnLine1)
                ) || Mathf.Approximately(
                    connectionDist,
                    Vector3.Distance(pos1, pointOnLine2) + Vector3.Distance(pos2, pointOnLine2)
                );
        } else {
            return false;
        }
    } 
    public bool Equals(NodeHandler one, NodeHandler two) {
        return Contains(one) && Contains(two);
    }
    public override string ToString() { return nodeOne.name + " - " + nodeTwo.name; }
	
}
