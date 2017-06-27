using UnityEngine;
using DG.Tweening;

public class AddPanelHandler : MonoBehaviour {

    public Sequence GetShowSequence() {
        return DOTween.Sequence()
            .Pause();
    }
    public Sequence GetHideSequence() {
        return DOTween.Sequence()
            .Pause();
    }
}
