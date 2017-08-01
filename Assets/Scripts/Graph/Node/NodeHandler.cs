using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;

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
            else if (value > HostCount - RecoveredCount) throw new System.ArgumentException("Infected count cannot be higher than host count minus patched!");
            else infectedCount = value;
        }
    }

    private int recoveredCount = 0;
    /// <summary>
    /// How many hosts are patched
    /// </summary>
    public int RecoveredCount {
        get { return recoveredCount; }
        set {
            if (value < 0) throw new System.ArgumentException("Patched count cannot be lower than 0!");
            else if (value > HostCount - InfectedCount) throw new System.ArgumentException("Patched count cannot be higher than host count minus infected!");
            else recoveredCount = value;
        }
    }

    /// <summary>
    /// The host count which are neither recovered, neither infected
    /// </summary>
    public int NormalCount {
        get { return hostCount - infectedCount - recoveredCount; }
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
    private Color recoveredColor;

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
    public bool HasNodeNameHandler() { return nodeNameHandler != null; }
    public bool DoesNeedNodeNameHandler() { return !HasNodeNameHandler() && IsRendered(); }
    public bool DoesNoLongerNeedNodeNameHandler() { return HasNodeNameHandler() && !IsRendered(); }

    /// <summary>
    /// The position of the node in the world. Please use this instead of transform.position.
    /// No longer need to use it, but will keep it here for legacy code
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

    private List<ConnectionWrapper> connectedTo;
    /// <summary>
    /// Does some computation so maybe store the result
    /// </summary>
    public ReadOnlyCollection<NodeHandler> GetConnectedToNodes() {
        return connectedTo.Select(element => element.node).ToList().AsReadOnly();
    }
    public ReadOnlyCollection<ConnectionWrapper> GetConnectedToNodesWithWrapper() { return connectedTo.AsReadOnly(); }
    /// <summary>
    /// Returns a random connection (may be itself, if that's the case the connection part will be null)
    /// </summary>
    public ConnectionWrapper GetRandomConnection() {
        return Random.Range(0, connectedTo.Count + 1) == 0 ?
            new ConnectionWrapper(this, null) : connectedTo[Random.Range(0, connectedTo.Count)];
    }

    private SpriteRenderer fadeSpriteRenderer;
    private SpriteRenderer normalSpriteRenderer;
    private SpriteRenderer infectedSpriteRenderer;
    private SpriteRenderer patchedSpriteRenderer;
    
	void Awake() {
        connectedTo = new List<ConnectionWrapper>();

        fadeSpriteRenderer = GetComponent<SpriteRenderer>();
        // Use numbers so the patched is always below the infected and the normal is the lowest
        normalSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        patchedSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        infectedSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        if (graph == null) graph = FindObjectOfType<GraphHandler>();

        // Set the proportion spriterenderers' colors
        normalSpriteRenderer.color = normalHostColor;
        infectedSpriteRenderer.color = infectedColor;
        patchedSpriteRenderer.color = recoveredColor;

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
        float patchedPercent = ((float) RecoveredCount) / HostCount;
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
    public bool ConnectTo(NodeHandler other, NodeConnection connection) {
        if (!IsConnectedTo(other)) {
            connectedTo.Add(new ConnectionWrapper(other, connection));

            return true;
        }

        return false;
    }

    /// <summary>
    /// Disconnects the given connection
    /// </summary>
    public void Disconnect(NodeHandler connection) {
        int at;
        if (IsConnectedTo(connection, out at)) {
            connectedTo.RemoveAt(at);
        }
    }

    public bool IsConnectedTo(NodeHandler other) {
        int at;
        return IsConnectedTo(other, out at);
    }
    public bool IsConnectedTo(NodeHandler other, out int at) {
        for (int i = 0; i < connectedTo.Count; i++) {
            if (connectedTo[i].node == other) {
                at = i;
                return true;
            }
        }
        at = -1;
        return false;
    }

    /// <returns>Whether the sprite of this node contains the pos in the world</returns>
    public bool Contains(Vector2 pos) {
        return fadeSpriteRenderer.bounds.Contains(pos);
    }


    /// <summary>
    /// Infect count amount hosts
    /// </summary>
    public void Infect(int count) {
        int normalCount = NormalCount;
        if (normalCount != 0) {
            infectedCount += Mathf.Min(count, normalCount);
        }
    }

    /// <summary>
    /// Recover count amount infected hosts
    /// </summary>
    public void RecoverInfected(int count) {
        if (infectedCount != 0) {
            count = Mathf.Min(count, infectedCount);
            infectedCount -= count;
            recoveredCount += count;
        }
    }

    /// <summary>
    /// Recoveres count amount of not infected hosts
    /// </summary>
    public void RecoverNormal(int count) {
        int normalCount = NormalCount;
        if (normalCount != 0) {
            count = Mathf.Min(count, normalCount);
            recoveredCount += count;
        }
    }

    /// <summary>
    /// Whether this group has infectable hosts (normal)
    /// </summary>
    public bool HasInfectable() {
        return NormalCount > 0;
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

    public struct ConnectionWrapper {
        public NodeHandler node;
        public NodeConnection connection;

        public ConnectionWrapper(NodeHandler node, NodeConnection connection) {
            this.node = node;
            this.connection = connection;
        }
    }
}
