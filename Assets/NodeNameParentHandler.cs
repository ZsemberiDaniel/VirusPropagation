using System.Collections.Generic;
using UnityEngine;

public class NodeNameParentHandler : MonoBehaviour {

    [SerializeField]
    private GameObject nodeNamePrefab;

    private Queue<NodeNameHandler> texts;
    
	void Start () {
        texts = new Queue<NodeNameHandler>();	
	}
	
    public NodeNameHandler GetNewText(NodeHandler nodeToFollow) {
        NodeNameHandler text;

        if (texts.Count > 0) text = texts.Dequeue();
        else text = Instantiate(nodeNamePrefab, transform).GetComponent<NodeNameHandler>();

        text.SetNodeToFollow(nodeToFollow);

        text.gameObject.SetActive(true);
        return text;
    }

    public void QueueText(NodeNameHandler text) {
        text.gameObject.SetActive(false);
        texts.Enqueue(text);
    }
	
}
