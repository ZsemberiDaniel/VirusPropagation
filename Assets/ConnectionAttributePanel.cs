using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionAttributePanel : MonoBehaviour {

    [SerializeField]
    private TMP_Text fromText;

    [SerializeField]
    private TMP_Text toText;

    [SerializeField]
    private Button deleteButton;

    private GraphHandler graph;
    private NodeConnection selectedConnection;

	void Start() {
        graph = FindObjectOfType<GraphHandler>();

        UpdateToConnectionNone();

        deleteButton.onClick.AddListener(() => {
            if (selectedConnection != null)
                graph.RemoveConnectionWithSurePanel(selectedConnection);
        });
	}

    public void UpdateToConnectionNone() {
        fromText.text = "None";
        toText.text = "None";

        deleteButton.interactable = false;
    }

    public void UpdateToConnection(NodeConnection connection) {
        if (connection == null) return;

        selectedConnection = connection;

        deleteButton.interactable = true;

        fromText.text = connection.NodeOne.name;
        toText.text = connection.NodeTwo.name;
    }
}
