using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour {

    private const float animTime = 0.3f;

    //inputs
    [SerializeField]
    private Button settingsButton;
    private bool open = false;

    [SerializeField]
    private Toggle nodeToggle;
    [SerializeField]
    private Toggle connectionToggle;

    [SerializeField]
    private TMP_Text simulationSpeedText;
    [SerializeField]
    private Scrollbar simulationSpeedScrollbar;
    float[] speedForScrollbarSteps = new float[] { 0.25f, 0.5f, 1f, 1.5f, 2f, 3f, 5f, 10f };

    // own stuff
    private RectTransform rectTransform;
    
	void Start() {
        rectTransform = GetComponent<RectTransform>();

        // set positions because unity doesn't want to set it correctly
        rectTransform.anchoredPosition = new Vector2(-rectTransform.sizeDelta.x / 2f, rectTransform.anchoredPosition.y);

        var settingsButtonRectTransform = settingsButton.GetComponent<RectTransform>();
        settingsButtonRectTransform.anchoredPosition = new Vector2(settingsButtonRectTransform.sizeDelta.x / 2f, settingsButtonRectTransform.anchoredPosition.y);

        // listeners
        settingsButton.onClick.AddListener(() => {
            ToggleSettingsMenu();
        });

        nodeToggle.onValueChanged.AddListener((value) => {
            Settings.Instance.ShowNodeLabel = value;
        });

        connectionToggle.onValueChanged.AddListener((value) => {
            Settings.Instance.ShowConnectionLabel = value;
        });

        simulationSpeedScrollbar.onValueChanged.AddListener((value) => {
            // we subtract 0.001f so at 1 we don't get 6 (we get 5 instead)
            int at = (int) ((value - 0.001f) * simulationSpeedScrollbar.numberOfSteps);
            Settings.Instance.SimulationSpeed = speedForScrollbarSteps[at];
            simulationSpeedText.text = ">> " + Settings.Instance.SimulationSpeed;
        });
	}

    void Update() {
        if (open && Input.GetKeyDown(KeyCode.Escape)) {
            CloseSettingMenu();
        }
    }

    private void ToggleSettingsMenu() {
        if (open)
            CloseSettingMenu();
        else
            OpenSettingMenu();
    }

	private void OpenSettingMenu() {
        if (open) return;
        open = true;

        rectTransform.DOAnchorPosX(rectTransform.sizeDelta.x / 2f, animTime)
            .OnStart(() => {
                rectTransform.anchoredPosition = new Vector2(-rectTransform.sizeDelta.x / 2f, rectTransform.anchoredPosition.y);
            });
    }

    private void CloseSettingMenu() {
        if (!open) return;
        open = false;

        rectTransform.DOAnchorPosX(-rectTransform.sizeDelta.x / 2f, animTime)
            .OnStart(() => {
                rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2f, rectTransform.anchoredPosition.y);
            });
    }
}
