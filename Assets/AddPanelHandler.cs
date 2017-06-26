using UnityEngine;
using DG.Tweening;

public class AddPanelHandler : MonoBehaviour {

    private float time = 0.3f;

    public Sequence GetShowSequence() {
        return DOTween.Sequence()
            .Pause();
    }
    public Sequence GetHideSequence() {
        return DOTween.Sequence()
            .Pause();
    }
}
