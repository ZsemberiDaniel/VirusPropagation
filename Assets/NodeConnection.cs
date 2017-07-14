using UnityEngine;

public class NodeConnection {

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
    public bool Contains(Vector3 mouseWorldPos) {
        Vector3 pos1 = nodeOne.transform.position;
        Vector3 pos2 = nodeTwo.transform.position;

        return Mathf.Abs((pos2.y - pos1.y) * mouseWorldPos.x - (pos2.x - pos1.x) * mouseWorldPos.y + pos2.x * pos1.y - pos2.y * pos1.x) 
            / Mathf.Sqrt((pos2.y - pos1.y) * (pos2.y - pos1.y) + (pos2.x - pos1.x) * (pos2.x - pos1.x)) <= 1f;
    } 
    public bool Equals(NodeHandler one, NodeHandler two) {
        return Contains(one) && Contains(two);
    }
	
}
