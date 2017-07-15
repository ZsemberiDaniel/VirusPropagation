using UnityEngine;
using DG.Tweening;

public class EditPanel : MonoBehaviour {

    [SerializeField]
    private RectTransform nodeAttributePanel;
    private NodeAttributePanel nodeAttributePanelHandler;
    public NodeAttributePanel NodeAttributePanel { get { return nodeAttributePanelHandler; } }

    [SerializeField]
    private RectTransform connectionAttributePanel;
    private ConnectionAttributePanel connectionAttributePanelHandler;
    public ConnectionAttributePanel ConnectionAttributePanel { get { return connectionAttributePanelHandler; } }

    private AttributePanelType attributePanelType = AttributePanelType.node;
    private bool shown = false;

    private float hiddenPositionX;
    private float shownPositionX;

    private float time = 0.3f;

    private void Start() {
        // set the panels' sizes
        nodeAttributePanel.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, Camera.main.pixelHeight);
        connectionAttributePanel.sizeDelta = new Vector2(Camera.main.pixelWidth * 0.25f, Camera.main.pixelHeight);

        nodeAttributePanelHandler = nodeAttributePanel.GetComponent<NodeAttributePanel>();
        connectionAttributePanelHandler = connectionAttributePanel.GetComponent<ConnectionAttributePanel>();

        hiddenPositionX = 0f;
        shownPositionX = -nodeAttributePanel.sizeDelta.x;

        SwitchTo(AttributePanelType.node);
    }

    public void ShowPanel() {
        if (shown) return;
        shown = true;

        RectTransform curr = GetRectTransformFor(attributePanelType);

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

        this.attributePanelType = type;

        // deactivate all
        nodeAttributePanel.gameObject.SetActive(false);
        connectionAttributePanel.gameObject.SetActive(false);

        // Activate the one we want
        RectTransform curr = GetRectTransformFor(type);
        curr.gameObject.SetActive(true);
        curr.anchoredPosition = new Vector2(shown ? shownPositionX : hiddenPositionX, 0);
    }

    private RectTransform GetRectTransformFor(AttributePanelType type) {
        switch (attributePanelType) {
            case AttributePanelType.node: return nodeAttributePanel;
            case AttributePanelType.connection: return connectionAttributePanel;
            default: return null;
        }
    }

    public enum AttributePanelType {
        node, connection
    }
}