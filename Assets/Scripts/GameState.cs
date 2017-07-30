using UnityEngine;

public class GameState : ScriptableObject {

    // The four variables of the virus
    [Range(0.01f, 1f)]
    public float S2I = 0f;
    [Range(0.01f, 1f)]
    public float I2R = 0f;
    [Range(0.01f, 1f)]
    public float S2R = 0f;
    public int packetSize = 0;

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
