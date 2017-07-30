﻿using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class GraphHandler : MonoBehaviour {

    // The whole game state
    private GameState gameState;
    public GameState GameState {
        get { return gameState; }
    }


    // For handling nodes and connections in the scene
    [SerializeField]
    private GameObject nodePrefab;
    private GameObject nodeParent;
    [SerializeField]
    private GameObject connectionPrefab;
    private GameObject connectionParent;

    private NodeNameParent nodeNameParent;
    private ConnectionDataParent connectionDataParent;

    private List<NodeHandler> nodes;
    private List<NodeConnection> nodeConnections;


    // UI
    [SerializeField]
    private Toggle addEditToggle;

    [SerializeField]
    private SurelyDeletePanel areYouSurePanel;

    private LineRenderer connectStartLineRenderer;

    private EditPanel editPanel;


    // Selections
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

    private int selectedConnection = -1;
    private int SelectedConnection {
        get { return selectedConnection; }
        set {
            selectedConnection = value;
            UpdateSelectedConnection();
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


    // Simulation
    private Coroutine simulationCoroutine;
    private bool simulationPaused = false;
    public bool SimulationPaused {
        get { return simulationPaused; }
    }

    private PacketPooling packetPooling;

    void Awake() {
        gameState = ScriptableObject.CreateInstance<GameState>();
        gameState.hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    void Start() {
        #region Find stuff
        editPanel = FindObjectOfType<EditPanel>();
        nodeNameParent = FindObjectOfType<NodeNameParent>();
        connectionDataParent = FindObjectOfType<ConnectionDataParent>();
        packetPooling = FindObjectOfType<PacketPooling>();

        nodeParent = new GameObject("NodeParent");
        nodeParent.transform.parent = transform;
        connectionParent = new GameObject("ConnectionParent");
        connectionParent.transform.parent = transform;

        connectStartLineRenderer = GetComponent<LineRenderer>();

        nodes = new List<NodeHandler>();
        nodeConnections = new List<NodeConnection>();
        #endregion

        Settings.Instance.onShowNodeLabelChanged += (show) => {
            if (show) nodeNameParent.Show();
            else nodeNameParent.Hide();
        };
        Settings.Instance.onShowConnectionLabelChanged += (show) => {
            if (show) connectionDataParent.Show();
            else connectionDataParent.Hide();
        };

        // when game state changes
        gameState.OnStateChange += (oldState, newState) => {
            switch (newState) {
                case GameState.State.Adding:
                    addEditToggle.isOn = false;

                    SelectedNode = -1;
                    SelectedConnection = -1;
                    break;
                case GameState.State.Editing:
                    addEditToggle.isOn = true;
                    break;
            }
        };
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
                nodes[i].NodeNameHandler = nodeNameParent.GetNewText(nodes[i]);
            } else if (nodes[i].DoesNoLongerNeedNodeNameHandler()) {
                nodeNameParent.QueueText(nodes[i].NodeNameHandler);
                nodes[i].NodeNameHandler = null;
            }
        }

        // Connection data text
        for (int i = 0; i < nodeConnections.Count; i++) {
            if (nodeConnections[i].DoesNeedConnectionDataHandler()) {
                nodeConnections[i].ConnectionData = connectionDataParent.GetNewData(nodeConnections[i]);
            } else if (nodeConnections[i].DoesNoLongerNeedConnectionDataHandler()) {
                connectionDataParent.QueueData(nodeConnections[i].ConnectionData);
                nodeConnections[i].ConnectionData = null;
            }
        }

        #region Input
        // Not above GUI
        if (!EventSystem.current.IsPointerOverGameObject()) {
            // which node the mouse is over
            // only avliable when either mouse 0 or 1 is held down
            int aboveNodeIndex = -1;
            int aboveLineIndex = -1;

            // See whether the user clicked a node or a line
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                for (int i = 0; i < nodes.Count; i++) {
                    if (nodes[i].Contains(mouseWorldPos)) {
                        aboveNodeIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < nodeConnections.Count; i++) {
                    if (nodeConnections[i].Contains(mouseWorldPos, Camera.main.orthographicSize * 0.05f)) {
                        aboveLineIndex = i;
                        break;
                    }
                }
            }

            // Left mouse button down
            if (Input.GetMouseButtonDown(0)) {
                // Adding
                if (gameState.gState == GameState.State.Adding) {
                    if (aboveNodeIndex == -1) {
                        AddNodeAtPos(mouseWorldPos);
                    }
                // Select
                } else if (gameState.gState == GameState.State.Editing) {
                    mousePanStartedAt = mouseWorldPos;
                    if (aboveNodeIndex != -1) { 
                        SelectedNode = aboveNodeIndex;
                        SelectedConnection = -1;
                    } else if (aboveLineIndex != -1) {
                        SelectedConnection = aboveLineIndex;
                        SelectedNode = -1;
                    } else {
                        SelectedNode = -1;
                        SelectedConnection = -1;
                    }

                    // A node is selected for connection -> deselect
                    if (ConnectStartNode != -1)
                        ConnectStartNode = -1;
                }
            }

            // Left mouse button held
            if (Input.GetMouseButton(0)) {
                // Editing
                // The second part: we started the holding down on a node or we have not released the button yet
                if (gameState.gState == GameState.State.Editing) {
                    // we are above a node when moving the mouse around so let that be the moving node
                    if (aboveNodeIndex != -1 && whichNodeIsBeingMoved == -1) { 
                        whichNodeIsBeingMoved = aboveNodeIndex;
                        SelectedNode = whichNodeIsBeingMoved;
                        SelectedConnection = -1;
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
                if (gameState.gState == GameState.State.Editing) {
                    whichNodeIsBeingMoved = -1;
                }
            }

            // Right mouse button
            if (Input.GetMouseButtonDown(1)) {
                // Adding
                if (gameState.gState == GameState.State.Adding) {
                    // Remove if a node is clicked
                    if (aboveNodeIndex != -1) {
                        RemoveNodeWithIndexWithSurePanel(aboveNodeIndex);
                    // Remove if a connection is clicked
                    } else if (aboveLineIndex != -1) {
                        RemoveConnectionWithIndexWithSurePanel(aboveLineIndex);
                    }
                // Editing
                } else if (gameState.gState == GameState.State.Editing) {
                    // Selected none -> deselect the start thing
                    if (aboveNodeIndex == -1) { 
                        ConnectStartNode = -1;

                        // Remove if a connection is clicked
                        if (aboveLineIndex != -1)
                            RemoveConnectionWithIndexWithSurePanel(aboveLineIndex);
                    }
                    // We have a start node selected so connect them
                    else if (ConnectStartNode != -1) {
                        ConnectTwoNodes(ConnectStartNode, aboveNodeIndex, 1000);

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
        }

        // add or edit toggle
        if (Input.GetKeyDown(KeyCode.Tab)) {
            gameState.ToggleAddEditMode();
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
    /// Connect these node mutually in the nodes list. The indices are the ones from the nodes list
    /// </summary>
    private void ConnectTwoNodes(int index1, int index2, int capacity) {
        ConnectTwoNodes(nodes[index1], nodes[index2], capacity);
    }

    /// <summary>
    /// Connect these nodes mutually
    /// </summary>
    private void ConnectTwoNodes(NodeHandler one, NodeHandler two, int capacity = 1000) {
        if (one.Equals(two)) return;

        // If there already is a connection like this then return
        var connectedToOne = one.GetConnectedToNodes();
        for (int i = 0; i < connectedToOne.Count; i++)
            if (connectedToOne[i].Equals(two))
                return;

        // So at this point we are sure that this is not a duplicate connection
        var connection = Instantiate(connectionPrefab, connectionParent.transform).GetComponent<NodeConnection>();
        connection.SetNodes(one, two);
        connection.Capacity = capacity;

        one.ConnectTo(two, connection);
        two.ConnectTo(one, connection);

        nodeConnections.Add(connection);

        // Reflect change son the GUI
        editPanel.NodeAttributePanel.UpdateNodeCurrent();
    }

    /// <summary>
    /// Removes the node if it is present in the graph with the sure panel.
    /// </summary>
    public void RemoveNodeWithSurePanel(NodeHandler node) {
        int index = nodes.IndexOf(node);
        if (index != -1)
            RemoveNodeWithIndexWithSurePanel(index);
    }

    /// <summary>
    /// Removes the given node in the graph if it is in here.
    /// </summary>
    public void RemoveNode(NodeHandler node) {
        int index = nodes.IndexOf(node);
        if (index != -1) RemoveNodeWithIndex(index);
    }

    /// <summary>
    /// Removes the node in the indexth place of the nodes array without the are you sure panel.
    /// </summary>
    private void RemoveNodeWithIndex(int index) {
        // First update node connections that it is connected to
        var others = nodes[index].GetConnectedToNodes();
        for (int k = 0; k < others.Count; k++)
            others[k].Disconnect(nodes[index]);

        // Delete the connections here, in the graph
        for (int i = nodeConnections.Count - 1; i >= 0; i--) { 
            if (nodeConnections[i].Contains(nodes[index])) {
                RemoveConnectionWithIndex(i);
            }
        }

        nodeNameParent.QueueText(nodes[index].NodeNameHandler);

        Destroy(nodes[index].gameObject);
        nodes.RemoveAt(index);
    }

    /// <summary>
    /// Removes the node in the indexth place of the nodes array WITH the are you sure panel.
    /// </summary>
    private void RemoveNodeWithIndexWithSurePanel(int index) {
        // Hide immediatly so if it is shown at another node it can show the panel again
        areYouSurePanel.HideImmediatly();

        // Ask the user whether he is sure of deleting the node
        areYouSurePanel.ShowAtWithAction(
            Input.mousePosition,
            (isSure) => {
                if (isSure) {
                    RemoveNodeWithIndex(index);
                }
            }
        );
    }

    /// <summary>
    /// Removes the given connection if it exists in this graph while showing the sure panel
    /// </summary>
    public void RemoveConnectionWithSurePanel(NodeConnection connection) {
        int index = nodeConnections.IndexOf(connection);
        if (index != -1)
            RemoveConnectionWithIndexWithSurePanel(index);
    }

    /// <summary>
    /// Removes the given connection if it exists in this graph
    /// </summary>
    public void RemoveConnection(NodeConnection connection) {
        int index = nodeConnections.IndexOf(connection);
        if (index != -1)
            RemoveConnectionWithIndex(index);
    }

    /// <summary>
    /// Removes the node connection with the given index while asking the user whether he is sure about the removal
    /// </summary>
    private void RemoveConnectionWithIndexWithSurePanel(int index) {
        // Hide immediatly so if it is shown at another connection it can show the panel again
        areYouSurePanel.HideImmediatly();

        // Ask the user whether he is sure of deleting the connection
        areYouSurePanel.ShowAtWithAction(
            Input.mousePosition,
            (isSure) => {
                if (isSure) {
                    RemoveConnectionWithIndex(index);
                }
            }
        );
    }

    /// <summary>
    /// Removes the node connection with the given index.
    /// </summary>
    private void RemoveConnectionWithIndex(int index) {
        nodeConnections[index].DisconnectBoth();
        connectionDataParent.QueueData(nodeConnections[index].ConnectionData);

        Destroy(nodeConnections[index].gameObject);
        nodeConnections.RemoveAt(index);
    }

    /// <summary>
    /// Adds a node at the given position
    /// </summary>
    private NodeHandler AddNodeAtPos(Vector3 position) {
        NodeHandler added = Instantiate(nodePrefab, position, Quaternion.identity, nodeParent.transform).GetComponent<NodeHandler>();
        nodes.Add(added);

        return added;
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

            editPanel.SwitchTo(EditPanel.AttributePanelType.node);
            editPanel.NodeAttributePanel.UpdateNodeTo(nodes[SelectedNode]);
        } else {
            editPanel.NodeAttributePanel.UpdateToNoneSelected();
        }
    }

    /// <summary>
    /// Updates the variable, colors etc.
    /// </summary>
    private void UpdateSelectedConnection() {
        // Deselect all
        for (int i = 0; i < nodeConnections.Count; i++) nodeConnections[i].Deselect();

        if (SelectedConnection != -1) {
            // Select the current one
            nodeConnections[SelectedConnection].Select();

            editPanel.SwitchTo(EditPanel.AttributePanelType.connection);

            editPanel.ConnectionAttributePanel.UpdateToConnection(nodeConnections[SelectedConnection]);
        } else {
            editPanel.ConnectionAttributePanel.UpdateToConnectionNone();
        }
    }

    public void StartSimulation() {
        gameState.gState = GameState.State.Simulating;
        simulationCoroutine = StartCoroutine(StepVirus());
    }
    public void PauseSimulationToggle() {
        if (simulationPaused) UnPauseSimulation();
        else PauseSimulation();
    }
    public void PauseSimulation() {
        simulationPaused = true;
    }
    public void UnPauseSimulation() {
        simulationPaused = false;
    }
    public void StopSimulation() {
        // TODO stop the simulation
        gameState.gState = GameState.State.Adding;
        StopCoroutine(simulationCoroutine);
    }
    private IEnumerator StepVirus() {
        while (true) {
            while (simulationPaused) {
                yield return null;
            }

            // Spread the virus
            Dictionary<Tuple<NodeHandler, NodeHandler>, int> infectCounts = new Dictionary<Tuple<NodeHandler, NodeHandler>, int>();
            for (int i = 0; i < nodes.Count; i++) {
                int startInfectedCount = nodes[i].InfectedCount; // because more will be added this tick and those are 'sleeping'
                for (int h = 0; h < startInfectedCount; h++) {
                    // Has a random chance to infect
                    if (UnityEngine.Random.Range(0f, 1f) <= gameState.S2I) {
                        var infect = nodes[i].GetRandomConnection();

                        int count;

                        // infect itself
                        if (infect.connection == null) {
                            var tuple = Tuple.Create(nodes[i], nodes[i]);
                            infectCounts.TryGetValue(tuple, out count);
                            infectCounts[tuple] = count + 1;
                        } else { // infect someone else -> we need to check the connection bandwidth
                            // if packet can be sent
                            if (infect.node.HasInfectable() && infect.connection.SendPacket(gameState.packetSize)) {
                                var tuple = Tuple.Create(nodes[i], infect.node);
                                infectCounts.TryGetValue(tuple, out count);
                                infectCounts[tuple] = count + 1;
                            }
                        }
                    }
                }
            }
            float travelTime = 0.3f / Settings.Instance.SimulationSpeed;
            foreach (var key in infectCounts.Keys) {
                if (!key.Item1.Equals(key.Item2)) {
                    packetPooling.GetPacket().MoveThenPool(key.Item1.PositionMiddle, 
                        key.Item2.PositionMiddle, travelTime, packetPooling);
                }
            }

            // do all the math after the packets arrived
            StartCoroutine(ExecuteAfterSeconds(() => {
                // infect
                foreach (var key in infectCounts.Keys) {
                    key.Item2.Infect(infectCounts[key]);
                }

                // recover the infected and patch the not infected
                for (int i = 0; i < nodes.Count; i++) {
                    // recover infected
                    int startInfectedCount = nodes[i].InfectedCount; // because some will be removed this tick
                    for (int h = 0; h < startInfectedCount; h++) {
                        if (UnityEngine.Random.Range(0f, 1f) <= gameState.I2R) {
                            nodes[i].RecoverInfected(1);
                        }
                    }

                    // patch not infected
                    int startNormalHost = nodes[i].NormalCount;
                    for (int h = 0; h < startNormalHost; h++) {
                        if (UnityEngine.Random.Range(0f, 1f) <= gameState.S2R) {
                            nodes[i].RecoverNormal(1);
                        }
                    }
                }
            }, travelTime));

            // Reset the bandwidth on the connections
            for (int i = 0; i < nodeConnections.Count; i++) {
                nodeConnections[i].ResetBandwidth();
            }
            yield return new WaitForSeconds(0.1f / Settings.Instance.SimulationSpeed);
        }
    }
    private IEnumerator ExecuteAfterSeconds(UnityAction action, float time) {
        yield return new WaitForSeconds(time);

        action.Invoke();
    }

    /// <summary>
    /// Adds the nodes based on the given ParserNodes. If a node cannot be added because it's name
    /// then it won't be added. The same goes for a connection. If either one of the connecting neighbours cannot
    /// be found then the connection won't be established.
    /// </summary>
    public NodeHandler[] AddNodes(ParserNode[] nodesToBeAdded) {
        NodeHandler[] newNodes = new NodeHandler[nodesToBeAdded.Length];

        // add all the nodes
        for (int i = 0; i < nodesToBeAdded.Length; i++) { 
            if (IsNameTaken(nodesToBeAdded[i].name)) continue;

            // place it at a random pos
            float areaMultiply = Mathf.Sqrt(nodesToBeAdded.Length);
            Vector3 position = new Vector3(
                UnityEngine.Random.Range(0f, areaMultiply * 2f) - areaMultiply,
                UnityEngine.Random.Range(0f, areaMultiply * 2f) - areaMultiply
                );

            // Add node
            newNodes[i] = AddNodeAtPos(position);
            newNodes[i].Graph = this;
            newNodes[i].name = nodesToBeAdded[i].name;
            newNodes[i].HostCount = nodesToBeAdded[i].hostCount;
            newNodes[i].InfectedCount = nodesToBeAdded[i].infectedCount;
        }

        // Now connect the to their neighbours
        for (int i = 0; i < newNodes.Length; i++) {
            // Get the NodeHandlers the current NodeHandler is connected to
            List<System.Tuple<NodeHandler, int>> nodesConnectedTo = new List<System.Tuple<NodeHandler, int>>();
            var ct = nodesToBeAdded[i].ConnectedTo;

            for (int k = 0; k < nodes.Count; k++)
                for (int j = 0; j < ct.Count; j++)
                    if (ct[j].connectedTo.name == nodes[k].name)
                        nodesConnectedTo.Add(new System.Tuple<NodeHandler, int>(nodes[k], ct[j].capacity));

            for (int k = 0; k < nodesConnectedTo.Count; k++)
                ConnectTwoNodes(newNodes[i], nodesConnectedTo[k].Item1, nodesConnectedTo[k].Item2);
        }

        return newNodes;
    } 

    /// <summary>
    /// Seperates the given nodes. If the given nodes are null all the nodes will be seperated
    /// </summary>
    public void SeperateNodes(NodeHandler[] nodes = null, float repulsion = 3f, float stiffness = 0.08f, float springLength = 8f) {
        NodePhysicsWrapper[] _nodes;
        // if nodes is null then use all nodes
        if (nodes == null) _nodes = this.nodes.Select(node => new NodePhysicsWrapper(node)).ToArray();
        else _nodes = nodes.Select(node => new NodePhysicsWrapper(node)).ToArray();

        StartCoroutine(SeperateNodeCoroutine(_nodes, repulsion, stiffness, springLength));
    }
    private IEnumerator SeperateNodeCoroutine(NodePhysicsWrapper[] _nodes, float repulsion, float stiffness, float springLength) {
        /* while (true) { // Used for debugging
            
            float areaMultiply = Mathf.Sqrt(_nodes.Length);
            foreach (NodePhysicsWrapper w in _nodes) {
                Vector3 position = new Vector3(
                    Random.Range(0f, areaMultiply * 2f) - areaMultiply,
                    Random.Range(0f, areaMultiply * 2f) - areaMultiply
                    );
                w.node.transform.position = position;
            }*/

        bool hasEnoughEnergy = true;
        int iterationCount = 0;
        int maxIteration = _nodes.Length > 10 ? (int) (50 * (1 + Mathf.Log10(_nodes.Length - 10))) : 50;
            
        // first get the center
        Vector3 centerPos = new Vector3();
        for (int i = 0; i < _nodes.Length; i++) centerPos += _nodes[i].node.Position;
        centerPos /= _nodes.Length;

        float minimum = float.MaxValue;

        while (hasEnoughEnergy && iterationCount <= maxIteration) {
            iterationCount++;

            // *** Apply coulomb's law ***
            // *** Attract to center ***

            for (int i = 0; i < _nodes.Length; i++) {
                for (int k = 0; k < _nodes.Length; k++) {
                    if (i != k) {
                        Vector3 d = _nodes[i].node.Position - _nodes[k].node.Position;
                        float distance = d.magnitude + 0.2f; // avoid massive forces at small distances (and division by zero)
                        Vector3 direction = d.normalized;

                        // apply coulomb force
                        _nodes[i].ApplyForce(direction * repulsion / (distance * distance * 0.5f));
                        _nodes[k].ApplyForce(direction * repulsion / (distance * distance * -0.5f));
                    }
                }

                // now we can attract to center
                var directionCenter = centerPos - _nodes[i].node.Position;
                _nodes[i].ApplyForce(directionCenter * (repulsion / 200f)); // make some times less influential than the coulomb force
            }

            // *** Apply Hookes's law ***
            for (int i = 0; i < _nodes.Length; i++) {
                var connectedToNames = _nodes[i].node.GetConnectedToNodes().Select(node => node.name);

                // Go through only the ones that are connected to this node
                for (int k = 0; k < _nodes.Length; k++) {
                    if (connectedToNames.Contains(_nodes[k].node.name)) { 
                        Vector3 d = _nodes[i].node.Position - _nodes[k].node.Position;
                        Vector3 direction = d.normalized;
                        float displacement = springLength - d.magnitude;

                        _nodes[i].ApplyForce(direction * stiffness * displacement * 0.5f);
                        _nodes[k].ApplyForce(direction * stiffness * displacement * -0.5f);
                    }
                }
            }

            // *** Update velocity and position ***
            // *** Calculate total energy ****
            float totalEnergy = 0f;
            for (int i = 0; i < _nodes.Length; i++) {
                // vel
                _nodes[i].velocity += _nodes[i].acceleration;
                _nodes[i].acceleration = new Vector2();

                // limit to max speed
                if (_nodes[i].velocity.magnitude > _nodes[i].maxSpeed)
                    _nodes[i].velocity = _nodes[i].velocity.normalized * _nodes[i].maxSpeed;

                // pos
                _nodes[i].node.Position += _nodes[i].velocity;

                // energy
                float speed = _nodes[i].velocity.magnitude;
                totalEnergy += speed * speed * _nodes[i].mass * 0.5f;
            }

            // If total energy goes below threshold stop it
            if (totalEnergy < 4f)
                hasEnoughEnergy = false;

            // so we sample the energy in the first 80% then if we find a smaller one than the smallest * 1.2f then stop there
            // TODO try average
            if (iterationCount > maxIteration * 0.8f) {
                if (totalEnergy < minimum * 1.1f)
                    hasEnoughEnergy = false;
            } else if (totalEnergy < minimum) {
                minimum = totalEnergy;
            }

            yield return null;
        }

        Debug.Log(iterationCount + " " + minimum);
        // yield return new WaitForSeconds(2.5f);
        gameState.gState = GameState.State.Editing;
        // } // Used for debugging
    }

    /// <summary>
    /// Used for the SeperateNodes function
    /// </summary>
    private struct NodePhysicsWrapper {
        public NodeHandler node;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
        public float maxSpeed;

        public NodePhysicsWrapper(NodeHandler node) {
            this.node = node;
            mass = 1f;
            maxSpeed = 2f;
            velocity = new Vector2();
            acceleration = new Vector2();
        }
        
        public void ApplyForce(Vector3 force) {
            acceleration += force / mass;
        }
    }
}
