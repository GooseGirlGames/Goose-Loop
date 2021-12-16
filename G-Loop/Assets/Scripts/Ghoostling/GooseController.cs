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
    public Shoot Shoot;
    public Interact Interact;

    // Sadly, renderers are not behaviours, so they can't be managed by the lists :(
    public GameObject viewModel;
    public SkinnedMeshRenderer playerModelRenderer;
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
    public static int SPAWN_INVULNERABILITY_TICKS = 100;
    private int invulnerable = 0; 
    private bool? loopBeakFixable = null;
    public LayerMask bulletLayer;
    public const float bulletCheckRadius = 0.27f;
    public const float bulletCheckHeightThingy = 0.16f;

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

    // Return id of killer goose, -1 if not hit occurs
    private int GettingHitByBullet() {
        var bullet = Physics.OverlapCapsule(
            point0: Movement.groundCheck.position + Vector3.up * bulletCheckHeightThingy,
            point1: MouseLook.transform.position - Vector3.up * bulletCheckHeightThingy,
            radius: bulletCheckRadius,
            layerMask: bulletLayer);
        if (bullet.Length == 0) {
            return -1;
        } else {
            foreach (var b in bullet) {
                var bu = b.gameObject.GetComponent<Bullet>();
                if (bu) {
                    return bu.shooter;
                }
            }
            Debug.LogWarning("Couldn't find Bullet.  Is it missing the `Bullet` script?");
            return -1;
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
        GhoostlingData.UserInputs inputs;
        if (gman.IsFastForwarding() || gman.IsPaused()) {
            inputs = new GhoostlingData.UserInputs();  // empty set of inputs
        } else {
            inputs = new GhoostlingData.UserInputs(GhoostlingData.UserInputs.READ_USER_INPUTS);
        }
        currentFrame.inputs = inputs;
        Movement.ProcessInputs(inputs);
        MouseLook.ProcessInputs(inputs);
        Shoot.ProcessInputs(inputs);
        // TODO uncomment Interact.ProcessInputs(inputs);

        // Store positions, rotations etc.
        currentFrame.position = transform.position;
        currentFrame.eulerAngles = transform.rotation.eulerAngles;

        int killer = GettingHitByBullet();
        if (killer != -1 && killer != GetId()) {
            var death = new GhoostlingData.Death();
            death.cause = GhoostlingData.Death.Cause.EXTERNAL;
            death.killer = killer;
            currentFrame.death = death;
            Die();
            Debug.Log(
                "t=" + gman.GetCurrentTick() +
                "Active goose " + GenerateName() + " Died (killed by " + killer + ")");
            data.AddFrame(currentFrame);
            gman.EndLoop();  // hope this won't break anything
            return;
        }

        // TODO handle item interactions
        currentFrame.itemInteraction = null;
        // TODO handle non-break zones
        currentFrame.nonBreakZone = null;

        // Store frame in recording
        data.AddFrame(currentFrame);
    }

    public void MakeInvulnerable(int time_span){
        invulnerable = time_span;
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
        Shoot.ProcessInputs(currentFrame.inputs);
        // TODO uncomment Interact.ProcessInputs(currentFrame.inputs);

        if (currentFrame.nonBreakZone.HasValue) {
            if (currentFrame.nonBreakZone.Value.ignoreAxisY) {
                currentFrame.position.y = transform.position.y;
                data.AddFrame(currentFrame);  // update
            }
        }

        if(gman.GetCurrentTick() == 0){
            MakeInvulnerable(SPAWN_INVULNERABILITY_TICKS);
        }

        //bool invulnerable = tick < SPAWN_INVULNERABILITY_TICKS;
        if (invulnerable > 0) {
            ForceTransformToRecorded();
            invulnerable--;
        }

        bool externalDeathRecorded = false;
        if (currentFrame.death is GhoostlingData.Death d) {
            if (d.cause == GhoostlingData.Death.Cause.EXTERNAL) {
                externalDeathRecorded = true;
            } else {
                Debug.LogError("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa");
            }
        }
        bool hitByBullet = GettingHitByBullet() != -1;
        if (externalDeathRecorded && hitByBullet) {
            Die();  // All good
        } else if (externalDeathRecorded && !hitByBullet) {
            Debug.Log("Expecting loop break, goose was un-killed");
        } else if (!externalDeathRecorded && hitByBullet) {
            gman.DebugLog("Ghoostling killed.  updating data");
            var death = new GhoostlingData.Death();
            death.cause = GhoostlingData.Death.Cause.EXTERNAL;
            currentFrame.death = death;
            data.AddFrame(currentFrame);  // will update to include death
            Die();
        }

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
            loopBeakFixable = true;
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

        bool positionBroken = error > 0.5f;

        if (positionBroken) {
            loopBeakFixable = false;
            Debug.Log(GenerateName() + " broke due to position mismatch.");
            return true;
        } else {
            loopBeakFixable = null;
        }
        
        bool externalDeathRecorded = false;
        if (frame.death is GhoostlingData.Death d) {
            if (d.cause == GhoostlingData.Death.Cause.EXTERNAL) {
                Debug.Log(
                    "(t=" + gman.GetCurrentTick() + "): " +
                    " Death of " + GenerateName() + " recorded."
                );
                externalDeathRecorded = true;
            }
        }
        bool hitByBullet = GettingHitByBullet() != -1;
        if (!externalDeathRecorded && hitByBullet) {
            // Killed
            Debug.Log(
                "(t=" + gman.GetCurrentTick() + "): " +
                GenerateName() + " was killed, which broke the loop.");
            loopBeakFixable = false;
            return true;
        } else if (externalDeathRecorded && !hitByBullet) {
            // Saved
            Debug.Log(
                "(t=" + gman.GetCurrentTick() + "): " +
                GenerateName() + " was un-killed.  Unless lack of data this is ok");
            loopBeakFixable = null;
            return false;
        }
        return false;
    }

    public void ForceTransformToRecorded() {
        int tick = gman.GetCurrentTick();
        var frame = data.GetFrame(tick);
        transform.position = frame.position;
        transform.rotation = Quaternion.Euler(frame.eulerAngles);
    }

    public bool LoopIsFixable() {
        if (loopBeakFixable is bool fixable) {
            if (fixable) {
                return true;
            } else {
                Debug.Log("Exploding due to unfixable loop break.");
                feathers.Explode();
                SetGooseEnabled(false);
                return false;
            }
        } else {
            Debug.LogWarning("LoopIsFixable called even though loop it not broken.");
            return false;
        }
    }

    public void Die() {
        feathers.Explode();
        SetGooseEnabled(false);
    }

    // Called after tick is set to zero
    public void Goose_Reset() {
        ResetTransformToSpawn();
        Shoot.Goose_Reset();
    }

    private void ResetTransformToSpawn() {
        transform.position = gman.GetSpawnLocation().Item1;
        transform.rotation = gman.GetSpawnLocation().Item2;
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

    public float GetError() {
        return _error;
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
        Gizmos.DrawSphere(_actual_pos, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_recorded_pos, 0.3f);
        
        /*
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(
            Movement.groundCheck.position + Vector3.up * bulletCheckHeightThingy,
            bulletCheckRadius
        );
        Gizmos.DrawSphere(
            MouseLook.transform.position - Vector3.up * bulletCheckHeightThingy,
            bulletCheckRadius
        );
        */
    }
}
