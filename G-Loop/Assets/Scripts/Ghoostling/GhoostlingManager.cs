using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Exists once per Scene.  Transform position/rotation is Ghoosling spawn
 */
public class GhoostlingManager : MonoBehaviour {
    public const int PAUSE_TICKS = 300;
    public GameObject playerPrefab;
    public List<GameObject> physicObjects = new List<GameObject>();
    public int maximumTick = 3000;
    private List<Vector3> startPositions = new List<Vector3>();
    private List<Quaternion> startRotations = new List<Quaternion>();
    private List<GooseController> geese = new List<GooseController>();
    private int tick;
    private int pauseTicksRemaining;
    private struct LoopBreakStackFrame {
        public int tick;
        public int originGooseId;  // the goose we'll go back to when popping the frame
        public int brokenGooseId;
        public bool isFixed;
    }
    private Stack<LoopBreakStackFrame> loopBreaks = new Stack<LoopBreakStackFrame>();
    private const float FAST_FORWARD_SPEED_MANUAL = 2.5f;
    private const float FAST_FORWARD_SPEED_AUTO = 2f;
    private float fastForwardSpeed = FAST_FORWARD_SPEED_AUTO;
    private const int NOT_FAST_FORWARDING = -1;
    private (Vector3, Quaternion) gooseSpawn;

    private int fastForwardStopAt = NOT_FAST_FORWARDING;
    public void RegisterGoose(GooseController goose) {
        geese.Add(goose);
        Debug.Log("Registered " + goose.name + ".  Now managing " + geese.Count + " geese.");
    }

    private GooseController GetGoose(int id) {
        return geese[id];
    }

    void Start() {
        InitDebugMenuLines();
        foreach (var item in physicObjects) {
            startPositions.Add(item.transform.position);
            startRotations.Add(item.transform.rotation);
        }

        // Move GhoostlingManager to where goose is
        gooseSpawn.Item1 = playerPrefab.transform.position;
        gooseSpawn.Item2 = playerPrefab.transform.rotation;
    }

    // Get current scene's GhooslingManager
    public static GhoostlingManager GetInstance() {
        var managers = GameObject.FindObjectsOfType<GhoostlingManager>();
        if (managers.Length > 1) {
            Debug.LogWarning("There are multiple GhoostlingManager."
                    + "Please make sure only one exists per scene.");
        } else if (managers.Length == 0) {
            Debug.LogError("No GhoostlingManager is present."
                    + "Please make sure one exists in the scene.");
        }
        return managers[0];
    }

    public int GetCurrentTick() {
        return tick;
    }

    private void FixedUpdate() {
        if (pauseTicksRemaining > 0) {
            UpdateDebugMenuText();
            --pauseTicksRemaining;
            tick = 0;  // urgh
            return;
        }

        Time.timeScale = Input.GetKey(KeyCode.F) ? FAST_FORWARD_SPEED_MANUAL : 1;

        if (fastForwardStopAt != NOT_FAST_FORWARDING) {
            if (tick == fastForwardStopAt) {
                fastForwardStopAt = NOT_FAST_FORWARDING;
                Time.timeScale = 1;
            } else {
                if (Time.timeScale != fastForwardSpeed) {
                    Time.timeScale = fastForwardSpeed;
                }
            }
        }

        if (tick == maximumTick) {
            EndLoop();
        }

        if (loopBreaks.Count != 0) {
            var loopBreak = loopBreaks.Peek();
            if (tick == loopBreak.tick) {
                int newActiveGooseId;
                if (loopBreak.isFixed) {
                    newActiveGooseId = loopBreak.originGooseId;
                    loopBreaks.Pop();
                } else {
                    newActiveGooseId = loopBreak.brokenGooseId;
                }
                var newActiveGoose = GetGoose(newActiveGooseId);
                newActiveGoose.SetState(GooseController.GooseState.ACTIVE);
                DisableGeeseAfter(newActiveGoose.GetId());
            }
        }

        // ++tick;

        foreach (var controller in geese) {
            if (!controller.IsGooseEnabled()) {
                continue;
            }
            controller.Goose_FixedUpdate();
            if (controller.GetState() == GooseController.GooseState.GHOOSTLING) {
                if (controller.LoopIsBroken() && controller.LoopIsFixable()) {
                    Debug.Log("Loop broken!");
                    var loopBreak = new LoopBreakStackFrame();
                    loopBreak.isFixed = false;
                    loopBreak.tick = tick;  // may or may not need to be tick-1
                    var activeGoose = GetActiveGoose();
                    var brokenGoose = controller;
                    loopBreak.originGooseId = activeGoose.GetId();
                    loopBreak.brokenGooseId = brokenGoose.GetId();
                    loopBreaks.Push(loopBreak);
                    DisableGeeseAfter(brokenGoose.GetId());
                    brokenGoose.SetState(GooseController.GooseState.GHOOSTLING);  // until FF ends
                    fastForwardStopAt = loopBreak.tick;
                    fastForwardSpeed = FAST_FORWARD_SPEED_AUTO;
                    ResetTick(doPause: false);
                    return;
                    //activeGoose.SetGooseEnabled(false);  // will need to re-enable when popping
                    //JumpToTick(tick);
                    //brokenGoose.SetState(GooseController.GooseState.ACTIVE);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.G)) {
            EndLoop();
        }

        /*
        // only allow manual fast forward if auto ff not active
        if (Mathf.Abs(Time.timeScale - FAST_FORWARD_SPEED_AUTO) > 0.01) {
            if (Input.GetKey(KeyCode.F)) {  // fast forward till the end
                Time.timeScale = FAST_FORWARD_SPEED_MANUAL;
            } else {
                Time.timeScale = 1;
            }

        }
        */

        UpdateDebugMenuText();

        ++tick;
    }

    // disable all geese with id greater than the given gooseId
    private void DisableGeeseAfter(int gooseId) {
        foreach (var goose in geese) {
            goose.SetGooseEnabled(goose.GetId() <= gooseId);
        }
    }

    private void ResetTick(bool doPause = true) {
        // TODO UI stuff for delay
        tick = 0;
        foreach(var ghoostling in geese){
            ghoostling.Goose_Reset();
        }
        int i = 0;
        foreach(var item in physicObjects){
            item.transform.position = startPositions[i];
            item.transform.rotation = startRotations[i];
            item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            item.GetComponent<Rigidbody>().angularVelocity =  new Vector3(0, 0, 0);
            i++;
        }
        pauseTicksRemaining = doPause ? PAUSE_TICKS : 10;
    }

    public void EndLoop() {
        if (loopBreaks.Count == 0) {
            SpawnActiveGoose();
        } else {
            var loopBreak = loopBreaks.Pop();  // .Peek() returns a fucking copy of the element...
            var fixedGoose = GetGoose(loopBreak.brokenGooseId);

            fixedGoose.SetState(GooseController.GooseState.GHOOSTLING);
            // newActiveGoose will be activated in FixedUpdate, when the frame is popped.

            loopBreak.isFixed = true;  // ...so this would have no effect if...
            loopBreaks.Push(loopBreak);  // ...we didn't Pop and Push the element.
            

            DisableGeeseAfter(loopBreak.originGooseId);  // re-enable all geese

            fastForwardStopAt = loopBreak.tick;
            fastForwardSpeed = FAST_FORWARD_SPEED_AUTO;
            ResetTick(doPause: false);
        }
    }

    public void SpawnActiveGoose() {

        GooseController activeGoose = geese[geese.Count - 1];
        activeGoose.Goose_Reset();
        activeGoose.SetState(GooseController.GooseState.GHOOSTLING);

        ResetTick();

        GameObject newGoose = GameObject.Instantiate(playerPrefab, transform);
        GooseController controller = newGoose.GetComponent<GooseController>();
        controller.Goose_Reset();
        controller.SetState(GooseController.GooseState.ACTIVE);

        // enable all geese
        DisableGeeseAfter(controller.GetId());
    }

    public GooseController GetActiveGoose() {
        foreach (var goose in geese) {
            if (goose.GetState() == GooseController.GooseState.ACTIVE) {
                return goose;
            }
        }
        Debug.LogError("No active goose found.");
        return null;
    }

    public bool IsFastForwarding() {
        return Time.timeScale > 1;
    }
    public bool IsAutoFastForwarding() {
        return Mathf.Abs(Time.timeScale - FAST_FORWARD_SPEED_AUTO) < 0.01f;
    }
    public bool IsPaused() {
        return pauseTicksRemaining > 0;
    }

    public (Vector3, Quaternion) GetSpawnLocation() {
        return gooseSpawn;
    }

    // Debug stuff
    private int _debug_line_tick;
    private int _debug_line_break_stack;
    private void InitDebugMenuLines() {
        var debug = DebugMenu.GetInstance();
        _debug_line_tick = debug.RegisterLine();
        _debug_line_break_stack = debug.RegisterLine();
    }
    private void UpdateDebugMenuText() {
        var debug = DebugMenu.GetInstance();
        string pauseText = "";
        if (pauseTicksRemaining > 0) {
            pauseText += "PAUSED for " + pauseTicksRemaining + " ticks";
        }
        if (fastForwardStopAt != NOT_FAST_FORWARDING) {
            pauseText += " FF to " + fastForwardStopAt;
        }
        debug.UpdateLine(_debug_line_tick, "tick: " + tick + " " + pauseText);

        string stackText = "Break stack: " + loopBreaks.Count + ".";
        if (loopBreaks.Count > 0) {
            var f = loopBreaks.Peek();
            stackText += " Goose " + f.originGooseId
                    + " broke " + f.brokenGooseId + " at t=" + f.tick
                    + (f.isFixed ? "fixed" : "unfixed");
        }
        debug.UpdateLine(_debug_line_break_stack, stackText);
    }

    public void DebugLog(string text) {
        Debug.Log("(t=" + tick + ") " + text);
    }
}
