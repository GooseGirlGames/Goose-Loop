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
    public mouse_look MouseLook;

    // Sadly, renderers are not behaviours, so they can't be managed by the lists :(
    public GameObject viewModel;
    public MeshRenderer playerModelRenderer;

    /** All behaviours that should be active in the state.  Upon state
     * change, behaviours contained in other states' lists (but not in the list for the new active
     * state) will be disabled.
     */
    public List<Behaviour> BehavioursWhileActive = new List<Behaviour>();
    public List<Behaviour> BehavioursWhileGhoostling = new List<Behaviour>();
    public List<Behaviour> BehavioursWhileRagdoll = new List<Behaviour>();

    // Map state to list, for convinience
    private Dictionary<GooseState, List<Behaviour>> behaviours;

    private GooseState state;
    private GhoostlingData data;
    private GhoostlingManager gman;
    public bool EnableDebugStateChangeKeys = false;

    void Awake(){
        id = GooseController.count++;
        data = new GhoostlingData();
        behaviours = new Dictionary<GooseState, List<Behaviour>> {
            {GooseState.ACTIVE, BehavioursWhileActive},
            {GooseState.GHOOSTLING, BehavioursWhileGhoostling},
            {GooseState.RAGDOLL, BehavioursWhileRagdoll},
        };
        gman = GhoostlingManager.GetInstance();

        if (gameObject.name != GenerateName()) {  // initial goose needs setup
            SetState(GooseState.ACTIVE);
        }
        gameObject.name = GenerateName();

        gman.RegisterGoose(this);
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
    private void FixedUpdate() {
        // Updates are called by GhoostlingManager
    }

    public void Goose_FixedUpdate() {
        switch(state) {
            case (GooseState.ACTIVE):
                FixedUpdateActive();
                break;
            case (GooseState.GHOOSTLING):
                FixedUpdateGhoostling();
                break;
        }
    }

    /** Fixed update for active player:  Execute actions and store frame. */
    private void FixedUpdateActive() {

        // create frame, store metadata
        GhoostlingData.Frame currentFrame = new GhoostlingData.Frame();
        currentFrame.tick = gman.GetCurrentTick();

        // Collect inputs and act upon them
        var inputs = new GhoostlingData.UserInputs(GhoostlingData.UserInputs.READ_USER_INPUTS);
        currentFrame.inputs = inputs;
        Movement.ProcessInputs(inputs);

        // Store positions, rotations etc.
        currentFrame.position = transform.position;
        currentFrame.eulerAngles = transform.rotation.eulerAngles;
        currentFrame.cameraPitch = MouseLook.xRotation;

        // TODO handle shots
        currentFrame.shotFired = null;
        // TODO handle item interactions
        currentFrame.itemInteraction = null;
        // TODO handle non-break zones
        currentFrame.nonBreakZone = null;

        // Store frame in recording
        data.AddFrame(currentFrame);
    }

    /** Fixed update for ghoostling.  Play frame. */
    private void FixedUpdateGhoostling() {
        int tick = gman.GetCurrentTick();
        if (tick > data.GetFrameCount()) {
            Debug.LogWarning(GenerateName() + " ran out of ticks to replay.");
            return;
        }

        // Perform movement
        var currentFrame = data.GetFrame(tick);
        Movement.ProcessInputs(currentFrame.inputs);
        // TODO check if movement is broken

        // Restore rotations
        transform.rotation = Quaternion.Euler(currentFrame.eulerAngles);
        MouseLook.xRotation = currentFrame.cameraPitch;
        // TODO handle shots
        // TODO handle item interactions
        // TODO handle non-break zones
    }

    public void ResetTransformToSpawn() {
        transform.position = gman.transform.position;
        transform.eulerAngles = gman.transform.eulerAngles;
    }

    public int GetId() {
        return id;
    }

    public string GenerateName() {
        return "Goose #" + GetId() + " (" + GetState().ToString() + ")";
    }

    public void SetState(GooseState state) {
        this.state = state;
        var behaviours_for_state = behaviours[state];
        foreach (KeyValuePair<GooseState, List<Behaviour>> e in behaviours) {
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

        viewModel.SetActive(state == GooseState.ACTIVE);
        playerModelRenderer.enabled = (state != GooseState.ACTIVE);
    }

    public GooseState GetState() {
        return state;
    }

}
