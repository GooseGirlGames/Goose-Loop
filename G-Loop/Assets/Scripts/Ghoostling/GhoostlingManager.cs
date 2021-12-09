using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Exists once per Scene.  Transform position/rotation is Ghoosling spawn
 */
public class GhoostlingManager : MonoBehaviour {
    public const float PAUSE_TIME = 3.0f;
    private const float PAUSE_STEP_TIME = 0.05f;  // changes update rate of debug text while paused
    public GameObject playerPrefab;
    private List<GooseController> geese = new List<GooseController>();
    private int tick;
    private bool paused = false;
    private float pauseTimeRemaining;
    
    public void RegisterGoose(GooseController goose) {
        geese.Add(goose);
        Debug.Log("Registered " + goose.name + ".  Now managing " + geese.Count + " geese.");
    }

    void Start() {
        InitDebugMenuLines();
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
        if (paused) {
            UpdateDebugMenuText();
            return;
        }

        ++tick;

        foreach (var controller in geese) {
            controller.Goose_FixedUpdate();
            if (controller.GetState() == GooseController.GooseState.GHOOSTLING) {
                bool broken = controller.CheckForLoopBreak();
                if (broken) {
                    Debug.Log("Loop broken!");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.G)) {
            SpawnActiveGoose();
        }


        UpdateDebugMenuText();
    }
    private IEnumerator UnpauseAfterDelay() {
        while(pauseTimeRemaining > 0) {
            yield return new WaitForSeconds(PAUSE_STEP_TIME);
            pauseTimeRemaining -= PAUSE_STEP_TIME;
        }
        paused = false;
    }
    private void ResetTick() {
        // TODO UI stuff for delay
        paused = true;
        tick = 0;
        pauseTimeRemaining = PAUSE_TIME;
        StartCoroutine(UnpauseAfterDelay());
    }

    public void SpawnActiveGoose() {
        Debug.Log("Spawning new Goose at t=" + tick);

        GooseController activeGoose = geese[geese.Count - 1];
        activeGoose.ResetTransformToSpawn();
        activeGoose.SetState(GooseController.GooseState.GHOOSTLING);

        ResetTick();

        GameObject newGoose = GameObject.Instantiate(playerPrefab, transform);
        GooseController controller = newGoose.GetComponent<GooseController>();
        controller.ResetTransformToSpawn();
        controller.SetState(GooseController.GooseState.ACTIVE);
        
    }

    // Debug stuff
    private int _debug_line_tick;
    private void InitDebugMenuLines() {
        var debug = DebugMenu.GetInstance();
        _debug_line_tick = debug.RegisterLine();
    }
    private void UpdateDebugMenuText() {
        var debug = DebugMenu.GetInstance();
        string pauseText = "";
        if (paused) {
            pauseText += "PAUSED for " + pauseTimeRemaining.ToString("F2") + "secs";
        }
        debug.UpdateLine(_debug_line_tick, "tick: " + tick + " " + pauseText);
    }
}
