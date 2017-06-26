using UnityEngine;

[CreateAssetMenu(fileName = "VirusAttribute", menuName = "VirusProp/VirusAttribute")]
public class VirusAttributes : ScriptableObject {

    [SerializeField]
    [Range(0f, 1f)]
    private float SToI = 0.01f;
    [HideInInspector]
    public float S2I { get { return SToI; } }

    [SerializeField]
    [Range(0f, 1f)]
    private float IToR = 0.01f;
    [HideInInspector]
    public float I2R { get { return IToR; } }

    [SerializeField]
    [Range(0f, 1f)]
    private float SToR = 0.015f;
    [HideInInspector]
    public float S2R { get { return SToR; } }

}
