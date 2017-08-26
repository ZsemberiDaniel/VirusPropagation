using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(RectTransform))]
public class Chart : MonoBehaviour {

    private const float ZPos = -1f;

    private LineRenderer lineRenderer;
    private RectTransform rectTransform;

    private int dataCount {
        get { return lineRenderer.positionCount; }
        set { lineRenderer.positionCount = value; }
    }
    private int maxDataCount = 0;
    private float[] percents = new float[0];
    private float storedWidth, storedHeight;
    
	void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();

        ResetChart();
    }

    private void Start() {
        storedWidth = rectTransform.rect.width;
        storedHeight = rectTransform.rect.height;
    }

    private void Update() {
        // resize change detection
        if (rectTransform.rect.width != storedWidth || rectTransform.rect.height != storedHeight) {
            OnResize();

            storedWidth = rectTransform.rect.width;
            storedHeight = rectTransform.rect.height;
        }
    }

    /// <summary>
    /// Called when chart was resized
    /// </summary>
    private void OnResize() {
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition(i,
                new Vector3(
                    rectTransform.rect.width * ((float) i / maxDataCount),
                    rectTransform.rect.height * (percents[i]),
                    ZPos
                )
            );
        }
    }

    /// <summary>
    /// Adds a data entry to this chart
    /// </summary>
    public void AddData(float percent) {
        dataCount++;

        percents[dataCount - 1] = percent; 
        lineRenderer.SetPosition(dataCount - 1,
            new Vector3(
                rectTransform.rect.width * ((float) dataCount / maxDataCount),
                rectTransform.rect.height * (percent),
                ZPos
            )
        );
    }

    /// <summary>
    /// Sets at what data count the width of the chart should be 100%
    /// </summary>
    public void SetMaxDataCount(int count) {
        System.Array.Resize(ref percents, count);

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
        percents = new float[maxDataCount];
    }
}
