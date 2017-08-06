using UnityEngine;
using TMPro;

public class NoneSelectedPanel : MonoBehaviour, AttributePanel {

    private GameState gameState;

    [SerializeField]
    private TMP_InputField packetSizeInput;

    void Start() {
        gameState = FindObjectOfType<GameState>();

        // Packet size
        packetSizeInput.text = gameState.packetSize.ToString();
        packetSizeInput.onValueChanged.AddListener(newCount => {
            if (newCount.Length == 0) return;
            try {
                gameState.packetSize = int.Parse(packetSizeInput.text);
            } catch (System.ArgumentException e) {
                    // TODO wrong number
            }
        });
        // Set it to the before value so if it ha been changed invalidly it doesnt freak out
        packetSizeInput.onEndEdit.AddListener(newCount => {
            packetSizeInput.text = gameState.packetSize.ToString();
        });
    }

    public void FocusFirstUIElement() {
        
    }
}
