using System.Collections.Generic;
using UnityEngine;

public class NodeNameParent : MonoBehaviour {

    [SerializeField]
    private GameObject nodeNamePrefab;

    private Queue<NodeName> texts;
    
	void Start () {
        texts = new Queue<NodeName>();	
	}
	
    public NodeName GetNewText(NodeHandler nodeToFollow) {
        NodeName text;

        if (texts.Count > 0) text = texts.Dequeue();
        else text = Instantiate(nodeNamePrefab, transform).GetComponent<NodeName>();

        text.SetNodeToFollow(nodeToFollow);
        text.Size = new Vector2(Camera.main.pixelWidth * 0.1f, Camera.main.pixelWidth * 0.03f);

        text.gameObject.SetActive(true);
        return text;
    }

    public void QueueText(NodeName text) {
        text.gameObject.SetActive(false);
        texts.Enqueue(text);
    }
}
