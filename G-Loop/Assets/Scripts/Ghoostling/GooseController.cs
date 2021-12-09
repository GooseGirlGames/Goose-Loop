using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Holds state and manages activation/deactivation of specific behaviours in each state.
 */
public class GooseController : MonoBehaviour {
    /** Possible states a goose can be in. */
    public enum GooseState {
        ACTIVE,
        GHOOSTLING,
        RAGDOLL,
    }

    private int id;
    private static int count = 0;  // used to generate id

    /** References to commonly accessed behaviours. */
    public Movement Movement;

    /** All behaviours that should be active in the state.  Upon state
     * change, behaviours contained in other states' lists (but not in the list for the new active
     * state) will be disabled.
     */
    public List<MonoBehaviour> BehavioursWhileActive = new List<MonoBehaviour>();
    public List<MonoBehaviour> BehavioursWhileGhoostling = new List<MonoBehaviour>();
    public List<MonoBehaviour> BehavioursWhileRagdoll = new List<MonoBehaviour>();

    // Map state to list, for convinience
    private Dictionary<GooseState, List<MonoBehaviour>> behaviours;

    private GooseState state;
    public bool EnableDebugStateChangeKeys = false;

    void Awake(){
        id = GooseController.count++;
        behaviours = new Dictionary<GooseState, List<MonoBehaviour>> {
            {GooseState.ACTIVE, BehavioursWhileActive},
            {GooseState.GHOOSTLING, BehavioursWhileGhoostling},
            {GooseState.RAGDOLL, BehavioursWhileRagdoll},
        };
    }

    private void Update() {
        if (EnableDebugStateChangeKeys) {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetState(GooseState.ACTIVE);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SetState(GooseState.GHOOSTLING);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SetState(GooseState.RAGDOLL);
        }
    }

    public int GetId() {
        return id;
    }

    public string GenerateName() {
        return GetState().ToString() + " #" + GetId();
    }

    public void SetState(GooseState state) {
        this.state = state;
        var behaviours_for_state = behaviours[state];
        foreach(KeyValuePair<GooseState, List<MonoBehaviour>> e in behaviours) {
            if (state != e.Key) {
                foreach(var behaviour in e.Value) {
                    if (!behaviours_for_state.Contains(behaviour)) {
                        behaviour.enabled = false;
                    }
                }
            }
        }
        foreach (var behaviour in behaviours_for_state) {
            behaviour.enabled = true;
        }

        Movement.AcceptPlayerInput = (state == GooseState.ACTIVE);

        gameObject.name = GenerateName();
    }

    public GooseState GetState() {
        return state;
    }

}
