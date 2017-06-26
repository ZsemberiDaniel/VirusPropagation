using UnityEngine;
using TMPro;
using DG.Tweening;

public class EditPanelHandler : MonoBehaviour {

    [SerializeField]
    private RectTransform nodeAttributePanel;

    private float time = 0.3f;

    public Sequence GetShowSequence() {
        return DOTween.Sequence()
            .OnStart(() => {
                nodeAttributePanel.anchoredPosition = new Vector2(0, 0);
            })
            .Append(nodeAttributePanel.DOAnchorPosX(-nodeAttributePanel.sizeDelta.x, time))
            .Pause();
    }
    public Sequence GetHideSequence() {
        return DOTween.Sequence()
            .OnStart(() => {
                nodeAttributePanel.anchoredPosition = new Vector2(-nodeAttributePanel.sizeDelta.x, 0);
            })
            .Append(nodeAttributePanel.DOAnchorPosX(0, time))
            .Pause();
    }
}