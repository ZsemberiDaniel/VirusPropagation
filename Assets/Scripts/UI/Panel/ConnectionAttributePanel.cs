using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConnectionAttributePanel : MonoBehaviour, AttributePanel {

    [SerializeField]
    private TMP_Text fromText;

    [SerializeField]
    private TMP_Text toText;

    [SerializeField]
    private Button deleteButton;

    [SerializeField]
    private TMP_InputField capacityInput;
    private Image capacityImage;

    private GraphHandler graph;
    private NodeConnection selectedConnection;

	void Start() {
        graph = FindObjectOfType<GraphHandler>();
        capacityImage = capacityInput.GetComponent<Image>();

        UpdateToConnectionNone();

        #region Input
        deleteButton.onClick.AddListener(() => {
            if (selectedConnection != null)
                graph.RemoveConnectionWithSurePanel(selectedConnection);
        });

        // Capacity
        capacityInput.onValueChanged.AddListener(newCount => {
            if (newCount.Length == 0) return;
            if (selectedConnection != null) {
                try {
                    selectedConnection.Capacity = int.Parse(capacityInput.text);
                } catch (ArgumentException e) {
                    ErrorPanel.Instance.ShowError("This should never ever happen " + e.Message);
                }
            }
        });
        // Set it to the before value so if it ha been changed invalidly it doesnt freak out
        capacityInput.onEndEdit.AddListener(newCount => {
            if (selectedConnection != null) {
                // some error happened
                if (capacityInput.text != selectedConnection.Capacity.ToString()) { 
                    capacityImage.DisplayError();
                    ErrorPanel.Instance.ShowError("This should never ever happen!");
                }

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

        FocusFirstUIElement();
    }

    public void FocusFirstUIElement() {
        capacityInput.ActivateInputField();
        capacityInput.Select();
    }
}
