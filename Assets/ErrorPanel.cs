using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class ErrorPanel : MonoBehaviour {

    private static ErrorPanel instance;
    public static ErrorPanel Instance {
        get { return instance; }
    }

    private RectTransform rectTransform;

    [SerializeField]
    private TMP_Text errorText;

	private void Start() {
        // we somehow have a second error panel so delete it
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        // set singleton to this
        instance = this;

        // init
        rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(false);
	}

    public void ShowError(string error, float showForSecs = 3f) {
        // initial state
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -rectTransform.rect.height);
        gameObject.SetActive(true);
        errorText.text = error;

        DOTween.Sequence()
            .Append(rectTransform.DOAnchorPosY(0f, 0.3f))
            .AppendInterval(showForSecs)
            .Append(rectTransform.DOAnchorPosY(-rectTransform.rect.height, 0.3f));
    }
}
