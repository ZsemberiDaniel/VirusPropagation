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

    private bool panelShown = false;
    
	void Start() {
        rectTransform = GetComponent<RectTransform>();
        // resize and reposition
        rectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight * 0.8f);
        rectTransform.anchoredPosition = new Vector2(0, rectTransform.sizeDelta.y / 2f);

        // dropdown click listener
        dropdownButtonRectTranform.GetComponent<Button>().onClick.AddListener(() => {
            DropdownClicked();
        });

        // parse button click listener
        parseButton.onClick.AddListener(() => {
            // InputParser.Parse(inputField.text);
        });
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
