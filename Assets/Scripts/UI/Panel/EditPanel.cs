using UnityEngine;
using DG.Tweening;

public class EditPanel : MonoBehaviour {

    private GameState gameState;

    // the different panels
    [SerializeField]
    private RectTransform nodeAttributePanel;
    private NodeAttributePanel nodeAttributePanelHandler;
    public NodeAttributePanel NodeAttributePanel { get { return nodeAttributePanelHandler; } }

    [SerializeField]
    private RectTransform connectionAttributePanel;
    private ConnectionAttributePanel connectionAttributePanelHandler;
    public ConnectionAttributePanel ConnectionAttributePanel { get { return connectionAttributePanelHandler; } }

    [SerializeField]
    private RectTransform noneSelectedPanel;
    private NoneSelectedPanel noneSelectedPanelHandler;
    public NoneSelectedPanel NoneSelectedPanel { get { return noneSelectedPanelHandler; } }

    // which panel is shown if it is shown
    private AttributePanelType attributePanelType = AttributePanelType.connection;
    private bool shown = false;

    // animation stuff
    private float hiddenPositionX;
    private float shownPositionX;

    private float time = 0.3f;

    private void Start() {
        gameState = FindObjectOfType<GameState>();
        gameState.OnStateChange += (oldState, newState) => {
            switch (newState) {
                case GameState.State.Simulating:
                case GameState.State.Adding:
                    HidePanel();
                    break;
                case GameState.State.Editing:
                    ShowPanel();
                    break;
            }
        };

        // set the panels' sizes
        nodeAttributePanel.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, nodeAttributePanel.sizeDelta.y);
        connectionAttributePanel.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, connectionAttributePanel.sizeDelta.y);
        noneSelectedPanel.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, noneSelectedPanel.sizeDelta.y);

        nodeAttributePanelHandler = nodeAttributePanel.GetComponent<NodeAttributePanel>();
        connectionAttributePanelHandler = connectionAttributePanel.GetComponent<ConnectionAttributePanel>();
        noneSelectedPanelHandler = noneSelectedPanel.GetComponent<NoneSelectedPanel>();

        hiddenPositionX = 0f;
        shownPositionX = -nodeAttributePanel.sizeDelta.x;

        SwitchTo(AttributePanelType.noneselected);
    }

    public void ShowPanel() {
        if (shown) return;
        shown = true;

        RectTransform curr = GetRectTransformFor(attributePanelType);
        AttributePanel panel = GetAttrPanelFor(attributePanelType);

        DOTween.Sequence()
            .OnStart(() => {
                curr.anchoredPosition = new Vector2(hiddenPositionX, 0);
            })
            .Append(curr.DOAnchorPosX(shownPositionX, time));
    }
    public void HidePanel() {
        if (!shown) return;
        shown = false;

        RectTransform curr = GetRectTransformFor(attributePanelType);

        DOTween.Sequence()
            .OnStart(() => {
                curr.anchoredPosition = new Vector2(shownPositionX, 0);
            })
            .Append(curr.DOAnchorPosX(hiddenPositionX, time));
    }

    public void SwitchTo(AttributePanelType type) {
        if (type == attributePanelType) return;

        attributePanelType = type;

        // deactivate all
        nodeAttributePanel.gameObject.SetActive(false);
        connectionAttributePanel.gameObject.SetActive(false);
        noneSelectedPanel.gameObject.SetActive(false);

        // Activate the one we want
        RectTransform curr = GetRectTransformFor(type);
        curr.gameObject.SetActive(true);
        curr.anchoredPosition = new Vector2(shown ? shownPositionX : hiddenPositionX, 0);
    }

    private RectTransform GetRectTransformFor(AttributePanelType type) {
        switch (attributePanelType) {
            case AttributePanelType.node: return nodeAttributePanel;
            case AttributePanelType.connection: return connectionAttributePanel;
            case AttributePanelType.noneselected: return noneSelectedPanel;
            default: return null;
        }
    }
    private AttributePanel GetAttrPanelFor(AttributePanelType type) {
        switch (type) {
            case AttributePanelType.connection: return connectionAttributePanelHandler;
            case AttributePanelType.node: return nodeAttributePanelHandler;
            case AttributePanelType.noneselected: return noneSelectedPanelHandler;
            default: return null;
        }
    }

    public enum AttributePanelType {
        node, connection, noneselected
    }
}