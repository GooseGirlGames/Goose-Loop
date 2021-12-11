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
    private bool gooseEnabled = true;  // disabled Geese will be hidden and won't be updated

    /** References to commonly accessed behaviours. */
    public Movement Movement;
    public mouse_look MouseLook;

    // Sadly, renderers are not behaviours, so they can't be managed by the lists :(
    public GameObject viewModel;
    public MeshRenderer playerModelRenderer;
    public ExplodedFeathers feathers;

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
    public static int SPAWN_INVULNERABILITY_TICKS = 500;

    void Awake(){
        id = GooseController.count++;

        InitDebugMenuLines();

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

    private void OnGUI() {
        UpdateDebugMenuText();
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
        MouseLook.ProcessInputs(inputs);

        // Store positions, rotations etc.
        currentFrame.position = transform.position;
        currentFrame.eulerAngles = transform.rotation.eulerAngles;
        
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
        if (tick >= data.GetFrameCount()) {
            Debug.LogWarning(GenerateName() + " ran out of ticks to replay.");
            return;
        }

        // Perform movement
        var currentFrame = data.GetFrame(tick);
        Movement.ProcessInputs(currentFrame.inputs);
        MouseLook.ProcessInputs(currentFrame.inputs);

        bool invulnerable = tick < SPAWN_INVULNERABILITY_TICKS;
        if (invulnerable) {
            transform.position = currentFrame.position;
            transform.rotation = Quaternion.Euler(currentFrame.eulerAngles);
        }

        // check for broken movement is done in CheckForLoopBreak

        // TODO handle shots
        // TODO handle item interactions
        // TODO handle non-break zones


        // setup members for drawing debug gizmos
        _actual_pos = transform.position;
        _recorded_pos = currentFrame.position;
    }

    public bool LoopIsBroken() {

        int tick = gman.GetCurrentTick();
        if (tick >= data.GetFrameCount()) {
            Debug.Log(GenerateName() + " broke due to lack of data.");
            return true;
        }

        var frame = data.GetFrame(tick);
        var deltaPosition = transform.position - frame.position;

        if (frame.nonBreakZone.HasValue) {
            if (frame.nonBreakZone.Value.ignoreAxisY) {
                deltaPosition.y = 0;
            }
        }

        float error = deltaPosition.magnitude;

        _error = error;  // this member variable is only used for debug output

        bool broken = error > 0.5f;

        if (broken) {
            Debug.Log(GenerateName() + " broke due to position mismatch.");
        }

        return broken;
    }

    public bool LoopIsFixable() {
        feathers.Explode();
        SetGooseEnabled(false);
        return false;
    }

    public void ResetTransformToSpawn() {
        transform.position = gman.transform.position;
        transform.eulerAngles = gman.transform.eulerAngles;
    }

    public void SetGooseEnabled(bool gooseEnabled) {  // TODO rename to hide?
        this.gooseEnabled = gooseEnabled;
        if (!gooseEnabled) {
            SetState(GooseState.GHOOSTLING);
            viewModel.SetActive(false);
            playerModelRenderer.enabled = false;
        } else {
            viewModel.SetActive(state == GooseState.ACTIVE);
            playerModelRenderer.enabled = (state != GooseState.ACTIVE);
        }
    }
    public bool IsGooseEnabled() {
        return gooseEnabled;
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

        Debug.Log("Goose #" + GetId() + " changed state to " + state.ToString());
    }

    public GooseState GetState() {
        return state;
    }

    // On Screen Debug stuff
    private int _debug_line;
    private float _error;
    private Vector3 _actual_pos;
    private Vector3 _recorded_pos;
    private void InitDebugMenuLines() {
        var debug = DebugMenu.GetInstance();
        _debug_line = debug.RegisterLine();
    }
    private void UpdateDebugMenuText() {
        var debug = DebugMenu.GetInstance();
        string errorString = _error.ToString("F5");
        if (state != GooseState.GHOOSTLING) {
            errorString = "N/A";
        }
        debug.UpdateLine(_debug_line, "G" + GetId() + "  breakError: " + errorString +
         "  frame count: " + data.GetFrameCount());
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_actual_pos, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_recorded_pos, 1f);
    }
}
