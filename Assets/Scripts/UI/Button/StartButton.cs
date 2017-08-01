using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartButton : MonoBehaviour {

    [SerializeField]
    private string playText = "Play";
    [SerializeField]
    private Color playColor = Color.green;
    [SerializeField]
    private string pauseText = "Pause";
    [SerializeField]
    private Color pauseColor = Color.blue;
    [SerializeField]
    private string unpauseText = "Start";
    [SerializeField]
    private Color unpauseColor = Color.green;


    private GraphHandler graphHandler;
    private GameState gameState;

    private TMP_Text buttonText;
    private Image buttonImage;
    
	void Start() {
        graphHandler = FindObjectOfType<GraphHandler>();
        gameState = FindObjectOfType<GameState>();
        buttonText = GetComponentInChildren<TMP_Text>();
        buttonImage = GetComponent<Image>();

        buttonText.text = playText;
        buttonImage.color = playColor;

        GetComponent<Button>().onClick.AddListener(() => {
            if (gameState.gState == GameState.State.Simulating) {
                graphHandler.PauseSimulationToggle();

                if (graphHandler.SimulationPaused) {
                    buttonText.text = unpauseText;
                    buttonImage.color = unpauseColor;
                } else {
                    buttonText.text = pauseText;
                    buttonImage.color = pauseColor;
                }
            } else {
                graphHandler.StartSimulation();

                buttonText.text = pauseText;
                buttonImage.color = pauseColor;
            }
        });

        FindObjectOfType<GameState>().OnStateChange += (oldState, newState) => {
            // exit simulation -> reset
            if (oldState == GameState.State.Simulating) {
                buttonText.text = playText;
                buttonImage.color = playColor;
            }
        };
	}
}
