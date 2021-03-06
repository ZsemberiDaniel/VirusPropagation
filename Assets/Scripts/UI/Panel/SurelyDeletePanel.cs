﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class SurelyDeletePanel : MonoBehaviour {

    private GameState gameState;

    private const float animTime = 0.3f;

    private RectTransform rectTransform;
    public RectTransform RectTransf {
        get { return rectTransform; }
    }

    public Vector3 Size {
        get { return rectTransform.rect.size; }
    }

    private bool shown = false;

    private Button yesButton;
    private Button noButton;

    /// <summary>
    /// When a button is pressed what to do. Only called once then set to null
    /// </summary>
    private UnityAction<bool> actionToPerform;
    /// <summary>
    /// When a button is pressed what to do. Only called once then set to null
    /// </summary>
    public UnityAction<bool> ActionToPerform {
        set { actionToPerform = value; }
    }

    void Start() {
        // gamestate changes
        gameState = FindObjectOfType<GameState>();
        gameState.OnStateChange += (oldState, newState) => {
            if (newState == GameState.State.Editing)
                HidePanel();
        };

        rectTransform = GetComponent<RectTransform>();

        yesButton = transform.Find("YesButton").GetComponent<Button>();
        noButton = transform.Find("NoButton").GetComponent<Button>();

        gameObject.SetActive(false);

        // When yes is clicked
        yesButton.onClick.AddListener(() => {
            if (actionToPerform != null) {
                actionToPerform(true);
                actionToPerform = null;
            }
            HidePanel();
        });

        // When no is clicked
        noButton.onClick.AddListener(() => {
            if (actionToPerform != null) {
                actionToPerform(false);
                actionToPerform = null;
            }
            HidePanel();
        });
    }

    /// <summary>
    /// Shows the panel at the given screen pos.
    /// </summary>
    public void ShowAt(Vector2 pos) {
        if (shown) return;

        shown = true;

        // Set pivot correctly
        RectTransf.pivot = new Vector2();
        // But if it won't fit in screen from top -> show it at bottom
        if (pos.y > Camera.main.pixelHeight - Size.y) {
            RectTransf.pivot = new Vector2(0, 1);
        }
        // And if it won't fit to the right -> show it at the left
        if (pos.x > Camera.main.pixelWidth - Size.x) {
            RectTransf.pivot = new Vector2(1, RectTransf.pivot.y);
            // for some reason we have to subtract the size as well jsut from the x
            // it may be because of the custom anchoring
            pos.x -= Size.x;
        }

        rectTransform.anchoredPosition = pos;

        rectTransform.localScale = new Vector3();
        rectTransform.DOScale(1f, animTime).OnStart(() => {
            gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// Shows the panel at the position with the given action
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="action"></param>
    public void ShowAtWithAction(Vector2 pos, UnityAction<bool> action) {
        if (shown) return;

        ActionToPerform = action;
        ShowAt(pos);
    }

    /// <summary>
    /// Hides the panel
    /// </summary>
    public void HidePanel() {
        if (!shown) return;

        shown = false;
        rectTransform.DOScale(0f, animTime).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// Hide the panel immediatly
    /// </summary>
    public void HideImmediatly() {
        if (!shown) return;

        shown = false;
        rectTransform.localScale = new Vector3();
        gameObject.SetActive(false);
    }
}
