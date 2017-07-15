using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public GraphHandler Graph {
        get { return graph; }
        set { this.graph = value; }
    }

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

    private NodeName nodeNameHandler;
    public NodeName NodeNameHandler {
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
        }
    }

    /// <summary>
    /// The middle position of this node
    /// </summary>
    public Vector3 PositionMiddle {
        get { return Position + Size / 2f; }
    }

    private List<NodeHandler> connectedTo;
    public ReadOnlyCollection<NodeHandler> ConnectedToNodes {
        get {
            return connectedTo.AsReadOnly();
        }
    }

    private SpriteRenderer fadeSpriteRenderer;
    private SpriteRenderer normalSpriteRenderer;
    private SpriteRenderer infectedSpriteRenderer;
    private SpriteRenderer patchedSpriteRenderer;
    
	void Awake() {
        connectedTo = new List<NodeHandler>();

        fadeSpriteRenderer = GetComponent<SpriteRenderer>();
        // Use numbers so the patched is always below the infected and the normal is the lowest
        normalSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        patchedSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        infectedSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        if (graph == null) graph = FindObjectOfType<GraphHandler>();

        // Set the proportion spriterenderers' colors
        normalSpriteRenderer.color = normalHostColor;
        infectedSpriteRenderer.color = infectedColor;
        patchedSpriteRenderer.color = patchedColor;

        // Set tha fade colors to the correct alpha
        selectedColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0.3f);
        connectStartColor = new Color(connectStartColor.r, connectStartColor.g, connectStartColor.b, 0.3f);

        if (name == "")
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
    /// Connect this node to the other if it has not been connected already. 
    /// Also updates the linerednerer
    /// </summary>
    /// <return>Whether it could be connected or not</return>
    public bool ConnectTo(NodeHandler other) {
        if (!connectedTo.Contains(other)) {
            connectedTo.Add(other);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Disconnects the given connection
    /// </summary>
    public void Disconnect(NodeHandler connection) {
        if (connectedTo.Remove(connection)) {
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
