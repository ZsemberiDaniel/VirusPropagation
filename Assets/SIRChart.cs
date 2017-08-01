using UnityEngine;

public class SIRChart : MonoBehaviour {

    private LineRenderer lineRenderer;

    int dataCount = 0;
    int maxDataCount = 512;
    
	void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, new Vector3(Camera.main.pixelWidth / 2f * 0.5f, Camera.main.pixelHeight / 2f * 0.5f));
	}

    public void AddData(float infectedPercent) {
        dataCount++;

        if (dataCount > maxDataCount) return;
        lineRenderer.positionCount = dataCount + 1;

        lineRenderer.SetPosition(dataCount,
            new Vector3(
                Camera.main.pixelWidth / 2f * (0.5f + (dataCount * 0.5f / maxDataCount)),
                Camera.main.pixelHeight / 2f * (0.5f + infectedPercent * 0.5f)
            )
        );

    }

    /// <summary>
    /// Asserts that there is enough positions in the line renderer for one more.
    /// If not it doubles the positions count.
    /// </summary>
    private void AssertPositionCount() {
        if (dataCount >= lineRenderer.positionCount)
            lineRenderer.positionCount *= 2;
    }
}
