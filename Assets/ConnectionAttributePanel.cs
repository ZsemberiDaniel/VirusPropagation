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

    [SerializeField]
    private TMP_InputField capacityInput;

    private GraphHandler graph;
    private NodeConnection selectedConnection;

	void Start() {
        graph = FindObjectOfType<GraphHandler>();

        UpdateToConnectionNone();

        #region Input
        deleteButton.onClick.AddListener(() => {
            if (selectedConnection != null)
                graph.RemoveConnectionWithSurePanel(selectedConnection);
        });

        // Capacity
        capacityInput.onSubmit.AddListener(newCount => {
            if (selectedConnection != null) {
                try {
                    selectedConnection.Capacity = int.Parse(capacityInput.text);
                } catch (System.ArgumentException e) {
                    // TODO wrong number
                }
            }
        });
        // Set it to the before value so if it ha been changed invalidly it doesnt freak out
        capacityInput.onEndEdit.AddListener(newCount => {
            if (selectedConnection != null) {
                capacityInput.text = selectedConnection.Capacity.ToString();
            }
        });
        #endregion
    }

    public void UpdateToConnectionNone() {
        fromText.text = "None";
        toText.text = "None";
        capacityInput.text = "";

        capacityInput.interactable = false;
        deleteButton.interactable = false;
    }

    public void UpdateToConnection(NodeConnection connection) {
        if (connection == null) return;

        selectedConnection = connection;

        deleteButton.interactable = true;
        capacityInput.interactable = true;

        capacityInput.text = connection.Capacity.ToString();

        fromText.text = connection.NodeOne.name;
        toText.text = connection.NodeTwo.name;
    }
}
