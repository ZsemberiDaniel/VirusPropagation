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
        if (data == null) return;

        data.gameObject.SetActive(false);
        this.data.Enqueue(data);
    }

    public void Show() {
        gameObject.SetActive(true);
        // we need to update their position so they don't jump there on the screen
        System.Array.ForEach(transform.GetComponentsInChildren<ConnectionData>(), (connection) => {
            connection.Update();
        });
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
