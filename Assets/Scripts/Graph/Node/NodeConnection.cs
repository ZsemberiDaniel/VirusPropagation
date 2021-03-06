﻿using UnityEngine;

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

    private int usedCapacity = 0;
    public int UsedCapacity {
        get { return usedCapacity; }
    }

    /// <summary>
    /// Should be used for showing the user how much data is being used (usedCapacity)
    /// because this won't be reset every tick, which makes it 0 every time
    /// we want to draw it
    /// </summary>
    private int shownCapacity = 0;
    /// <summary>
    /// Should be used for showing the user how much data is being used (UsedCapacity)
    /// because this won't be reset every tick, which makes it 0 every time
    /// we want to draw it
    /// </summary>
    public int ShownCapacity {
        get { return shownCapacity; }
    }

    private ConnectionData connectionData;
    public ConnectionData ConnectionData {
        set { connectionData = value; }
        get { return connectionData; }
    }

    private LineRenderer lineRenderer;

    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        UpdateLineRenderer();

        lineRenderer.startColor = normalColor;
        lineRenderer.endColor = normalColor;
    }
    private void Update() {
        UpdateLineRenderer();
    }

    /// <summary>
    /// Can you send this packet on this connection?
    /// </summary>
    public bool CanHandlePacket(int packetSize) {
        return capacity - usedCapacity >= packetSize;
    }
    /// <summary>
    /// Only sends the packet if it is possible to be sent
    /// </summary>
    public bool SendPacket(int packetSize) {
        if (CanHandlePacket(packetSize)) { 
            usedCapacity += packetSize;
            return true;
        }

        return false;
    }
    /// <summary>
    /// Resets the bandwidth of this connection
    /// </summary>
    public void ResetBandwidth() {
        shownCapacity = usedCapacity;
        usedCapacity = 0;
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

    /// <summary>
    /// Updates linerenderer positions to the nodes' positions
    /// </summary>
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

    public bool IsRendered() {
        return nodeOne.IsRendered() || nodeTwo.IsRendered();
    }
    public bool DoesNeedConnectionDataHandler() {
        return IsRendered() && connectionData == null;
    }
    public bool DoesNoLongerNeedConnectionDataHandler() {
        return !IsRendered() && connectionData != null;
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
