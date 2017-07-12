public class NodeConnection {

    private NodeHandler nodeOne;
    private NodeHandler nodeTwo;

    private int capacity;
    public int Capacity {
        get { return capacity; }
        set { capacity = value; }
    }

    public NodeConnection(NodeHandler one, NodeHandler two) {
        this.nodeOne = one;
        this.nodeTwo = two;
    }

    public NodeHandler OtherNode(NodeHandler currNode) {
        if (currNode.Equals(nodeOne)) return nodeTwo;
        else if (currNode.Equals(nodeTwo)) return nodeOne;
        return null;
    }

    public bool Contains(NodeHandler node) {
        return nodeOne.Equals(node) || nodeTwo.Equals(node);
    }

    public bool Equals(NodeHandler one, NodeHandler two) {
        return Contains(one) && Contains(two);
    }
	
}
