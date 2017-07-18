using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InputInfoPanel : MonoBehaviour {

    [SerializeField]
    private Button infoButton;
    [SerializeField]
    private Button closeButton;

    private CanvasGroup infoPanelGroup;

	void Start() {
        infoPanelGroup = GetComponent<CanvasGroup>();

        gameObject.SetActive(false);

        infoButton.onClick.AddListener(() => {
            DOTween.Sequence()
                .OnStart(() => {
                    gameObject.SetActive(true);
                    infoPanelGroup.alpha = 0f;
                })
                .Append(infoPanelGroup.DOFade(1f, 0.2f));
        });

        closeButton.onClick.AddListener(() => {
            DOTween.Sequence()
                .OnStart(() => {
                    gameObject.SetActive(false);
                    infoPanelGroup.alpha = 1f;
                })
                .Append(infoPanelGroup.DOFade(0f, 0.2f));
        });
	}
}
