using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class GraphClearButton : MonoBehaviour {

    private GraphHandler graphHandler;
    private GameState gameState;

    private RectTransform rectTransform;

    void Start() {
        graphHandler = FindObjectOfType<GraphHandler>();
        gameState = FindObjectOfType<GameState>();

        rectTransform = GetComponent<RectTransform>();

        GetComponent<Button>().onClick.AddListener(() => {
            graphHandler.ClearAll();
        });

        gameState.OnStateChange += (oldState, newState) => {
            if (newState == GameState.State.Simulating) {
                rectTransform.DOAnchorPosX(-rectTransform.rect.width * 1.2f, 0.3f);
            } else if (oldState == GameState.State.Simulating) {
                rectTransform.DOAnchorPosX(0, 0.3f);
            }
        };
    }
}
