using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChartLines : MonoBehaviour {

    [SerializeField]
    private Image line0;
    [SerializeField]
    private Image line1;

    [Range(0.05f, 0.5f)]
    [SerializeField]
    private float divide = 0.2f;

    [SerializeField]
    private TMP_Text textForLines;

	void Start() {
        float linesAllHeight = line1.rectTransform.anchorMax.y - line0.rectTransform.anchorMin.y;
        float lineHeight = line0.rectTransform.anchorMax.y - line0.rectTransform.anchorMin.y;

		for (float i = divide, k = 1; i < 1; i += divide, k++) {
            Image img = Instantiate(line0, transform, true);

            img.gameObject.name = i.ToString("0.##");
            img.transform.SetParent(transform);

            img.rectTransform.anchorMin = line0.rectTransform.anchorMin + new Vector2(0, lineHeight * k + linesAllHeight * i);
            img.rectTransform.anchorMax = line0.rectTransform.anchorMax + new Vector2(0, lineHeight * k + linesAllHeight * i);

            img.rectTransform.offsetMin = new Vector2();
            img.rectTransform.offsetMax = new Vector2();

            img.rectTransform.localScale = new Vector3(1, 1, 1);
            img.rectTransform.anchoredPosition3D = new Vector3(img.rectTransform.anchoredPosition3D.x,
                img.rectTransform.anchoredPosition3D.y, 0);

            if (textForLines != null) {
                TMP_Text text = Instantiate(textForLines, transform, true);

                text.gameObject.name = "Static" + i.ToString("0.##");

                text.text = i.ToString("0.##"); ;
                
                text.rectTransform.anchorMin = new Vector2(0, i);
                text.rectTransform.anchorMax = new Vector2(0, i);
            }
        }
	}
}
