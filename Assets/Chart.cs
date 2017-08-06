using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(RectTransform))]
public class Chart : MonoBehaviour {

    private LineRenderer lineRenderer;
    private RectTransform rectTransform;

    private int dataCount {
        get { return lineRenderer.positionCount; }
        set { lineRenderer.positionCount = value; }
    }
    private int maxDataCount = 0;
    
	void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();

        ResetChart();
    }

    /// <summary>
    /// Adds a data entry to this chart
    /// </summary>
    public void AddData(float percent) {
        dataCount++;

        lineRenderer.SetPosition(dataCount - 1,
            new Vector3(
                rectTransform.rect.width * ((float) dataCount / maxDataCount),
                rectTransform.rect.height * (percent)
            )
        );
    }

    /// <summary>
    /// Sets at what data count the width of the chart should be 100%
    /// </summary>
    public void SetMaxDataCount(int count) {
        for (int i = 1; i < lineRenderer.positionCount; i++) {
            Vector3 pos = lineRenderer.GetPosition(i);
            pos.x = rectTransform.rect.width * i / maxDataCount;

            lineRenderer.SetPosition(i, pos);
        }

        maxDataCount = count;
    }

    /// <summary>
    /// Resets this chart's variables and linerenderer
    /// </summary>
    /// <param name="maxDataCount">If given the maxDataCount will be set to this and then you won't have to call SetMaxDataCount</param>
    public void ResetChart(int maxDataCount = 0) {
        this.maxDataCount = maxDataCount;
        dataCount = 0;
    }
}
