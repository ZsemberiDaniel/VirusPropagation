﻿using UnityEngine;
using TMPro;

public class NodeName : MonoBehaviour {

    private NodeHandler nodeToFollow;

    private TMP_Text text;
    private TMP_Text Text {
        get {
            if (text == null) text = GetComponent<TextMeshProUGUI>();

            return text;
        }
    }

    private RectTransform rectTransform;
    private RectTransform RectTransf {
        get {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

            return rectTransform;
        }
    }
    public Vector3 Size {
        get { return rectTransform.sizeDelta; }
        set {
            rectTransform.sizeDelta = value;
            Text.RecalculateClipping();
        }
    }

    /// <summary>
    /// Set every frame
    /// </summary>
    private Vector3 offsetBy = new Vector3();
    
    public void Update() {
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(nodeToFollow.transform.position + offsetBy);

        UpdateText();
	}

    public void SetNodeToFollow(NodeHandler node) {
        nodeToFollow = node;

        UpdateText();

        offsetBy.x = nodeToFollow.Size.x / 2f;
        offsetBy.y = -nodeToFollow.Size.y * 0.1f;

        // Set here as well so it does not simply just jump there
        RectTransf.anchoredPosition = Camera.main.WorldToScreenPoint(nodeToFollow.transform.position + offsetBy);
    }

    private void UpdateText() {
        Text.text = nodeToFollow.name + "\n<size=60%>" + nodeToFollow.InfectedCount + "/" + nodeToFollow.RecoveredCount + "/" + nodeToFollow.HostCount;
        name = nodeToFollow.name + "Text";
    }
}
