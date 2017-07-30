using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StopButton : MonoBehaviour {

    private GraphHandler graphHandler;
    private GameState gameState;

    private RectTransform rectTransform;
    private Button button;
    
	void Start() {
        gameState = FindObjectOfType<GameState>();
        graphHandler = FindObjectOfType<GraphHandler>();

        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();

        rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width * 1.2f, rectTransform.anchoredPosition.y);

        button.onClick.AddListener(() => {
            if (gameState.gState == GameState.State.Simulating) {
                graphHandler.StopSimulation();
            }
        });

        gameState.OnStateChange += (oldState, newState) => {
            if (newState == GameState.State.Simulating) {
                rectTransform.DOAnchorPosX(0, 0.3f);
            } else if (oldState == GameState.State.Simulating) {
                rectTransform.DOAnchorPosX(-rectTransform.rect.width * 1.2f, 0.3f);
            }
        };
	}
}
