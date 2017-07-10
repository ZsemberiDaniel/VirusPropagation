using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class NodeHandler : MonoBehaviour {
    
    private int hostCount = 1000;
    /// <summary>
    /// How many hosts there are in this network
    /// </summary>
    public int HostCount {
        get { return hostCount; }
        set {
            if (value < 0) throw new System.ArgumentException("Host count cannot be lower than 0!");
            else hostCount = value;
        }
    }

    private int infectedCount = 0;
    /// <summary>
    /// How many of the hosts are infected
    /// </summary>
    public int InfectedCount {
        get { return infectedCount; }
        set {
            if (value < 0) throw new System.ArgumentException("Infected count cannot be lower than 0!");
            else if (value > HostCount - PatchedCount) throw new System.ArgumentException("Infected count cannot be higher than host count minus patched!");
            else infectedCount = value;
        }
    }

    private int patchedCount = 0;
    /// <summary>
    /// How many hosts are patched
    /// </summary>
    public int PatchedCount {
        get { return patchedCount; }
        set {
            if (value < 0) throw new System.ArgumentException("Patched count cannot be lower than 0!");
            else if (value > HostCount - InfectedCount) throw new System.ArgumentException("Patched count cannot be higher than host count minus infected!");
            else patchedCount = value;
        }
    }

    private Color normalColor = new Color(1, 1, 1, 0);
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color connectStartColor;

    [SerializeField]
    private Color normalHostColor;
    [SerializeField]
    private Color infectedColor;
    [SerializeField]
    private Color patchedColor;

    private bool selected = false;
    public bool Selected {
        set { selected = value; }
        get { return selected; }
    }

    private bool selectedStart = false;
    public bool SelectedStart {
        set { selectedStart = value; }
        get { return selectedStart; }
    }

    /// <summary>
    /// The graph this node is in
    /// </summary>
    private GraphHandler graph;

    /// <summary>
    /// Size of the sprite
    /// </summary>
    public Vector3 Size {
        get {
            var size = fadeSpriteRenderer.bounds.size;
            size.z = 0;

            return size;
        }
    }

    private NodeNameHandler nodeNameHandler;
    public NodeNameHandler NodeNameHandler {
        get { return nodeNameHandler; }
        set { nodeNameHandler = value; }
    }
    public bool HasNodeNameHandler() { return nodeNameHandler != null; }

    private string nodeName = "";
    public new string name {
        get { return nodeName; }
        set {
            if (value == "") return;
            else if (graph.IsNameTaken(value)) return;

            nodeName = value;
            base.name = nodeName;
        }
    }

    public bool IsRendered() { return fadeSpriteRenderer.isVisible; }
    public bool DoesNeedNodeNameHandler() { return !HasNodeNameHandler() && IsRendered(); }
    public bool DoesNoLongerNeedNodeNameHandler() { return HasNodeNameHandler() && !IsRendered(); }

    /// <summary>
    /// The position of the node in the world. Please use this instead of transform.position
    /// </summary>
    public Vector3 Position {
        get { return transform.position; }
        set {
            transform.position = value;
            // Update linerenderer
            LineRendererUpdateToTransform();

            // Update the other nodes' lines to this node
            for (int i = 0; i < connectedTo.Count; i++)
                connectedTo[i].OtherNode(this).LineRendererUpdatePosition(this);
        }
    }

    /// <summary>
    /// The middle position of this node
    /// </summary>
    public Vector3 PositionMiddle {
        get { return Position + Size / 2f; }
    }

    private List<NodeConnection> connectedTo;
    public ReadOnlyCollection<NodeHandler> ConnectedToNodes {
        get {
            return connectedTo.Select(connection => connection.OtherNode(this)).ToList().AsReadOnly();
        }
    }

    private SpriteRenderer fadeSpriteRenderer;
    private SpriteRenderer normalSpriteRenderer;
    private SpriteRenderer infectedSpriteRenderer;
    private SpriteRenderer patchedSpriteRenderer;
    /// <summary>
    /// Every even position is the node's position. Every odd position is the node it is connected to in the order of the list.
    /// </summary>
    private LineRenderer lineRenderer;
    
	void Start() {
        connectedTo = new List<NodeConnection>();

        fadeSpriteRenderer = GetComponent<SpriteRenderer>();
        // Use numbers so the patched is always below the infected and the normal is the lowest
        normalSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        patchedSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        infectedSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        graph = FindObjectOfType<GraphHandler>();

        // Set the proportion spriterenderers' colors
        normalSpriteRenderer.color = normalHostColor;
        infectedSpriteRenderer.color = infectedColor;
        patchedSpriteRenderer.color = patchedColor;

        // Set tha fade colors to the correct alpha
        selectedColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0.3f);
        connectStartColor = new Color(connectStartColor.r, connectStartColor.g, connectStartColor.b, 0.3f);

        name = RandomWords.getRandomWords().Substring(0, 3) + RandomWords.getRandomWords().Substring(0, 3);
    }
	void Update() {
        // Update color based on the selection
        if (SelectedStart) fadeSpriteRenderer.color = connectStartColor;
        else if (Selected) fadeSpriteRenderer.color = selectedColor;
        else fadeSpriteRenderer.color = normalColor;

        UpdateSpriteRenderersToSIRProportion();
	}

    /// <summary>
    /// Updates all the spriterenderes so they mirorr the proportions of the three variables:
    /// HostCount, InfectedCount and PatchedCount;
    /// </summary>
    private void UpdateSpriteRenderersToSIRProportion() {
        float patchedPercent = ((float) PatchedCount) / HostCount;
        float infectedPercent = ((float) InfectedCount) / HostCount;

        // Because the patched is below the infected it needs the infected height plus the patched height
        patchedSpriteRenderer.transform.localScale = new Vector3(1, patchedPercent + infectedPercent, 1);

        // This just needs the infected height
        infectedSpriteRenderer.transform.localScale = new Vector3(1, infectedPercent, 1);
    }

    /// <summary>
    /// Updates the linerenderer position of the 'other' node.
    /// </summary>
    public void LineRendererUpdatePosition(NodeHandler other) {
        lineRenderer.SetPosition(GetPositionOfNodeInLineRenderer(GetPositionOfNodeInList(other)), other.PositionMiddle);
    }

    /// <summary>
    /// Returns the position of the given node in the nodes list
    /// </summary>
    private int GetPositionOfNodeInList(NodeHandler other) {
        for (int i = 0; i < connectedTo.Count; i++)
            if (other.Equals(connectedTo[i].OtherNode(this)))
                return i;

        return -1;
    }

    /// <summary>
    /// Returns the position of the given node (from it's index in the connectedTo list) in linerenderer
    /// </summary>
    private int GetPositionOfNodeInLineRenderer(int index) {
        return index * 2 + 1;
    }

    /// <summary>
    /// Updates the line renderer's positions because the transform.position changed.
    /// </summary>
    private void LineRendererUpdateToTransform() {
        for (int i = 0; i < lineRenderer.positionCount; i += 2) { 
            lineRenderer.SetPosition(i, PositionMiddle);
        }
    }
    
    /// <summary>
    /// Connect this node to the other if it has not been connected already. 
    /// Also updates the linerednerer
    /// </summary>
    /// <return>Whether it could be connected or not</return>
    public bool ConnectTo(NodeConnection other) {
        if (!connectedTo.Contains(other)) {
            connectedTo.Add(other);

            // make a line to there as well
            // first we have to bring it back to this point only then can we make a line to there
            lineRenderer.positionCount += 2;
            lineRenderer.SetPosition(lineRenderer.positionCount - 2, PositionMiddle);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, other.OtherNode(this).PositionMiddle);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Disconnects the given connection
    /// </summary>
    public void Disconnect(NodeConnection connection) {
        if (connectedTo.Remove(connection)) {
            // redraw the lines
            LineRendererUpdateAllPositions();
        }
    }

    /// <summary>
    /// Disconnect the given node
    /// </summary>
    public void DisconnectFromNode(NodeHandler other) {
        int index = GetPositionOfNodeInList(other);
        if (index != -1) {
            connectedTo.RemoveAt(index);

            // redraw the lines
            LineRendererUpdateAllPositions();
        }
    }

    /// <summary>
    /// Updates the position of all the lines in this node
    /// </summary>
    public void LineRendererUpdateAllPositions() {
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = connectedTo.Count * 2;

        for (int i = 0; i < connectedTo.Count; i++) {
            lineRenderer.SetPosition(i * 2, PositionMiddle);
            lineRenderer.SetPosition(i * 2 + 1, connectedTo[i].OtherNode(this).PositionMiddle);
        }
    }

    /// <returns>Whether the sprite of this node contains the pos in the world</returns>
    public bool Contains(Vector2 pos) {
        return fadeSpriteRenderer.bounds.Contains(pos);
    }

    public override int GetHashCode() {
        return name.GetHashCode();
    }
    public override bool Equals(object other) {
        if (other is NodeHandler) {
            return ((NodeHandler) other).name.Equals(name);
        }

        return base.Equals(other);
    }
}
