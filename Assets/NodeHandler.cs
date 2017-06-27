using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class NodeHandler : MonoBehaviour {

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color connectStartColor;

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
    /// Size of the sprite
    /// </summary>
    public Vector3 Size {
        get {
            return spriteRenderer.bounds.size;
        }
    }

    private NodeNameHandler nodeNameHandler;
    public NodeNameHandler NodeNameHandler {
        get { return nodeNameHandler; }
        set { nodeNameHandler = value; }
    }
    public bool HasNodeNameHandler() { return nodeNameHandler != null; }

    public bool IsRendered() { return spriteRenderer.isVisible; }
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
                connectedTo[i].UpdateLinePositionOf(this);
        }
    }

    private List<NodeHandler> connectedTo;
    public ReadOnlyCollection<NodeHandler> ConnectedTo {
        get {
            return connectedTo.AsReadOnly();
        }
    }

    private SpriteRenderer spriteRenderer;
    /// <summary>
    /// Every even position is the node's position. Every odd position is the node it is connected to in the order of the list.
    /// </summary>
    private LineRenderer lineRenderer;
    
	void Start () {
        connectedTo = new List<NodeHandler>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();

        name = RandomWords.getRandomWords(2);
    }
	
	void Update () {
        // Update color based on the selection
        if (SelectedStart) spriteRenderer.color = connectStartColor;
        else if (Selected) spriteRenderer.color = selectedColor;
        else spriteRenderer.color = normalColor;
	}

    /// <summary>
    /// Updates the linerenderer position of the 'other' node.
    /// </summary>
    public void UpdateLinePositionOf(NodeHandler other) {
        lineRenderer.SetPosition(GetPositionOfNodeInLineRenderer(connectedTo.IndexOf(other)), other.Position + other.Size / 2f);
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
            lineRenderer.SetPosition(i, transform.position + Size / 2f);
        }
    }
    
    /// <summary>
    /// Connect this node to the other if it has not been connected already. 
    /// Also updates the linerednerer
    /// </summary>
    /// <return>Whether it could be connected or not</return>
    public bool ConnectTo(NodeHandler other) {
        if (!connectedTo.Contains(other)) {
            connectedTo.Add(other);

            // make a line to there as well
            // first we have to bring it back to this point only then can we make a line to there
            lineRenderer.positionCount += 2;
            lineRenderer.SetPosition(lineRenderer.positionCount - 2, transform.position + Size / 2f);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, other.transform.position + other.Size / 2f);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Disconnects from the given node (if it is connected) then updates the linererndener
    /// </summary>
    public void DisconnectFrom(NodeHandler other) {
        if (connectedTo.Remove(other)) {
            // redraw the lines
            UpdateAllLineRendererPositions();
        }
    }

    /// <summary>
    /// Updates the position of all the lines in this node
    /// </summary>
    public void UpdateAllLineRendererPositions() {
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = connectedTo.Count * 2;

        for (int i = 0; i < connectedTo.Count; i++) {
            lineRenderer.SetPosition(i * 2, transform.position + Size / 2f);
            lineRenderer.SetPosition(i * 2 + i, connectedTo[i].transform.position + connectedTo[i].Size / 2f);
        }
    }

    /// <returns>Whether the sprite of this node contains the pos in the world</returns>
    public bool Contains(Vector2 pos) {
        return spriteRenderer.bounds.Contains(pos);
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
