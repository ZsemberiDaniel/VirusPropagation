using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class SetTextTo : MonoBehaviour {

    [SerializeField]
    private Scrollbar scrollbar;

    private TMP_Text text;
    
	void Start() {
        text = GetComponent<TMP_Text>();

        text.text = scrollbar.value.ToString("0.####");
        scrollbar.onValueChanged.AddListener(value => {
            text.text = value.ToString("0.####");
        });
	}
}
