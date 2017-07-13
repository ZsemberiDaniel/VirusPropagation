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

    private GraphHandler graphHandler;

    private bool panelShown = false;
    public bool IsShown {
        get { return panelShown; }
    }
    
	void Start() {
        rectTransform = GetComponent<RectTransform>();
        // resize and reposition
        rectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight * 0.8f);
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
                    out graphHandler.S2I, 
                    out graphHandler.I2R, 
                    out graphHandler.S2R, 
                    out graphHandler.packetSize);
            } catch (System.FormatException e) {
                // TODO input not correct!!
                return;
            }
            
            graphHandler.SeperateNodes(graphHandler.AddNodes(nodes));
            inputField.text = "";
            Close();
        });
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
