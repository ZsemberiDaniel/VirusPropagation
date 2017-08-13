using UnityEngine;

public class GameState : ScriptableObject {

    // The four variables of the virus
    private float s2i;
    public float S2I {
        get {
            return s2i;
        }
        set {
            s2i = value;
            OnVirusAttributeChange?.Invoke(s2i, i2r, s2r, packetSize);
        }
    }

    private float i2r;
    public float I2R {
        get {
            return i2r;
        }
        set {
            i2r = value;
            OnVirusAttributeChange?.Invoke(s2i, i2r, s2r, packetSize);
        }
    }

    private float s2r;
    public float S2R {
        get {
            return s2r;
        }
        set {
            s2r = value;
            OnVirusAttributeChange?.Invoke(s2i, i2r, s2r, packetSize);
        }
    }

    private int packetSize;
    public int PacketSize {
        get {
            return packetSize;
        }
        set {
            packetSize = value;
            OnVirusAttributeChange?.Invoke(s2i, i2r, s2r, packetSize);
        }
    }

    public delegate void VirusAttributeChange(float S2I, float I2R, float S2R, int packetSize);
    public event VirusAttributeChange OnVirusAttributeChange;

    public delegate void StateChange(State oldState, State newState);
    public event StateChange OnStateChange;
    private State state = State.Adding;
    public State gState {
        get { return state; }
        set {
            if (state != value) { 
                OnStateChange?.Invoke(state, value);
                state = value;
            }
        }
    }

    /// <summary>
    /// Toggle the add/edit mode
    /// </summary>
    public void ToggleAddEditMode() {
        if (gState == State.Adding) {
            gState = State.Editing;
        } else if (gState == State.Editing) {
            gState = State.Adding;
        }
    }

    public enum State {
        Adding, Editing, Simulating
    }
	
}
