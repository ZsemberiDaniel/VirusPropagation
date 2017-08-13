using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoneSelectedPanel : MonoBehaviour, AttributePanel {

    private const float transformValueMultiplier = 0.2f;
    private GameState gameState;

    [SerializeField]
    private TMP_InputField packetSizeInput;

    [SerializeField]
    private Scrollbar S2IScrollbar;
    [SerializeField]
    private TMP_Text S2IText;
    [SerializeField]
    private Scrollbar S2RScrollbar;
    [SerializeField]
    private TMP_Text S2RText;
    [SerializeField]
    private Scrollbar I2RScrollbar;
    [SerializeField]
    private TMP_Text I2RText;

    void Start() {
        gameState = FindObjectOfType<GameState>();

        // Packet size
        packetSizeInput.onValueChanged.AddListener(newCount => {
            if (newCount.Length == 0) return;
            try {
                gameState.PacketSize = int.Parse(packetSizeInput.text);
            } catch (System.ArgumentException e) {
                ErrorPanel.Instance.ShowError("This should never ever happen.\n" + e.Message);
            }
        });
        // Set it to the before value so if it ha been changed invalidly it doesnt freak out
        packetSizeInput.onEndEdit.AddListener(newCount => {
            packetSizeInput.text = gameState.PacketSize.ToString();
        });

        // transform values
        S2IText.text = (gameState.S2I).ToString("0.###");
        S2IScrollbar.onValueChanged.AddListener(value => {
            gameState.S2I = value * transformValueMultiplier;
            S2IText.text = (gameState.S2I).ToString("0.###");
        });

        S2RText.text = (gameState.S2R).ToString("0.###");
        S2RScrollbar.onValueChanged.AddListener(value => {
            gameState.S2R = value * transformValueMultiplier;
            S2RText.text = (gameState.S2R).ToString("0.###");
        });
        
        I2RText.text = (gameState.I2R).ToString("0.###");
        I2RScrollbar.onValueChanged.AddListener(value => {
            gameState.I2R = value * transformValueMultiplier;
            I2RText.text = (gameState.I2R).ToString("0.###");
        });

        gameState.OnVirusAttributeChange += OnVirusAttrChange;
    }

    private void OnVirusAttrChange(float S2I, float I2R, float S2R, int packetSize) {
        packetSizeInput.text = packetSize.ToString();

        S2IScrollbar.value = S2I / transformValueMultiplier;
        S2IText.text = S2I.ToString("0.###");
        S2RScrollbar.value = S2R / transformValueMultiplier;
        S2RText.text = S2R.ToString("0.###");
        I2RScrollbar.value = gameState.I2R / transformValueMultiplier;
        I2RText.text = I2R.ToString("0.###");
    }

    public void FocusFirstUIElement() {
        
    }
}
