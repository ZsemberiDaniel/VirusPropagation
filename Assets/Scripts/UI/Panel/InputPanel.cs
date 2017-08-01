using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class InputPanel : MonoBehaviour {

    private float animationSpeed = 1f;

    private RectTransform rectTransform;

    [SerializeField]
    private RectTransform dropdownButtonRectTranform;
    [SerializeField]
    private Button parseButton;
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button randomButton;
    [SerializeField]
    private TMP_InputField randomNodeInputField;

    private GraphHandler graphHandler;

    private bool panelShown = false;
    public bool IsShown {
        get { return panelShown; }
    }
    
	void Start() {
        rectTransform = GetComponent<RectTransform>();
        // resize and reposition
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Camera.main.pixelHeight * 0.8f);
        rectTransform.anchoredPosition = new Vector2(0, rectTransform.sizeDelta.y / 2f);

        graphHandler = FindObjectOfType<GraphHandler>();

        // dropdown click listener
        dropdownButtonRectTranform.GetComponent<Button>().onClick.AddListener(() => {
            DropdownClicked();
        });

        // parse button click listener
        parseButton.onClick.AddListener(() => {
            ParserNode[] nodes;

            try { 
                nodes = InputParser.Parse(inputField.text, 
                    out graphHandler.GameState.S2I, 
                    out graphHandler.GameState.I2R, 
                    out graphHandler.GameState.S2R, 
                    out graphHandler.GameState.packetSize);
            } catch (System.FormatException e) {
                // TODO input not correct!!
                return;
            }

            graphHandler.SeperateNodes(graphHandler.AddNodes(nodes));
            inputField.text = "";
            Close();
        });

        // random button click listener
        randomButton.onClick.AddListener(() => {
            ParserNode[] nodes = InputParser.Random(int.Parse(randomNodeInputField.text),
                out graphHandler.GameState.S2I,
                out graphHandler.GameState.I2R,
                out graphHandler.GameState.S2R,
                out graphHandler.GameState.packetSize);

            graphHandler.SeperateNodes(graphHandler.AddNodes(nodes));
            randomNodeInputField.text = "";
            Close();
        });

        // game state change
        FindObjectOfType<GameState>().OnStateChange += (oldState, newState) => {
            if (newState == GameState.State.Simulating) {
                dropdownButtonRectTranform.gameObject.SetActive(false);

                Close();
            } else if (oldState == GameState.State.Simulating) {
                dropdownButtonRectTranform.gameObject.SetActive(true);
            }
        };
	}

    private void Update() {
        // Close on escape key
        if (panelShown && Input.GetKeyDown(KeyCode.Escape)) {
            Close();
        }
    }

    /// <summary>
    /// Called when the dropdown button is clicked
    /// </summary>
    public void DropdownClicked() {
        if (!panelShown) Open();
        else Close();
    }

    public void Open() {
        panelShown = true;

        rectTransform.DOAnchorPosY(-rectTransform.sizeDelta.y / 2f, 0.3f / animationSpeed);
        dropdownButtonRectTranform.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.3f / animationSpeed);
    }
    public void Close() {
        panelShown = false;

        rectTransform.DOAnchorPosY(rectTransform.sizeDelta.y / 2f, 0.3f / animationSpeed);
        dropdownButtonRectTranform.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.3f / animationSpeed);
    }
}
