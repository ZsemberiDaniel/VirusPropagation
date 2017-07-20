using UnityEngine;

public class Settings : ScriptableObject {

    private static Settings _instance;
    public static Settings Instance {
        get {
            if (!_instance) {

            }

            return _instance;
        }
    }

    public delegate void SettingChanged(bool changedValue);

    [SerializeField]
    private bool showNodeLabel = true;
    public event SettingChanged onShowNodeLabelChanged;
    public bool ShowNodeLabel {
        get { return showNodeLabel; }
        set {
            ShowNodeLabel = value;
            onShowNodeLabelChanged?.Invoke(value);
        }
    }

    [SerializeField]
    private bool showConnectionLabel = true;
    public event SettingChanged onShowConnectionLabelChanged;
    public bool ShowConnectionLabel {
        get { return showConnectionLabel; }
        set {
            showConnectionLabel = value;
            onShowConnectionLabelChanged?.Invoke(value);
        }
    }
	
}
