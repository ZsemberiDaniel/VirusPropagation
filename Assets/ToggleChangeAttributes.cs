using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(Image))]
public class ToggleChangeAttributes : MonoBehaviour {

    [SerializeField]
    private Color toggleColor;
    private Color normalColor;

    [SerializeField]
    private string toggleText;
    private string normalText;

    private Toggle toggle;
    private Image image;
    private TextMeshProUGUI text;
    
	void Start () {
        toggle = GetComponent<Toggle>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        // if at first it's not on then it has the normal colors
        if (!toggle.isOn) {
            normalColor = image.color;
            normalText = text.text;

            // If the user hasn't set a toggle text
            if (toggleText == "") toggleText = normalText;
        } else { // if it's on we need to swap the variables as well
            normalColor = new Color(toggleColor.r, toggleColor.g, toggleColor.b, toggleColor.a);

            // If the user hasn't set a toggle text
            if (toggleText != "") normalText = toggleText;
            else normalText = text.text;

            toggleColor = image.color;
            toggleText = text.text;
        }

        // Add listener that changes the attributes
        toggle.onValueChanged.AddListener((toggled) => {
            if (toggled) {
                image.color = toggleColor;
                text.text = toggleText;
            } else {
                image.color = normalColor;
                text.text = normalText;
            }
        });
	}
}
