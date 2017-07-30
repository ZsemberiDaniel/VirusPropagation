using UnityEngine;
using DG.Tweening;

public class AddButton : MonoBehaviour {

    private RectTransform rectTranform;
    
	void Start() {
        rectTranform = GetComponent<RectTransform>();

        FindObjectOfType<GameState>().OnStateChange += (oldState, newState) => {
            if (newState == GameState.State.Simulating) {
                rectTranform.DOAnchorPosX(-rectTranform.rect.width * 1.2f, 0.3f);
            } else if (oldState == GameState.State.Simulating) {
                rectTranform.DOAnchorPosX(0, 0.3f);
            }
        };
	}
}
