public class NodeConnection {

    private NodeHandler nodeOne;
    private NodeHandler nodeTwo;

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
	
}
