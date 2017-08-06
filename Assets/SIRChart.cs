using System.Collections;
using UnityEngine;

public class SIRChart : MonoBehaviour {

    [SerializeField]
    private Chart infectedChart;
    [SerializeField]
    private Chart recoveredChart;
    [SerializeField]
    private Chart normalChart;

    // These are set in ResetChart
    int dataCount;
    int maxDataCount;

    private Coroutine extendCoroutine;
    private bool extendCoroutineRunning = false;
    
	void Start() {
        ResetChart();
    }

    public void AddData(float infectedPercent, float recoveredPercent, float normalPercent) {
        dataCount++;

        if (dataCount > maxDataCount) {
            // if for some reason the coroutine is running then stop it
            if (extendCoroutineRunning) {
                StopCoroutine(extendCoroutine);
                extendCoroutineRunning = false;
            }
            ExtendMaxDataCountImmediatly(maxDataCount + 20);
            extendCoroutine = StartCoroutine(ExtendMaxDataCount(maxDataCount * 2, maxDataCount / 20));
        }

        infectedChart.AddData(infectedPercent);
        recoveredChart.AddData(recoveredPercent);
        normalChart.AddData(normalPercent);
    }

    /// <summary>
    /// Extends the data count immediatly to the given count
    /// </summary>
    private void ExtendMaxDataCountImmediatly(int count) {
        maxDataCount = count;

        infectedChart.SetMaxDataCount(maxDataCount);
        recoveredChart.SetMaxDataCount(maxDataCount);
        normalChart.SetMaxDataCount(maxDataCount);
    }
    /// <summary>
    /// Extends maximum data count gradually
    /// </summary>
    /// <param name="count">To what count it should be extended</param>
    /// <param name="step">In what steps it should be extended</param>
    private IEnumerator ExtendMaxDataCount(int count, int step) {
        extendCoroutineRunning = true;

        while (maxDataCount < count) {
            ExtendMaxDataCountImmediatly(maxDataCount + step);

            yield return null;
        }

        extendCoroutineRunning = false;
    }

    /// <summary>
    /// Resets this SIRChart's variables and linerenderers
    /// </summary>
    public void ResetChart() {
        dataCount = 0;
        maxDataCount = 32;

        infectedChart.ResetChart(maxDataCount);
        recoveredChart.ResetChart(maxDataCount);
        normalChart.ResetChart(maxDataCount);
    }
}
