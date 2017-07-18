using System.Collections.Generic;
using UnityEngine;

public class ConnectionDataParent : MonoBehaviour {

    [SerializeField]
    private GameObject connectionNamePrefab;
    
    private Queue<ConnectionData> data;

    void Start() {
        data = new Queue<ConnectionData>();
    }

    public ConnectionData GetNewData(NodeConnection connectionToFolloe) {
        ConnectionData data;

        if (this.data.Count > 0) data = this.data.Dequeue();
        else data = Instantiate(connectionNamePrefab, transform).GetComponent<ConnectionData>();

        data.SetConnectionToFollow(connectionToFolloe);
        data.Size = new Vector2(Camera.main.pixelWidth * 0.05f, Camera.main.pixelWidth * 0.01f);

        data.gameObject.SetActive(true);
        return data;
    }

    public void QueueData(ConnectionData data) {
        data.gameObject.SetActive(false);
        this.data.Enqueue(data);
    }
}
