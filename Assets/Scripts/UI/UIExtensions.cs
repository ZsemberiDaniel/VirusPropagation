using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public static class UIExtensions {

    private static Color errorColor = new Color(0.95686f, 0.26275f, 0.21176f, 0.89f);

    public static void DisplayError(this Image image) {
        Color startColor = image.color;

        DOTween.Sequence()
            .Append(DOTween.To(() => image.color, (color) => image.color = color, errorColor, 0.1f))
            .AppendInterval(0.06f)
            .Append(DOTween.To(() => image.color, (color) => image.color = color, startColor, 0.1f))
            .SetLoops(2)
            .OnComplete(() => { image.color = startColor; });
    }
	
}
